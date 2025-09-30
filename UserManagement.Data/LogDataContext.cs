using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data;
public class LogDataContext : DbContext, ILogDataContext
{
    public LogDataContext() => Database.EnsureCreated();
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.LogsDataContext");

    protected override void OnModelCreating(ModelBuilder model)
        => model.Entity<Log>().HasData(new[]
        {
            new Log
            {
                Id = 1,
                Timestamp = DateTime.UtcNow,
                Level = LogLevel.Information,
                Message = "Logging started",
            },
            new Log
            {
                Id = 2,
                Timestamp = DateTime.UtcNow,
                Level = LogLevel.Information,
                Message = "Exception example",
                Exception = "Fake NullReferenceException"
            },
        });

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        =>  base.Set<TEntity>();

    public async Task<TEntity> GetLogAsync<TEntity>(long id) where TEntity : class
    {
        var log = await base.Set<TEntity>().FindAsync(new object[] { id });

        if (log == null)
        {
            throw new InvalidOperationException($"{typeof(TEntity)} with ID {id} not found.");
        }

        return log;
    }

    public async Task<Result<TEntity>> CreateLogAsync<TEntity>(TEntity entity) where TEntity : class
    {
        try
        {
            await base.AddAsync(entity);
            await SaveChangesAsync();

            return new Result<TEntity> { IsSuccess = true, Message = $"Log created", Value = entity };
        }
        catch
        {
            return new Result<TEntity> { IsSuccess = false, Message = $"Error creating log.", Value = null };
        }
    }
}
