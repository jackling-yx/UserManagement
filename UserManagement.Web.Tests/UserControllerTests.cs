using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_WhenServiceReturnsUsersOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.ActiveOnly();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.ForEach(user => user.IsActive.Should().BeTrue());
    }

    [Fact]
    public void List_WhenServiceReturnsUsersOnlyInactiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.InactiveOnly();

        // Assert: Verifies that the action of the method under test behaves as expected.
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

        var result = await controller.ViewUser(99);
        result.Model.Should().BeNull();
    }

    [Fact]
    public async Task Add_CreatesUserSuccessfully()
    {
        var controller = CreateController();
        var users = SetupUsers();

        var result = await controller.AddUser("New", "User", "nuser@gmail.com", new DateTime(1991, 1, 1));

        result.Model.Should().NotBeNull();
        result.Model.Should().BeOfType<UserListItemViewModel>()
            .Which.Should().BeEquivalentTo(new UserListItemViewModel
            {
                Id = 3,
                Forename = "New",
                Surname = "User",
                Email = "nuser@gmail.com",
            });
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
