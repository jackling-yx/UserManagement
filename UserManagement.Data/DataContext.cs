using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model)
        => model.Entity<User>().HasData(new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = new DateTime(1990, 1, 1)},
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = new DateTime(1991, 2, 2) },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = new DateTime(1992, 3, 3) },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true, DateOfBirth = new DateTime(1993, 4, 4) },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true, DateOfBirth = new DateTime(1994, 5, 5) },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true, DateOfBirth = new DateTime(1995, 6, 6) },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false, DateOfBirth = new DateTime(1996, 7, 7) },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false, DateOfBirth = new DateTime(1997, 8, 8) },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false, DateOfBirth = new DateTime(1998, 9, 9) },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true, DateOfBirth = new DateTime(1999, 10, 10) },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true, DateOfBirth = new DateTime(2000, 11, 11) },
        });

    public DbSet<User>? Users { get; set; }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    public async Task<TEntity> GetUserAsync<TEntity>(long id) where TEntity : class
    {
        var user = await base.Set<TEntity>().FindAsync(new object[] { id });

        if (user == null)
        {
            throw new InvalidOperationException($"Entity of type {typeof(TEntity)} with ID {id} not found.");
        }

        return user;
    }

    public void Create<TEntity>(TEntity entity) where TEntity : class
    {
        base.Add(entity);
        SaveChanges();
    }
    public async Task<Result<TEntity>> CreateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await base.AddAsync(entity);
        var result = await SaveChangesAsync();

        if (result == 0)
        {
            return new Result<TEntity> { IsSuccess = false, Message = "Creation failed.", Value = entity };
        }

        return new Result<TEntity> { IsSuccess = true, Message = "Creation successful.", Value = entity };
    }

    public new void Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        SaveChanges();
    }

    public async Task<Result<TEntity>> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        var result = await SaveChangesAsync();

        if (result == 0)
        {
            return new Result<TEntity> { IsSuccess = false, Message = "Update failed.", Value = entity };
        }

        return new Result<TEntity> { IsSuccess = true, Message = "Update successful.", Value = entity };
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        SaveChanges();
    }

    public async Task<Result<TEntity>> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        var result = await SaveChangesAsync();

        if (result == 0)
        {
            return new Result<TEntity> { IsSuccess = false, Message = "Delete failed.", Value = entity };
        }

        return new Result<TEntity> { IsSuccess = true, Message = "Delete successful.", Value = entity };
    }
}
