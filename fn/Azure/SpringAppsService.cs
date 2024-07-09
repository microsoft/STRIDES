using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.AppPlatform;

namespace Microsoft.Education
{
    public class SpringAppsService(string subscription) : AzureServiceBase(subscription), IDeallocatableService
    {
        public async Task Down(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var app = await subscription.GetResourceGroup(resourceGroup).Value.GetAppPlatformServiceAsync(name);
            await app.Value.StopAsync(WaitUntil.Started);
        }

        public async Task Up(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var app = await subscription.GetResourceGroup(resourceGroup).Value.GetAppPlatformServiceAsync(name);
            await app.Value.StartAsync(WaitUntil.Started);
        }
    }
}