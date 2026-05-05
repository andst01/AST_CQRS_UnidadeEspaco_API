using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Data.Contexto
{
    /// <summary>
    /// MongoDbContext: Contexto de acesso ao MongoDB, responsável por configurar as convenções de serialização e fornecer acesso às coleções do banco.
    /// </summary>
    public class MongoDbContext : IMongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoDbContext(IConfiguration configuration)
        {
           
            var connectionString = configuration.GetSection("MongoDBConnection:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoDBConnection:DataBaseName").Value;

           
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// GetCollection: Método genérico para obter uma coleção do MongoDB, permitindo acesso fácil e tipado às coleções do banco.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return Database.GetCollection<T>(name);
        }
    }
}
