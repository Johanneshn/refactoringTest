using LegacyApp.Services;
using System.Collections.Generic;
using System.Linq;

namespace LegacyApp.CreditProviders
{

    public interface ICreditLimitFactory
    {
        public ICreditLimitProvider GetCreditLimitProviderByClientName(string clientName);
    }

    public  class CreditLimitProviderFactory: ICreditLimitFactory
    {
        private readonly Dictionary<object, ICreditLimitProvider> _clientDictionary;

        public CreditLimitProviderFactory(IUserCreditService userCreditService)
        {
           
            var clients = new List<ICreditLimitProvider>()
            { 
                new VeryImportantClientCreditLimitProvider(userCreditService),
                new ImportanClientCreditLimitProvider(userCreditService),
                new UnspecializedClientCreditLimit(userCreditService)
            };

            _clientDictionary = clients.ToDictionary(c => c.GetType().GetProperty("ClientName").GetValue(c, null));
            
        }

        public ICreditLimitProvider GetCreditLimitProviderByClientName(string clientName) {

            if (_clientDictionary.TryGetValue(clientName, out var client)) {
                return client;
            }
            return DefaultClientLimitProvider();
        }

        private ICreditLimitProvider DefaultClientLimitProvider()
        {
            return _clientDictionary[string.Empty];
        }
    }
}
