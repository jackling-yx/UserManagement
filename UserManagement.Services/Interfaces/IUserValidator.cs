using System;

namespace UserManagement.Services.Interfaces;
public interface IUserValidator
{
    bool IsValidEmail(string email);
    bool IsAdult(DateTime dateOfBirth);
}
