using LegacyApp.Models;
using LegacyApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Validators
{
    internal static class UserValidators
    {
        public static bool UserHasCreditLimitAndItsLessThan500(User user)
        {
            return user.HasCreditLimit && user.CreditLimit < 500;
        }

        public static bool IsAtLeast21YearsOld(DateTime dateOfBirth, DateTime now)
        {
            int age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            if (age < 21)
            {
                return false;
            }

            return true;
        }

        public static bool HasValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public static bool HasValidFullName(string firname, string surname)
        {
            return !string.IsNullOrEmpty(firname) && !string.IsNullOrEmpty(surname);
        }
    }
}
