using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;

namespace Microsoft.Education
{
    public class VirtualMachineService(string resourceGroup, string subscription) : AzureServiceBase(resourceGroup, subscription), IDeallocatableService
    {
        public async Task Down(string name)
        {
            var vm = await base.GetResourceGroup().GetVirtualMachineAsync(name);
            await vm.Value.DeallocateAsync(WaitUntil.Started);
        }

        public async Task Up(string name)
        {
            var vm = await base.GetResourceGroup().GetVirtualMachineAsync(name);
            await vm.Value.PowerOnAsync(WaitUntil.Started);
        }
    }
}