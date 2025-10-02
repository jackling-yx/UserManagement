using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = controller.List();

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_WhenServiceReturnsUsersOnlyActiveUsers()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = controller.ActiveOnly();

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.ForEach(user => user.IsActive.Should().BeTrue());
    }

    [Fact]
    public void List_WhenServiceReturnsUsersOnlyInactiveUsers()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = controller.InactiveOnly();

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.ForEach(user => user.IsActive.Should().BeFalse());
    }

    [Theory]
    [InlineData(1, "Johnny", "User", "juser@example.com", "1990-01-01", true)]
    [InlineData(2, "John", "Smith", "jsmith@example.com", "1994-02-02", false)]
    public async Task View_ReturnsCorrectUser(long id, string forename, string surname, string email, string dateOfBirth, bool isActive)
    {
        var controller = CreateController();
        var users = SetupUsers();


        _userService.Setup<User>(x => x.GetUserAsync(id).Result).Returns(users.First(user => user.Id == id));

        var result = await controller.ViewUser(id);

        result.Model.Should().BeOfType<UserListItemViewModel>()
            .Which.Should().BeEquivalentTo(new UserListItemViewModel
            {
                Id = id,
                Forename = forename,
                Surname = surname,
                Email = email,
                DateOfBirth = DateTime.Parse(dateOfBirth),
                IsActive = isActive
            });
    }

    [Fact]
    public async Task View_HandlesNoUserFound()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = await controller.ViewUser(0);
        result.Model.Should().BeNull();
    }

    [Fact]
    public async Task Add_CreatesUserSuccessfully()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var forename = "New";
        var surname = "User";
        var email = "nuser@example.com";
        var dateOfBirth = new DateTime(1991, 1, 1);

        _userValidator.Setup(x => x.IsValidEmail(It.IsAny<string>())).Returns(true);
        _userValidator.Setup(x => x.IsAdult(It.IsAny<DateTime>())).Returns(true);

        _userService.Setup(x => x.CreateUserAsync(forename, surname, email, dateOfBirth))
            .ReturnsAsync(new Result<User> { IsSuccess = true, Message = "User created", Value = new User { Id = 3, Forename = forename, Surname = surname, Email = email, DateOfBirth = dateOfBirth, IsActive = false } });

        var result = await controller.AddUser(forename, surname, email, dateOfBirth);

        result.Should().NotBeNull();
        _userService.Verify(x => x.CreateUserAsync(forename, surname, email, dateOfBirth), Times.Once);
    }

    [Fact]
    public async Task Add_HandlesInvalidProperties()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var forename = "New";
        var surname = "User";
        var email = "nuser@example.com.";
        var dateOfBirth = new DateTime(3000, 1, 1);

        _userValidator.Setup(x => x.IsValidEmail(It.IsAny<string>())).Returns(false);
        _userValidator.Setup(x => x.IsAdult(It.IsAny<DateTime>())).Returns(false);

        _userService.Setup(x => x.CreateUserAsync(forename, surname, email, dateOfBirth))
            .ReturnsAsync(new Result<User> { IsSuccess = true, Message = "User created", Value = new User { Id = 3, Forename = forename, Surname = surname, Email = email, DateOfBirth = dateOfBirth, IsActive = false } });


        var result = await controller.AddUser(forename, surname, email, dateOfBirth);

        result.Should().NotBeNull();
        _userService.Verify(x => x.CreateUserAsync(forename, surname, email, dateOfBirth), Times.Never);
    }

    [Fact]
    public async Task Delete_DeletesUserSuccessfully()
    {
        var controller = CreateController();
        var users = SetupUsers();
        var user = users.First(u => u.Id == 1);

        _userService.Setup(x => x.DeleteUserAsync(1).Result).Returns(new Result<User> {  IsSuccess = true, Message = "Delete successful", Value = user });
        var result = await controller.DeleteUser(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new RedirectToActionResult("List", null, null));
    }

    [Fact]
    public async Task Delete_HandlesNoUserFound()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = await controller.DeleteUser(0);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new RedirectToActionResult("List", null, null));
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new User
            {
                Id = 2,
                Forename = "John",
                Surname = "Smith",
                Email = "jsmith@example.com",
                IsActive = false,
                DateOfBirth = new DateTime(1994, 2, 2)
            }
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }

    private readonly Mock<IUserService> _userService = new Mock<IUserService>();
    private readonly Mock<IUserValidator> _userValidator = new Mock<IUserValidator>();
    private UsersController CreateController() => new(_userService.Object, _userValidator.Object);
}
