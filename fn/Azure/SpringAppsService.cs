using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.AppPlatform;

namespace Microsoft.Education
{
    public class SpringAppsService(string resourceGroup, string subscription) : AzureServiceBase(resourceGroup, subscription), IDeallocatableService
    {
        public async Task Down(string name)
        {
            var app = await base.GetResourceGroup().GetAppPlatformServiceAsync(name);
            await app.Value.StopAsync(WaitUntil.Started);
        }

        public async Task Up(string name)
        {
            var app = await base.GetResourceGroup().GetAppPlatformServiceAsync(name);
            await app.Value.StartAsync(WaitUntil.Started);
        }
    }
}