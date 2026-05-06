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
            var collectionUnidade = _context.GetCollection<UnidadeNotification>("Unidade");

            var pipeline = await collectionEspaco.Aggregate()

                .Lookup<EspacoNotification, UnidadeNotification, EspacoReadModel>(
                        foreignCollection: collectionUnidade,
                        localField: e => e.Id,           // Chave local (Espaço)
                        foreignField: u => u.IdEspaco,     // Chave estrangeira (Unidade)
                        @as: res => res.Unidades  // Onde salvar o array resultante no EspacoReadModel
                    )
                .Project(x => new EspacoReadModel
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Endereco = x.Endereco,
                    Unidades = x.Unidades //.Where(u => u.Ativo).ToList()
                }).ToListAsync();

            #region Lista Espaçoa
            //.Lookup(
            //    foreignCollectionName: "Unidade",
            //    localField: "Id",           // Campo no Espaço
            //    foreignField: "IdEspaco",   // Campo na Unidade
            //    @as: "Unidades"             // Nome do campo de array que será criado
            //)
            //.Project(new BsonDocument
            //    {
            //{ "Id", 1 },
            //{ "Endereco", 1 },
            //{ "Nome", 1 }, // Substitua pelos campos reais do seu Espaço
            //{ "Unidades", 1 } // Isso será um array de objetos Unidade
            //    })
            //.As<EspacoReadModel>()
            //.ToListAsync();
            #endregion

            return pipeline;
        }


       
    }
}
