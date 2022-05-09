using LegacyApp.CreditProviders;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;
using System;

namespace LegacyApp
{
    public class UserService
    {

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IClientRepository _clientRepository;
        private readonly IUserDataAccess _userDataAccess;
        private readonly ICreditLimitFactory _creditLimitFactory;

        public UserService(): this(new DateTimeProvider(), new ClientRepository(), new CreditLimitProviderFactory(new UserCreditServiceClient()), new UserDataAccessProxy()) { }

        public UserService(
            IDateTimeProvider dateTimeProvider, 
            IClientRepository clientRepository,
            ICreditLimitFactory creditLimitFactory,
            IUserDataAccess userDataAccess)
        {
            _dateTimeProvider = dateTimeProvider;
            _clientRepository = clientRepository;
            _creditLimitFactory = creditLimitFactory;
            _userDataAccess = userDataAccess;
        }

   
        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            if (UserValidators.HasValidFullName(firname, surname) == false)
            {
                return false;
            }

            if (UserValidators.HasValidEmail(email) == false)
            {
                return false;
            }

            if (UserValidators.IsAtLeast21YearsOld(dateOfBirth, _dateTimeProvider.DateTimeNow) == false)
            {
                return false;
            }


            var client = _clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            var provider = _creditLimitFactory.GetCreditLimitProviderByClientName(client.Name);
            var creditLimit = provider.GetCreditLimits(user);

            user.HasCreditLimit = creditLimit.HasCreditLimit;
            user.CreditLimit = creditLimit.CreditLimit;

    
            if (UserValidators.UserHasCreditLimitAndItsLessThan500(user))
            {
                return false;
            }

            _userDataAccess.AddUser(user);

            return true;
        }


       
    }
}