using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerService;

namespace Microsoft.Education
{
    public class ManagedContainerClusterService(string subscription) : AzureServiceBase(subscription), IDeallocatableService
    {
        public async Task Down(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var cluster = subscription.GetResourceGroup(resourceGroup).Value.GetContainerServiceManagedCluster(name);
            await cluster.Value.StopAsync(WaitUntil.Started);
        }

        public async Task Up(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var cluster = subscription.GetResourceGroup(resourceGroup).Value.GetContainerServiceManagedCluster(name);
            await cluster.Value.StartAsync(WaitUntil.Started);
        }
    }
}