using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    private readonly DataContext _context;
    private readonly DbContextOptions<DataContext> _options;

    public DataContextTests()
    {
        _options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext();
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        //var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };

        await _context.CreateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = _context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task CreateAsync_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        //var context = CreateContext();
        await PopulateDb(_context);

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await _context.CreateAsync(entity);

        var all = _context.GetAll<User>();

        var newUserId = _context.GetAll<User>().Last().Id;

        // Act: Invokes the method under test with the arranged parameters.
        var result = await _context.GetUserAsync<User>(newUserId);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result?.Forename.Should().Be("Brand New");
        result?.Surname.Should().Be("User");
        result?.Email.Should().Be("brandnewuser@example.com");
        result?.DateOfBirth.Should().HaveYear(1990);
        result?.DateOfBirth.Should().HaveMonth(1);
        result?.DateOfBirth.Should().HaveDay(1);
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var entity = _context.GetAll<User>().First();
        await _context.DeleteAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = _context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
        result.Should().NotContain(s => s.Id == entity.Id);
    }

    [Fact]
    public async Task UpdateAsync_SuccessfullyUpdatesEmail()
    {
        var entity = _context.GetAll<User>().First();
        var newEmail = "newemail@example.com";

        entity.Email = newEmail;

        await _context.UpdateAsync(entity);

        var result = await _context.GetUserAsync<User>(entity.Id);
        result.Email.Should().Be(newEmail);
        result.Forename.Should().Be(entity.Forename);
        result.Surname.Should().Be(entity.Surname);
        result.DateOfBirth.Should().Be(entity.DateOfBirth);
    }

    [Fact]
    public async Task UpdateAsync_SuccessfullyUpdatesDateOfBirth()
    {
        var entity = _context.GetAll<User>().First();
        var newDateOfBirth = new DateTime(2000, 12, 12);

        entity.DateOfBirth = newDateOfBirth;

        await _context.UpdateAsync(entity);

        var result = await _context.GetUserAsync<User>(entity.Id);
        result.Email.Should().Be(entity.Email);
        result.Forename.Should().Be(entity.Forename);
        result.Surname.Should().Be(entity.Surname);
        result.DateOfBirth.Should().Be(newDateOfBirth);
    }

    [Fact]
    public async Task UpdateAsync_SuccessfullyUpdatesName()
    {
        var entity = _context.GetAll<User>().First();
        var forename = "New";

        entity.Forename = forename;

        await _context.UpdateAsync(entity);

        var result = await _context.GetUserAsync<User>(entity.Id);
        result.Email.Should().Be(entity.Email);
        result.Forename.Should().Be(forename);
        result.Surname.Should().Be(entity.Surname);
        result.DateOfBirth.Should().Be(entity.DateOfBirth);
    }

    private async Task PopulateDb(DataContext context)
        {
        var entity = new User
        {
            Forename = "Existing",
            Surname = "User",
            Email = "existinguser@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

            await context.CreateAsync(entity);
        }
}
