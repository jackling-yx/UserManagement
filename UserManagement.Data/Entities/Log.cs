using System;

namespace UserManagement.Data.Entities;

public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}

public class Log
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public LogLevel Level { get; set; } = LogLevel.Information;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public string? Exception { get; set; }
}
