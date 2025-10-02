using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Controllers;
public class LogsController : Controller
{
    private readonly ILogService _logService;
    private const int ResultsPerPage = 5;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    [HttpGet("all")]
    public ViewResult List(int page)
    {
        var logs = _logService.GetAll().Select(log => new LogListItemViewModel
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            Message = log.Message,
            Exception = log.Exception,
        }).ToList();

        var model = new LogListViewModel
        {
            Items = logs
        };

        var firstPaginatedIndex = (page - 1) * ResultsPerPage;
        var lastPaginatedIndex = logs.Count % 5  == 0 ? (page * ResultsPerPage) - 1 : logs.Count;

        ViewBag.PaginatedLogs = logs.Skip((page - 1) * ResultsPerPage).Take(ResultsPerPage).ToList();

        ViewBag.CurrentPage = page;


        return View(model);
    }

    [HttpGet("view")]
    public async Task<ViewResult> View(long id)
    {
        var result = await _logService.GetLogAsync(id);

        if (result.Value == null)
        {
            return View("List");
        }

        var model = new LogListItemViewModel
        {
            Id = result.Value.Id,
            Timestamp = result.Value.Timestamp,
            Level = result.Value.Level,
            Message = result.Value.Message,
            Exception = result.Value.Exception
        };

        return View("View", model);
    }

    [HttpPost]
    public ActionResult CreateFakeException()
    {
        var exception = new Exception("Fake exception created for demo");
        _logService.CreateLog(LogLevel.Error, exception.Message);

        return RedirectToAction("List", new { page = 1 });
    }
}
