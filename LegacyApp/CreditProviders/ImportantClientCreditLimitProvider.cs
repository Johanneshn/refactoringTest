using LegacyApp.Models;
using LegacyApp.Services;
using System;

namespace LegacyApp.CreditProviders
{
    internal class ImportantClientCreditLimitProvider : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public ImportantClientCreditLimitProvider(IUserCreditService userCreditService)
        {
            this._userCreditService = userCreditService;
        }

        public string ClientName => "ImportantClient";

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
            return (true, creditLimit * 2);
            
        }
    }
}
