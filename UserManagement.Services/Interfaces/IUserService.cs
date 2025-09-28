using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();
    Task<Result<User>> CreateUserAsync(string forename, string surname, string email, DateTime dateOfBirth);
    Task<User> GetUserAsync(long id);
    Task<Result<User>> UpdateUser(long id, string forename, string surname, string email, DateTime dateOfBirth);
    Task<Result<User>> DeleteUserAsync(long id);
}
