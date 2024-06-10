namespace Microsoft.Education
{
    public class DeallocatableServiceManager(string subscription, string resourceGroup)
    {
        private ManagedContainerClusterService _containerService = new ManagedContainerClusterService(subscription, resourceGroup);
        private SpringAppsService _springAppsService = new SpringAppsService(subscription, resourceGroup);
        private SynapseSQLPoolService _synapseService = new SynapseSQLPoolService(subscription, resourceGroup);
        private VirtualMachineService _vmService = new VirtualMachineService(subscription, resourceGroup);

        public IDeallocatableService Get(string service)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return service switch
            {
                "Microsoft.ContainerService/managedClusters" => _containerService,
                "Microsoft.AppPlatform/Spring" => _springAppsService,
                "Microsoft.Synapse/workspaces" => _synapseService,
                "Microsoft.Compute/virtualMachines" => _vmService,
                _ => null,
            };
            #pragma warning restore CS8603 // Possible null reference return.
        }
    }
}