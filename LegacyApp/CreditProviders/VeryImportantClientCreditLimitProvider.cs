using LegacyApp.Models;
using LegacyApp.Services;

namespace LegacyApp.CreditProviders
{
    internal class VeryImportantClientCreditLimitProvider : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public VeryImportantClientCreditLimitProvider(IUserCreditService userCreditService)
        {
            this._userCreditService = userCreditService;
        }

        public string ClientName => "VeryImportantClient";

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            return (false, 0);
        }
    }
}
