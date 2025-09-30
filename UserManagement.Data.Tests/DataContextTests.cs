using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetUserAsync_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        PopulateDb(context);

        var id = 1;

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetUserAsync<User>(id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result?.Forename.Should().Be("Existing");
        result?.Surname.Should().Be("User");
        result?.Email.Should().Be("existinguser@example.com");
        result?.DateOfBirth.Should().HaveYear(1990);
        result?.DateOfBirth.Should().HaveMonth(1);
        result?.DateOfBirth.Should().HaveDay(1);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    private DataContext CreateContext() => new();

    private void PopulateDb(DataContext context)
        {
        var entity = new User
        {
            Forename = "Existing",
            Surname = "User",
            Email = "existinguser@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

            context.Create(entity);
        }
}
