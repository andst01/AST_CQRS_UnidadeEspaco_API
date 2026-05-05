
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Data.Repository.MongoDB
{
    /// <summary>
    /// UnidadeMongoDbRepository is a repository class responsible for handling MongoDB operations related to UnidadeNotification entities. It inherits from MongoDbBaseRepository, which provides common MongoDB functionalities, and implements the IUnidadeMongoDbRepository interface, ensuring that it adheres to the contract defined for MongoDB repositories in the domain layer.
    /// </summary>
    public class UnidadeMongoDbRepository
        : MongoDbBaseRepository<UnidadeNotification>,
        IUnidadeMongoDbRepository
    {
        public UnidadeMongoDbRepository(IMongoContext context) : base(context)
        {
        }

        /// <summary>
        /// Asynchronously retrieves all units, including their associated space information, from the data store.
        /// </summary>
        /// <remarks>The returned list includes each unit with its related space information, if such a
        /// relationship exists. This method does not filter or paginate results.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of unit read models, each
        /// including space data if available. The list is empty if no units are found.</returns>
        public async Task<List<UnidadeReadModel>> ObterTodasUnidadeComEspaco()
        {
            /// Unidade Filha
            var collecrtionUnidade = _context.GetCollection<UnidadeNotification>("Unidade");

            var pipeline = await collecrtionUnidade.Aggregate()
                .Lookup(foreignCollectionName: "Espaco",
                        localField: "IdEspaco",
                        foreignField: "Id",
                        @as: "Espaco")
                .Unwind("Espaco", new AggregateUnwindOptions<BsonDocument>
                { PreserveNullAndEmptyArrays = true })
                .Project(new BsonDocument
                    {
                        { "Id", 1 },
                        { "Rede", 1 },
                        { "IdEspaco", 1 },
                        { "Espaco", 1 } // O Mongo apenas repassa o objeto que veio do Lookup
                    }).As<UnidadeReadModel>()
                    .ToListAsync();

            return pipeline;

        }

        /// <summary>
        /// Asynchronously retrieves a unit by its unique identifier, including related space information.
        /// </summary>
        /// <remarks>The returned unit includes associated space data if available. If no unit with the
        /// specified identifier exists, the result is null.</remarks>
        /// <param name="id">The unique identifier of the unit to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unit read model if found;
        /// otherwise, null.</returns>
        public async Task<UnidadeReadModel> ObterUnidadeComEspacoPorIdAsync(int id)
        {
            /// Unidade Filha
            var collecrtionUnidade = _context.GetCollection<UnidadeNotification>("Unidade");
           
            var pipeline = await collecrtionUnidade.Aggregate()
                .Match(Builders<UnidadeNotification>.Filter.Eq(e => e.Id, id))
                .Lookup(foreignCollectionName: "Espaco",
                        localField: "IdEspaco",
                        foreignField: "Id",
                        @as: "Espaco")
                .Unwind("Espaco", new AggregateUnwindOptions<BsonDocument>
                { PreserveNullAndEmptyArrays = true })
                .Project(new BsonDocument
                    {
                        { "Id", 1 },
                        { "Rede", 1 },
                        { "IdEspaco", 1 },
                        { "Espaco", 1 } // O Mongo apenas repassa o objeto que veio do Lookup
                    }).As<UnidadeReadModel>()
                    .FirstOrDefaultAsync();

            return pipeline;

        }
    
    }
}
