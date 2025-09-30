using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Data;

public interface IDataContext
{
    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    void Create<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Update an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<Result<TEntity>> CreateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task<TEntity> GetUserAsync<TEntity>(long id) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    Task<Result<TEntity>> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    void Delete<TEntity>(TEntity entity) where TEntity : class;
    Task<Result<TEntity>> DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
    Exception ThrowException();
}
