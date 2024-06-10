using System.ComponentModel.DataAnnotations;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace Microsoft.Education
{
    public class AzureServiceBase
    {
        private ArmClient _client;
        private ResourceGroupResource? _resourceGroup;
        private SubscriptionResource? _subscription;

        public ResourceGroupResource? GetResourceGroup()
        {
            return _resourceGroup;
        }

        public AzureServiceBase(string subscription, string resourceGroup)
        {
            _client = new ArmClient(new DefaultAzureCredential());
            _subscription = _client.GetSubscriptions().GetAsync(subscription).Result;
            _resourceGroup = _subscription.GetResourceGroupAsync(resourceGroup).Result;
        }
        
    }
}