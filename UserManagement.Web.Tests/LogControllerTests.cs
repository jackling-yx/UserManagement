using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Data.Tests;

public class LogControllerTests
{
    private readonly Mock<ILogService> _mockLogService;
    private readonly LogsController _logsController;
    public LogControllerTests()
    {
        _mockLogService = new Mock<ILogService>();

        _logsController = new LogsController(_mockLogService.Object);
    }

    [Fact]
    public void List_ModelContainsAllLogs()
    {
        var logs = SetupLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = _logsController.List(1);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var model = result.Model as LogListViewModel;
        var expectedLogs = logs.Select(log => new LogListItemViewModel
        {
            Id = log.Id,
            Level = log.Level,
            Message = log.Message,
            Timestamp = log.Timestamp
        }).ToList();

        model.Should().NotBeNull();
        model.Items.Should().BeEquivalentTo(expectedLogs);

    }

    [Fact]
    public void List_PaginatesLogs()
    {
        var logs = SetupLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = _logsController.List(1);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var model = result.Model as LogListViewModel;

        var paginatedLogs = result.ViewData["PaginatedLogs"] as List<LogListItemViewModel>;

        model.Should().NotBeNull();

        paginatedLogs.Should().HaveCount(5);
    }

    [Fact]
    public async Task View_ReturnsCorrectLog()
    {
        var logs = SetupLogs();
        long id = 1;

        var logResult = new Result<Log>
        {
            IsSuccess = true,
            Message = "Success",
            Value = logs.First(log => log.Id == id)
        };

        _mockLogService.Setup(x => x.GetLogAsync(id).Result).Returns(logResult);

        var result = await _logsController.View(id);

        var model = result.Model as LogListItemViewModel;

        model.Should().NotBeNull();
        model.Id.Should().Be(logs.First().Id);
        model.Level.Should().Be(logs.First().Level);
        model.Message.Should().Be(logs.First().Message);
        model.Timestamp.Should().BeCloseTo(logs.First().Timestamp, TimeSpan.FromSeconds(1));
        result.ViewName.Should().Be("View");
    }

    [Fact]
    public async Task View_HandlesNoLogFound()
    {
        var logs = SetupLogs();
        long id = 0;

        var logResult = new Result<Log>
        {
            IsSuccess = false,
            Message = "Log not found",
            Value = null
        };

        _mockLogService.Setup(x => x.GetLogAsync(id).Result).Returns(logResult);
        var result = await _logsController.View(id);
        result.ViewName.Should().Be("List");
    }

    private Log[] SetupLogs()
    {
        var logs = new[]
        {
            new Log
            {
                Id = 1,
                Level = LogLevel.Information,
                Message = "Info message",
                Timestamp = DateTime.UtcNow,
            },
            new Log
            {
                Id = 2,
                Level = LogLevel.Warning,
                Message = "Warning message",
                Timestamp = DateTime.UtcNow,
            },
            new Log
            {
                Id = 3,
                Level = LogLevel.Trace,
                Message = "Trace message",
                Timestamp = DateTime.UtcNow,
            },
            new Log
            {
                Id = 4,
                Level = LogLevel.Debug,
                Message = "Debug message",
                Timestamp = DateTime.UtcNow,
            },
            new Log
            {
                Id = 5,
                Level = LogLevel.Error,
                Message = "Error message",
                Timestamp = DateTime.UtcNow,
            },
            new Log
            {
                Id = 6,
                Level = LogLevel.Critical,
                Message = "Critical message",
                Timestamp = DateTime.UtcNow,
            }
        };

        _mockLogService
            .Setup(s => s.GetAll())
            .Returns(logs);

        return logs;
    }
}
