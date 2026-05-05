using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Interfaces.MongoDb
{
    public interface IMongoDbBaseRepository<TEntity> where TEntity : EventBase
    {
       // void Add(TEntity obj);
        Task AddAsync(TEntity obj, string collectionName = null);
        Task<TEntity> GetById(int id, string collectionName = null);
        Task<TEntity> GetByCode(string codigo, string collectionName = null);
        Task<IEnumerable<TEntity>> GetAll(string collectionName = null);
      //  Task Update(TEntity obj);
       // void Remove(string id);
        Task RemoveAsync(string id, string collectionName = null);
        Task<IEnumerable<TEntity>> GetByFilter(FilterDefinition<TEntity> filter, string collectionName = null);
        Task UpdateAsync(FilterDefinition<TEntity> filter, TEntity obj, string collectionName = null);
        Task RemoveAsync(FilterDefinition<TEntity> filter, string collectionName = null);
    }
}
