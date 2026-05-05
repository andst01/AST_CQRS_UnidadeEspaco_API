using MongoDB.Bson;
using MongoDB.Driver;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Data.Repository.MongoDB
{
    /// <summary>
    /// EspaçoMongoDbRepository is a repository class responsible for handling MongoDB operations related to EspacoNotification entities. It inherits from MongoDbBaseRepository, which provides common MongoDB functionalities, and implements the IEspacoMongoDbRepository interface, ensuring that it adheres to the contract defined for MongoDB repositories in the domain layer.
    /// </summary>
    public class EspacoMongoDbRepository 
        : MongoDbBaseRepository<EspacoNotification>, 
        IEspacoMongoDbRepository
    {
        public EspacoMongoDbRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<EspacoReadModel>> ObterEspacosComUnidadesAsync()
        {
            // Agora a coleção principal é a de Espaço
            var collectionEspaco = _context.GetCollection<EspacoNotification>("Espaco");

            var pipeline = await collectionEspaco.Aggregate()
                // 1. Fazemos o Join: Procuramos na coleção "Unidade" 
                // onde o "IdEspaco" seja igual ao "Id" do Espaço atual.
                .Lookup(
                    foreignCollectionName: "Unidade",
                    localField: "Id",           // Campo no Espaço
                    foreignField: "IdEspaco",   // Campo na Unidade
                    @as: "Unidades"             // Nome do campo de array que será criado
                )
                // 2. Projetamos o resultado
                .Project(new BsonDocument
                    {
                { "Id", 1 },
                { "Endereco", 1 },
                { "Nome", 1 }, // Substitua pelos campos reais do seu Espaço
                { "Unidades", 1 } // Isso será um array de objetos Unidade
                    })
                .As<EspacoReadModel>()
                .ToListAsync();

            return pipeline;
        }
    }
}
