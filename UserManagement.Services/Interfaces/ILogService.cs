using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;
public interface ILogService
{
    IEnumerable<Log> GetAll();
    Task<Result<Log>> CreateLog(LogLevel logLevel, string message);
    Task<Result<Log>> GetLogAsync(long id);
}
