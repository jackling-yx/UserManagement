using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    private readonly ILogDataContext _logDataAccess;
    private readonly ILogService _logService;
    public UserService(IDataContext dataAccess, ILogDataContext logDataAccess, ILogService logService) {
        _dataAccess = dataAccess;
        _logDataAccess = logDataAccess;
        _logService = logService;
    }

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

            await _logService.CreateLog(LogLevel.Information, $"User with ID: {result.Value?.Id} created");

            return result;
        }
        catch (Exception ex)
        {
            await _logService.CreateLog(LogLevel.Error, $"Error creating user: {ex.Message}");
            return UnsuccessfulResult(ex.Message);
        }
    }

    public async Task<User> GetUserAsync(long id)
    {
        return await _dataAccess.GetUserAsync<User>(id);
    }

    public async Task<Result<User>> UpdateUser(long id, string forename, string surname, string email, DateTime dateOfBirth)
    {
        try
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

            await _logService.CreateLog(LogLevel.Information, $"User with ID: {id}, updated");
            return result;
        }
        catch(Exception ex)
        {
            await _logService.CreateLog(LogLevel.Error, $"Error updating user id {id}: {ex.Message}");
            return UnsuccessfulResult(ex.Message);
        }
    }

    public async Task<Result<User>> DeleteUserAsync(long id)
    {
        try
        {
            var userToDelete = await _dataAccess.GetUserAsync<User>(id);
            var result = await _dataAccess.DeleteAsync<User>(userToDelete);

            await _logService.CreateLog(LogLevel.Information, $"User with ID: {id}, deleted");
            return result;
        }
        catch (Exception ex)
        {
            await _logService.CreateLog(LogLevel.Error, $"Error deleting user id {id}: {ex.Message}");

            return UnsuccessfulResult(ex.Message);
        }
    }

    public async Task<Result<User>> ThrowExceptionAsync()
    {
        try
        {
            _dataAccess.ThrowException();
            return UnsuccessfulResult("Error");
        }
        catch (Exception ex)
        {
            await _logService.CreateLog(LogLevel.Error, ex.Message);

            return UnsuccessfulResult(ex.Message);
        }
    }

    private Result<User> UnsuccessfulResult(string message)
    {
        return new Result<User> { IsSuccess = false, Message = message, Value = null };
    }
}
