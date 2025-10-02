using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;

namespace UserManagement.Data.Tests;

public class LogDataContextTests
{
    private readonly LogDataContext _context;
    private readonly DbContextOptions<LogDataContext> _options;

    public LogDataContextTests()
    {
        _options = new DbContextOptionsBuilder<LogDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new LogDataContext();
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void GetAll_ReturnsAllLogs()
    {
        // Act: Invokes the method under test with the arranged parameters.
        var result = _context.GetAll<Log>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetLogAsync_ReturnsCorrectLog()
    {
        var logs = _context.GetAll<Log>();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await _context.GetLogAsync<Log>(logs.First().Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Level.Should().Be(LogLevel.Information);
        result.Value.Message.Should().Be("Logging started");
        result.Value.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetLogAsync_HandlesException()
    {
        var result = await _context.GetLogAsync<Log>(0);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<Result<Log>>();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task CreateLogAsync_SuccessfullyCreatesLog()
    {
        var message = "Test critical error";
        var exceptionMessage = "Test exception message";

        var log = new Log
        {
            Level = LogLevel.Critical,
            Message = message,
            Exception = exceptionMessage,
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await _context.CreateLogAsync<Log>(log);

        var allLogs = await _context.GetAll<Log>().ToListAsync();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(log);

        allLogs.Should().Contain(log => log.Message == message
        && log.Exception == exceptionMessage
        && log.Level == LogLevel.Critical);
    }
}
