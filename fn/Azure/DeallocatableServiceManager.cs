namespace Microsoft.Education
{
    public class DeallocatableServiceManager(string subscription)
    {
        private ManagedContainerClusterService _containerService = new(subscription);
        private SpringAppsService _springAppsService = new(subscription);
        private SynapseSQLPoolService _synapseService = new(subscription);
        private VirtualMachineService _vmService = new(subscription);

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