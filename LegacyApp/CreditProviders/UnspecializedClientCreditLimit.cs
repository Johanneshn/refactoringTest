using LegacyApp.Models;
using LegacyApp.Services;
using System;

namespace LegacyApp.CreditProviders
{
    internal class UnspecializedClientCreditLimit : ICreditLimitProvider
    {
        private readonly IUserCreditService _userCreditService;

        public UnspecializedClientCreditLimit(IUserCreditService userCreditService)
        {
            this._userCreditService = userCreditService;
        }

        public string ClientName => String.Empty;

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(User user)
        {
            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
            return (true, creditLimit);
        }
    }
}
