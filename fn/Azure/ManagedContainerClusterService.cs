using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerService;

namespace Microsoft.Education
{
    public class ManagedContainerClusterService(string subscription, string resourceGroup) : AzureServiceBase(subscription, resourceGroup), IDeallocatableService
    {
        public async Task Down(string name)
        {
            var cluster = base.GetResourceGroup().GetContainerServiceManagedCluster(name);
            await cluster.Value.StopAsync(WaitUntil.Started);
        }

        public async Task Up(string name)
        {
            var cluster = base.GetResourceGroup().GetContainerServiceManagedCluster(name);
            await cluster.Value.StartAsync(WaitUntil.Started);
        }
    }
}