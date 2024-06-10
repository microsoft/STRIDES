using Microsoft.AspNetCore.Components.Web;
using Microsoft.Azure;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Education
{
    public class ResourceService(string resourceGroup, string subscription) : AzureServiceBase(resourceGroup, subscription)
    {
        public async Task<List<DeallocatableResource>> GetResources(string filter = "")
        {
            var resourceGroup = base.GetResourceGroup() ?? throw new Exception("Resource service returned no resource groups.");
            
            var resources = String.IsNullOrWhiteSpace(filter) ? 
                resourceGroup.GetGenericResourcesAsync().AsPages() : 
                resourceGroup.GetGenericResourcesAsync(filter).AsPages();

            var deallocatables = new List<DeallocatableResource>();

            await resources.ForEachAsync(page =>
            {
                foreach (var resource in page.Values)
                {
                    deallocatables.Add(new DeallocatableResource(){ Name = resource.Data.Name, Type = resource.Data.ResourceType.ToString() });
                }
            });

            return deallocatables; 
        }

    }
}