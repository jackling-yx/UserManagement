using System;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();
}

public class LogListItemViewModel
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
}
