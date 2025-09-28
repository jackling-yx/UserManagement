using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(user => user.IsActive == isActive);
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public async Task<Result<User>> CreateUserAsync(string forename, string surname, string email, DateTime dateOfBirth)
    {
        try
        {

            var user = new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = false,
                DateOfBirth = dateOfBirth
            };

            var result = await _dataAccess.CreateAsync<User>(user);

            return result;
        }
        catch (Exception ex)
        {
            return new Result<User> { IsSuccess = false, Message = ex.Message, Value = null };
        }
    }

    public async Task<User> GetUserAsync(long id)
    {
        return await _dataAccess.GetUserAsync<User>(id);
    }

    public async Task<Result<User>> UpdateUser(long id, string forename, string surname, string email, DateTime dateOfBirth)
    {

        var user = new User
        {
            Id = id,
            Forename = forename,
            Surname = surname,
            Email = email,
            IsActive = false,
            DateOfBirth = dateOfBirth
        };

        var result = await _dataAccess.UpdateAsync<User>(user);
        return result;
    }

    public async Task<Result<User>> DeleteUserAsync(long id)
    {
        try
        {
            var userToDelete = await _dataAccess.GetUserAsync<User>(id);
            var result = await _dataAccess.DeleteAsync<User>(userToDelete);

            return result;
        }
        catch
        {
            return new Result<User> { IsSuccess = false, Message = "Delete failed.", Value = null };
        }
    }
}
