using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Data;
public interface ILogDataContext
{
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;
    Task<Result<TEntity>> CreateLogAsync<TEntity>(TEntity entity) where TEntity : class;
    Task<TEntity> GetLogAsync<TEntity>(long id) where TEntity : class;
}
