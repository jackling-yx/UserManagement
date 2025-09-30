using System;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Validators;
public class UserValidator : IUserValidator
{

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.EndsWith(".") )
            return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool IsAdult(DateTime dateOfBirth)
    {
        var today = DateTime.Today;

        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age)) {
            age--;
        };

        return age >= 18;
    }
}
