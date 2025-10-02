using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
public class LogService : ILogService
{
    private readonly ILogDataContext _logDataAccess;
    public LogService(ILogDataContext logDataContext)
    {
        _logDataAccess = logDataContext;
    }

    public IEnumerable<Log> GetAll()
    {
        return _logDataAccess.GetAll<Log>(); 
    }

    public async Task<Result<Log>> CreateLog(LogLevel logLevel, string message)
    {
        var log = new Log();
        try
        {
            log.Level = logLevel;
            log.Message = message;
            log.Timestamp = DateTime.UtcNow;
            log.IsSuccess = true;

            await _logDataAccess.CreateLogAsync(log);

            return new Result<Log> { IsSuccess = true, Message = "Success", Value = log };
        }
        catch(Exception ex)
        {
            log.Level = logLevel;
            log.Message = message;
            log.Timestamp = DateTime.UtcNow;
            log.IsSuccess = false;
            log.Exception = ex.Message;

            return new Result<Log> { IsSuccess = false, Message = ex.Message, Value = log };
        }
    }

    public async Task<Result<Log>> GetLogAsync(long id)
    {
        try
        {
            var log = await _logDataAccess.GetLogAsync<Log>(id);

            return log;
        }
        catch(Exception ex)
        {
            return new Result<Log> { IsSuccess = false, Message = ex.Message, Value = null };
        }
    }
}
