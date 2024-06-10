using Microsoft.Azure;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Synapse;

namespace Microsoft.Education
{
    public class SynapseSQLPoolService(string resourceGroup, string subscription) : AzureServiceBase(resourceGroup, subscription), IDeallocatableService
    {
        public async Task Down(string name)
        {
            var workspace = await base.GetResourceGroup().GetSynapseWorkspaceAsync(name);
            var pools = workspace.Value.GetSynapseSqlPools();
            await pools.ForEachAsync(async pool => await pool.PauseAsync(WaitUntil.Started));      
        }

        public async Task Up(string name)
        {
            var workspace = await base.GetResourceGroup().GetSynapseWorkspaceAsync(name);
            var pools = workspace.Value.GetSynapseSqlPools();
            await pools.ForEachAsync(async pool => await pool.ResumeAsync(WaitUntil.Started));
        }
    }
}                                                                                       