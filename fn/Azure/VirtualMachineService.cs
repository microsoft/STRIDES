using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;

namespace Microsoft.Education
{
    public class VirtualMachineService(string subscription) : AzureServiceBase(subscription), IDeallocatableService
    {
        public async Task Down(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var vm = await subscription.GetResourceGroup(resourceGroup).Value.GetVirtualMachineAsync(name);
            await vm.Value.DeallocateAsync(WaitUntil.Started);
        }

        public async Task Up(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var vm = await subscription.GetResourceGroup(resourceGroup).Value.GetVirtualMachineAsync(name);
            await vm.Value.PowerOnAsync(WaitUntil.Started);
        }
    }
}