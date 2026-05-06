using MongoDB.Driver;
using System.Collections;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;
using UnidadeEspacoSrv.Domain.Utils;
using static System.Net.WebRequestMethods;

namespace UnidadeEspacoSrv.Data.Repository.MongoDB
{
    /// <summary>
    /// MongoDbBaseRepository: Implementação genérica do repositório para entidades MongoDB, fornecendo operações CRUD básicas.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MongoDbBaseRepository<TEntity> 
        : IDisposable, 
        IMongoDbBaseRepository<TEntity> where TEntity : EventBase
    {

        protected readonly IMongoContext _context;
        protected IMongoCollection<TEntity> collection;
        private readonly string _defaultCollection;

        protected MongoDbBaseRepository(IMongoContext context)
        {
            _context = context;

            //var collectionName = ((BsonCollectionAttribute)(typeof(TEntity) as Type)
            //        .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
            //        .FirstOrDefault())?.CollectionName;

            //collection = _context.Database.GetCollection<TEntity>(collectionName);
            _defaultCollection = typeof(TEntity).Name;

        }

        /// <summary>
        /// GetCollectionName: Método protegido para obter o nome da coleção MongoDB a partir do atributo BsonCollectionAttribute aplicado à classe de entidade.
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)

                .FirstOrDefault())?.CollectionName;
        }

        private IMongoCollection<TEntity> GetCollection(string collectionName = null)
        => _context.GetCollection<TEntity>(collectionName ?? _defaultCollection);

        /// <summary>
        /// Add: Método para adicionar um novo documento à coleção MongoDB. Insere o objeto fornecido na coleção correspondente.
        /// </summary>
        /// <param name="obj"></param>
        //public void Add(TEntity obj)
        //{
        //    collection.InsertOne(obj);
        //}

        /// <summary>
        /// AddAsync: Método assíncrono para adicionar um novo documento à coleção MongoDB. Insere o objeto fornecido na coleção correspondente de forma assíncrona.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task AddAsync(TEntity obj, string collectionName = null)
        {
            await GetCollection(collectionName).InsertOneAsync(obj);
        }

        /// <summary>
        /// dispose: Método para liberar os recursos utilizados pelo repositório. Chama GC.SuppressFinalize para evitar que o coletor de lixo finalize o objeto, já que não há recursos não gerenciados a serem liberados explicitamente.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// GetAll: Método assíncrono para obter todos os documentos da coleção MongoDB. Retorna uma lista de objetos do tipo TEntity representando todos os documentos presentes na coleção.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAll(string collectionName = null)
        {
            var result = await GetCollection(collectionName).FindAsync(Builders<TEntity>.Filter.Empty);
            return await result.ToListAsync();
        }

        /// <summary>
        /// GetByCode: Método assíncrono para obter um documento da coleção MongoDB com base em um código específico. Utiliza o filtro para encontrar o documento cujo campo _Id corresponde ao código fornecido e retorna o resultado como um objeto do tipo TEntity.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByCode(string codigo, string collectionName = null)
        {
            var result = await GetCollection(collectionName).FindAsync(Builders<TEntity>.Filter.Eq(doc => doc._Id.ToString(), codigo));
            return result.SingleOrDefault();
        }

        /// <summary>
        /// GetByFilter: Método assíncrono para obter documentos da coleção MongoDB com base em um filtro específico. Recebe um FilterDefinition<TEntity> como parâmetro, que define os critérios de filtragem, e retorna uma lista de objetos do tipo TEntity que correspondem ao filtro aplicado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetByFilter(FilterDefinition<TEntity> filter, string collectionName = null)
        {
            var result = await GetCollection(collectionName).FindAsync(filter);
            return result.ToEnumerable();
        }

        /// <summary>
        /// GetById: Método assíncrono para obter um documento da coleção MongoDB com base em um identificador numérico (id). Utiliza o filtro para encontrar o documento cujo campo Id corresponde ao valor fornecido e retorna o resultado como um objeto do tipo TEntity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetById(int id, string collectionName = null)
        {
            var result = await GetCollection(collectionName).FindAsync(Builders<TEntity>.Filter.Eq(doc => doc.Id, id));
            return result.SingleOrDefault();
        }

        /// <summary>
        /// Remove: Método para remover um documento da coleção MongoDB com base em um identificador de string (id). Utiliza o método DeleteOneAsync para excluir o documento cujo campo _Id corresponde ao valor fornecido. A operação é executada de forma assíncrona utilizando Task.Run para evitar bloqueios na thread principal.
        /// </summary>
        /// <param name="id"></param>
        //public void Remove(string id)
        //{
        //    Task.Run(() => collection
        //    .DeleteOneAsync(Builders<TEntity>.Filter.Eq(doc => doc._Id.ToString(), id)));
        //}

        /// <summary>
        /// Remove: Método para remover um documento da coleção MongoDB com base em um filtro específico. Recebe um FilterDefinition<TEntity> como parâmetro, que define os critérios de filtragem para identificar o documento a ser removido. O método busca o documento correspondente ao filtro, obtém seu identificador (_Id) e utiliza o método DeleteOneAsync para excluir o documento da coleção. A operação é executada de forma assíncrona utilizando Task.Run para evitar bloqueios na thread principal.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task RemoveAsync(FilterDefinition<TEntity> filter, string collectionName = null)
        {
            var data = await GetCollection(collectionName).FindAsync(filter);
            var existing = await data.FirstOrDefaultAsync();
            

            await GetCollection(collectionName).DeleteOneAsync(filter);
        }

        /// <summary>
        /// RemoveAsync: Método assíncrono para remover um documento da coleção MongoDB com base em um identificador de string (id). Utiliza o método DeleteOneAsync para excluir o documento cujo campo _Id corresponde ao valor fornecido. A operação é executada de forma assíncrona, permitindo que a thread principal continue a execução sem bloqueios.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id, string collectionName = null)
        {
            //var filter = 
           // var data = await GetCollection(collectionName).FindAsync(filter);
           // var existing = await data.FirstOrDefaultAsync();
            await GetCollection(collectionName).DeleteOneAsync(Builders<TEntity>.Filter.Eq(doc => doc._Id.ToString(), id));
        }

        /// <summary>
        /// Update: Método assíncrono para atualizar um documento existente na coleção MongoDB. Recebe um objeto do tipo TEntity como parâmetro, que representa o documento atualizado. O método utiliza o método ReplaceOneAsync para substituir o documento existente cujo campo _Id corresponde ao valor do objeto fornecido. A operação é executada de forma assíncrona, permitindo que a thread principal continue a execução sem bloqueios.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //public async Task Update(TEntity obj)
        //{
        //    await collection.ReplaceOneAsync(x => x._Id == obj._Id, obj);
        //}

        /// <summary>
        /// Update: Método assíncrono para atualizar um documento existente na coleção MongoDB com base em um filtro específico. Recebe um FilterDefinition<TEntity> como parâmetro, que define os critérios de filtragem para identificar o documento a ser atualizado, e um objeto do tipo TEntity que representa o documento atualizado. O método busca o documento correspondente ao filtro, obtém seu identificador (_Id) e utiliza o método ReplaceOneAsync para substituir o documento existente na coleção. A operação é executada de forma assíncrona, permitindo que a thread principal continue a execução sem bloqueios.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task UpdateAsync(FilterDefinition<TEntity> filter, TEntity obj, string collectionName = null)
        {
            
            var data = await GetCollection(collectionName).FindAsync(filter);
            var existing = await data.FirstOrDefaultAsync();
            if (existing != null)
            {
                obj._Id = existing._Id;
                await GetCollection(collectionName).ReplaceOneAsync(filter, obj);
            }
           
        }
    }
}
