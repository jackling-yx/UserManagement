using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Interfaces;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public async Task GetUser_ReturnsCorrectUser()
    {
        var service = CreateService();
        var users = SetupUsers();
        long id = 1;

        _dataContext.Setup(x => x.GetUserAsync<User>(id)).ReturnsAsync(users.First(user => user.Id == 1));

        var result = await service.GetUserAsync(1);

        result.Forename.Should().Be(users.First().Forename);
        result.Surname.Should().Be(users.First().Surname);
        result.Email.Should().Be(users.First().Email);
        result.DateOfBirth.Should().Be(users.First().DateOfBirth);
    }

    [Fact]
    public async Task GetUser_HandlesUserNotFound()
    {
        var service = CreateService();
        var users = SetupUsers();

        var result = await service.GetUserAsync(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUser_UpdatesUserDetails()
    {
        var service = CreateService();
        var users = SetupUsers();
        long id = 1;

        string newForename = "Updated";
        string newSurname = "User";
        string newEmail = "@uuser@example.com";
        DateTime newDateOfBirth = new DateTime(1993, 3, 3);

        var updatedUser = new User
        {
            Id = id,
            Forename = newForename,
            Surname = newSurname,
            Email = newEmail,
            IsActive = true,
            DateOfBirth = newDateOfBirth
        };

        _dataContext.Setup(x => x.GetUserAsync<User>(id)).ReturnsAsync(users.First(user => user.Id == id));
        _dataContext.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(
            new Result<User>
            {
                IsSuccess = true,
                Value = updatedUser,
                Message = "User updated successfully"
            });

        var result = await service.UpdateUser(id, newForename, newSurname, newEmail, newDateOfBirth);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.Forename.Should().Be(newForename);
        result.Value.Surname.Should().Be(newSurname);
        result.Value.Email.Should().Be(newEmail);
        result.Value.DateOfBirth.Should().Be(newDateOfBirth);

        _dataContext.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.Id == id &&
            u.Forename == newForename &&
            u.Surname == newSurname &&
            u.Email == newEmail &&
            u.DateOfBirth == newDateOfBirth
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_HandlesExceptionFromDb()
    {
        var service = CreateService();
        var users = SetupUsers();
        long id = 1;
        var errorMessage = "Error";

        string newForename = "Updated";
        string newSurname = "User";
        string newEmail = "@uuser@example.com";
        DateTime newDateOfBirth = new DateTime(1993, 3, 3);

        var updatedUser = new User
        {
            Id = id,
            Forename = newForename,
            Surname = newSurname,
            Email = newEmail,
            IsActive = true,
            DateOfBirth = newDateOfBirth
        };


        _dataContext.Setup(x => x.GetUserAsync<User>(id)).ReturnsAsync(users.First(user => user.Id == id));
        _dataContext.Setup(x => x.UpdateAsync(It.IsAny<User>())).ThrowsAsync(
            new Exception(errorMessage));

        var result = await service.UpdateUser(id, newForename, newSurname, newEmail, newDateOfBirth);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUser_SuccessfullyDeletesUser()
    {
        var service = CreateService();
        var users = SetupUsers();
        long id = 1;

        _dataContext.Setup(x => x.GetUserAsync<User>(id)).ReturnsAsync(users.First(user => user.Id == id));
        _dataContext.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(
            new Result<User>
            {
                IsSuccess = true,
                Value = users.First(user => user.Id == id),
                Message = "User deleted successfully"
            });

        var result = await service.DeleteUserAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(id);
        _dataContext.Verify(x => x.DeleteAsync(It.Is<User>(u => u.Id == id)), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_HandlesExceptionFromDb()
    {
        var service = CreateService();
        var users = SetupUsers();
        long id = 1;
        var errorMessage = "Error";

        _dataContext.Setup(x => x.GetUserAsync<User>(id)).ReturnsAsync(users.First(user => user.Id == id));
        _dataContext.Setup(x => x.DeleteAsync(It.IsAny<User>())).ThrowsAsync(
            new Exception(errorMessage));

        var result = await service.DeleteUserAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
        result.Value.Should().BeNull();
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Id = 1,
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = new DateTime(1991, 1, 1)
            },
            new User
            {
                Id = 2,
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = new DateTime(1992, 2, 2)
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    //Maintaining code style consistency for this test class
    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<ILogDataContext> _logDataContext = new();
    private readonly Mock<ILogService> _logService = new();
    private UserService CreateService() => new(_dataContext.Object, _logDataContext.Object, _logService.Object);
}
