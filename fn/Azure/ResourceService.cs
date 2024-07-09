using Microsoft.AspNetCore.Components.Web;
using Microsoft.Azure;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Education
{
    public class ResourceService(string subscription) : AzureServiceBase(subscription)
    {
        public async Task<List<DeallocatableResource>> GetResources(string filter = "")
        {
            var subscription = base.GetSubscription() ?? throw new Exception("Service base returned no subscriptions.");

            var resources = String.IsNullOrWhiteSpace(filter) ? 
            subscription.GetGenericResourcesAsync().AsPages() : 
            subscription.GetGenericResourcesAsync(filter).AsPages();

             var deallocatables = new List<DeallocatableResource>();

            await resources.ForEachAsync(page =>
            {
                foreach (var resource in page.Values)
                {
                    deallocatables.Add(new DeallocatableResource(){ Name = resource.Data.Name, Type = resource.Data.ResourceType.ToString(), ResourceGroup = resource.Data.Id.ResourceGroupName });
                }
            });

            return deallocatables; 
        }

    }
}