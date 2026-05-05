using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Interfaces.SQL
{
    public interface ISQLBaseRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, object id);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();

       
    }
}
