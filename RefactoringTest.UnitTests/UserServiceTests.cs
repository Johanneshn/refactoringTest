using AutoFixture;
using FluentAssertions;
using LegacyApp;
using LegacyApp.CreditProviders;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RefactoringTest.UnitTests
{
    public class UserServiceTests
    {

        private readonly UserService _sut; 

        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly IUserDataAccess _userDataAccess= Substitute.For<IUserDataAccess>();
        private readonly IUserCreditService _userCreditService = Substitute.For<IUserCreditService>();
        private readonly ICreditLimitFactory _creditLimitFactory = Substitute.For<ICreditLimitFactory>();

        private readonly IFixture _fixture = new Fixture();
        public UserServiceTests()
        {
            _sut = new UserService(_dateTimeProvider, _clientRepository, new CreditLimitProviderFactory(_userCreditService), _userDataAccess);
        }

        [Fact]
        public void AddUser_ShouldCreateUser_WhenAllParametersAreValid()
        {
            // Arrange
            const int clientId = 1;
            const string firstName = "John";
            const string lastName = "Doe";
            var dateOfBirth = new DateTime(1989, 1, 1);

            var client = _fixture.Build<Client>()
                .With(c => c.Id, clientId)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2022,05,03));
            _clientRepository.GetById(clientId).Returns(client);
            _userCreditService.GetCreditLimit(firstName, lastName, dateOfBirth).Returns(1000);


            // Act 
            var result = _sut.AddUser(firstName, lastName, "john.doe@gmail.com", dateOfBirth, clientId);


            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received(1).AddUser(Arg.Any<User>()); 
        }


        [Theory]
        [InlineData("", "Doe", "john.doe@gmail.com", 1989)]
        [InlineData("John", "", "john.doe@gmail.com", 1989)]
        [InlineData("John", "Doe", "johndoe@gmailcom", 1989)]
        [InlineData("John", "Doe", "john.doe@gmail.com", 2012)]
        public void AddUser_ShouldNotCreateUser_WhenInputDetailsAreInvalid(
            string firstName, string lastName, string email, int yearOfBirth)
        {

            // Arrange
            const int clientId = 1;
            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2022, 05, 03));
            var dateOfBirth = new DateTime(yearOfBirth, 1, 1);


            var client = _fixture.Build<Client>()
                .With(c => c.Id, clientId)
                .Create();
            _clientRepository.GetById(clientId).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(1000);

            // Act
            var result = _sut.AddUser(firstName, lastName, email, dateOfBirth, clientId);

            // Assert
            result.Should().BeFalse();

        }


        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenUserHasCreditLimitAndLimitIsBelow500()
        {
            // Arrange
            const int clientId = 1;
            const string firstName = "John";
            const string lastName = "Doe";
            var dateOfBirth = new DateTime(1989, 02, 24);

            var client = _fixture.Build<Client>()
                .With(c => c.Id, clientId)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2022, 05, 03));
            _clientRepository.GetById(Arg.Is(clientId)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(400);


            // Act 
            var result = _sut.AddUser(firstName, lastName, "email@mail.com", dateOfBirth, clientId);

            // Assert 
            result.Should().BeFalse();

        }


        [Theory]
        [InlineData("", true, 600, 600)]
        [InlineData("ImportantClient", true, 500, 1000)]
        [InlineData("VeryImportantClient", false, 0, 0)]
        public void AddUser_ShouldCreateUserWithCorrectCreditLimit_WhenNameIndicatesDifferentClassification(
            string clientName, bool hasCreditLimit, int initialCreditLimit, int finalCreditLimit)
        {

            // Arrange
            const int clientId = 1;
            const string firstName = "John";
            const string lastName = "Doe";
            var dateOfBirth = new DateTime(1989, 02, 24);

            var client = _fixture.Build<Client>()
                .With(c => c.Id, clientId)
                .With(c => c.Name, clientName)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2022, 05, 03));
            _clientRepository.GetById(Arg.Is(clientId)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(initialCreditLimit);
                
            // Act
            var result = _sut.AddUser("John", "Doe", "email@mail.com", new DateTime(1989,02,24), clientId);

            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received()
                .AddUser(Arg.Is<User>(user => user.HasCreditLimit == hasCreditLimit && user.CreditLimit == finalCreditLimit));
        }
    }
}
