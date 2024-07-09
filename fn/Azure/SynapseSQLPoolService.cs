using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Synapse;

namespace Microsoft.Education
{
    public class SynapseSQLPoolService(string subscription) : AzureServiceBase(subscription), IDeallocatableService
    {
        public async Task Down(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var workspace = await subscription.GetResourceGroup(resourceGroup).Value.GetSynapseWorkspaceAsync(name);
            var pools = workspace.Value.GetSynapseSqlPools();
            await pools.ForEachAsync(async pool => await pool.PauseAsync(WaitUntil.Started));      
        }

        public async Task Up(string name, string resourceGroup)
        {
            var subscription = base.GetSubscription();
            if (subscription == null) { return; }
            var workspace = await subscription.GetResourceGroup(resourceGroup).Value.GetSynapseWorkspaceAsync(name);
            var pools = workspace.Value.GetSynapseSqlPools();
            await pools.ForEachAsync(async pool => await pool.ResumeAsync(WaitUntil.Started));
        }
    }
}                                                                                       