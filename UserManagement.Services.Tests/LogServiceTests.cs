using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;
public class LogServiceTests
{
    //Using code style from what I am used to
    private readonly Mock<ILogDataContext> _logDataContext;
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _logDataContext = new Mock<ILogDataContext>();
        _logService = new LogService(_logDataContext.Object);
    }

    [Fact]
    public async Task GetLogAsync_ReturnsCorrectLog()
    {
        var logs = SetupLogs();
        long id = 1;

        var logResult = new Result<Log>
        {
            IsSuccess = true,
            Message = "Success",
            Value = logs.First(log => log.Id == id)
        };

        _logDataContext.Setup(x => x.GetLogAsync<Log>(id).Result).Returns(logResult);

        var result = await _logService.GetLogAsync(1);

        result.Value.Should().NotBeNull();
        result.Value.IsSuccess.Should().Be(logs.First().IsSuccess);
        result.Value.Level.Should().Be(logs.First().Level);
        result.Value.Message.Should().Be(logs.First().Message);
        result.Value.Timestamp.Should().Be(logs.First().Timestamp);
    }

    [Fact]
    public async Task GetLogAsync_HandlesException()
    {
        var logs = SetupLogs();
        long id = 100;
        var exceptionMessage = "Test exception";
        var mockDataResult = new Result<Log> { IsSuccess = false, Message = exceptionMessage, Value = null };

        _logDataContext.Setup(x => x.GetLogAsync<Log>(id)).ThrowsAsync(new Exception(exceptionMessage));

        var result = await _logService.GetLogAsync(100);

        result.Value.Should().BeNull();
        result.Message.Should().Be(exceptionMessage);
        result.IsSuccess.Should().BeFalse();

        _logDataContext.Verify(x => x.GetLogAsync<Log>(id), Times.Once);
    }

    [Fact]
    public async Task CreateLogAsync_CreatesLogSuccessfully()
    {
        var logs = SetupLogs();
        var logLevel = LogLevel.Information;
        var message = "Success";

        _logDataContext.Setup(x => x.CreateLogAsync(It.IsAny<Log>())).ReturnsAsync(
            new Result<Log> { IsSuccess = true, Message = "Log created", Value = new Log { Id = 4, Level = logLevel, Message = message, Timestamp = DateTime.UtcNow, IsSuccess = true } });

        var result = await _logService.CreateLog(logLevel, message);

        result.Value.Should().NotBeNull();
        result.Value.Level.Should().Be(logLevel);
        result.Value.Message.Should().Be(message);
        result.Value.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Success");

        _logDataContext.Verify(x => x.CreateLogAsync<Log>(It.IsAny<Log>()), Times.Once);
    }

    [Fact]
    public async Task CreateLogAsync_HandlesException()
    {
        var logs = SetupLogs();
        var logLevel = LogLevel.Error;
        var message = "Failed to create log";
        var exceptionMessage = "Test exception";

        _logDataContext.Setup(x => x.CreateLogAsync(It.IsAny<Log>())).ThrowsAsync(new Exception(exceptionMessage));
        var result = await _logService.CreateLog(logLevel, message);

        result.Value.Should().NotBeNull();
        result.Value.Level.Should().Be(logLevel);
        result.Value.Message.Should().Be(message);
        result.Value.IsSuccess.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(exceptionMessage);

        _logDataContext.Verify(x => x.CreateLogAsync<Log>(It.IsAny<Log>()), Times.Once);
    }

    private IQueryable<Log> SetupLogs()
    {
        var logs = new[]
        {
            new Log { Id = 1, Level = LogLevel.Information, Message = "Info message", Timestamp = DateTime.UtcNow, IsSuccess = true },
            new Log { Id = 2, Level = LogLevel.Warning, Message = "Warning message", Timestamp = DateTime.UtcNow, IsSuccess = true },
            new Log { Id = 3, Level = LogLevel.Error, Message = "Error message", Timestamp = DateTime.UtcNow, IsSuccess = false, Exception = "Test exception" }
        }.AsQueryable();

        _logDataContext
            .Setup(s => s.GetAll<Log>())
            .Returns(logs.AsQueryable());

        return logs;
    }
}
