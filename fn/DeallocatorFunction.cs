using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace Microsoft.Education
{
    public class DeallocatorFunction
    {
        private readonly ILogger<DeallocatorFunction> _logger;
        private string? subscription = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");
        private string? resourceGroup = Environment.GetEnvironmentVariable("AZURE_RESOURCE_GROUP");
        private ResourceService _resourceService;
        private DeallocatableServiceManager _serviceManager;

        public DeallocatorFunction(ILogger<DeallocatorFunction> logger)
        {
            _logger = logger;
            if (string.IsNullOrEmpty(subscription) || string.IsNullOrEmpty(resourceGroup))
            {
                _logger.LogError("Subscription or resourceGroup is null");
                throw new ArgumentNullException("Subscription or resourceGroup is null");
            }
            _resourceService = new ResourceService(subscription, resourceGroup);
            _serviceManager = new DeallocatableServiceManager(subscription, resourceGroup);
        }

        [Function("DeallocatorFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
          
            var query = req.Query.ContainsKey("filter") ? req.Query["filter"].ToString() : "";
            var action = req.Query.ContainsKey("action") ? req.Query["action"].ToString().ToLowerInvariant() : "";

            var resources = new List<DeallocatableResource>();
            try { resources = _resourceService.GetResources(query).Result; }
            catch (Exception ex) { return new OkObjectResult(new DeallocatableResourceResponse() { Action = action, ResourceCount = resources.Count, Resources = resources, Error = ex.Message.ToString() }); }

            if (resources.Count == 0) { return new OkObjectResult(new DeallocatableResourceResponse() { Resources = resources, Action = action, ResourceCount = resources.Count}); }
            if (string.IsNullOrWhiteSpace(action)) { return new OkObjectResult(new DeallocatableResourceResponse() { Resources = resources, Action = action, ResourceCount = resources.Count, Error = "Action must be 'up' or 'down'."}); }

            Execute(resources, action);

            return new OkObjectResult(new DeallocatableResourceResponse() { Action = action, ResourceCount = resources.Count, Resources = resources});

        }

        private void Execute(List<DeallocatableResource> resources, string action)
        {
            foreach (var resource in resources)
            {
                if (resource == null || resource.Type == null || resource.Name == null) continue;
                var service = _serviceManager.Get(resource.Type);
                if (service == null) { resource.Error = "Not a dellocatable service."; continue; }
                try { if (action == "up") { service.Up(resource.Name).Wait(); } else {service.Down(resource.Name).Wait(); }}
                catch (Exception ex) { resource.Error = ex.Message.ToString(); continue; }
            }
        }

    }
}

/*
Supports standard Azure RM filters:
resourceType eq 'Microsoft.Compute/virtualMachines' OR resourceType eq 'Microsoft.ContainerService/managedClusters' OR resourceType eq 'Microsoft.Synapse/workspaces/sqlPools'
http://localhost:7071/api/DeallocatorFunction?filter=resourceType%20eq%20%27Microsoft.Compute/virtualMachines%27%20OR%20resourceType%20eq%20%27Microsoft.ContainerService/managedClusters%27%20OR%20resourceType%20eq%20%27Microsoft.Synapse/workspaces/sqlPools%27
tagName eq 'shutdown' AND tagValue eq 'true'
http://localhost:7071/api/DeallocatorFunction?filter=tagName%20eq%20%27shutdown%27%20AND%20tagValue%20eq%20%27true%27
Read more: https://learn.microsoft.com/en-us/rest/api/resources/resources/list?view=rest-resources-2021-04-01
*/