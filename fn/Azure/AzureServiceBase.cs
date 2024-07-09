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
        private SubscriptionResource? _subscription;

        public SubscriptionResource? GetSubscription()
        {
            return _subscription;
        }

        public AzureServiceBase(string subscription)
        {
            _client = new ArmClient(new DefaultAzureCredential());
            _subscription = _client.GetSubscriptions().GetAsync(subscription).Result;
        }
        
    }
}