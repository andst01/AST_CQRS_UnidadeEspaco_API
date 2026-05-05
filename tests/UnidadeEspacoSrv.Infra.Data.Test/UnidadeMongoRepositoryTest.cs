using FluentAssertions;
using MongoDB.Driver;
using Moq;
using UnidadeEspacoSrv.Data.Repository.MongoDB;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class UnidadeMongoRepositoryTest
    {
        private IMongoCollection<UnidadeNotification> _collection;
        private IUnidadeMongoDbRepository _repository;
        private Mock<IMongoContext> _mockContext;
        private Mock<IMongoCollection<UnidadeNotification>> _mockCollection;
        private Mock<IMongoCollection<EspacoNotification>> _mockEspacoCollection;

        [SetUp]
        public void Setup()
        {

            _mockContext = new Mock<IMongoContext>();
            _mockCollection = new Mock<IMongoCollection<UnidadeNotification>>();
            _mockEspacoCollection = new Mock<IMongoCollection<EspacoNotification>>();

            _mockContext = new Mock<IMongoContext>();

            var database = GlobalTestSetup.MongoDatabase;
            if (database == null)
            {
                throw new Exception("O banco de dados de teste (GlobalTestSetup.MongoDatabase) não foi inicializado!");
            }

            _mockContext.Setup(c => c.Database).Returns(database);

            // Mock do GetCollection do Contexto
            _mockContext.Setup(c => c.GetCollection<UnidadeNotification>(It.IsAny<string>()))
            .Returns((IMongoCollection<UnidadeNotification>)_mockCollection.Object);

            _repository = new UnidadeMongoDbRepository(_mockContext.Object);
        }

        [Test]
        public async Task ObterTodasUnidadeComEspaco_Test()
        {

            _mockContext.Setup(c => c.GetCollection<UnidadeNotification>("Unidade"))
                .Returns(GlobalTestSetup.MongoDatabase.GetCollection<UnidadeNotification>("Unidade"));

            _mockContext.Setup(c => c.GetCollection<EspacoNotification>("Espaco"))
                .Returns(GlobalTestSetup.MongoDatabase.GetCollection<EspacoNotification>("Espaco"));

            var entidade = new EspacoNotification { Id = 2, Nome = "Audit Log 1" };
            var unidade = new UnidadeNotification { Id = 1, IdEspaco = 2, Rede = "Unidade 1" };

            var colEspaco = _mockContext.Object.GetCollection<EspacoNotification>("Espaco");
            colEspaco.InsertOne(entidade);

            var colUnidade = _mockContext.Object.GetCollection<UnidadeNotification>("Unidade");
            colUnidade.InsertOne(unidade);

            //await _repository.AddAsync(entidade, "Espaco");


            var result = await _repository.ObterTodasUnidadeComEspaco();

            result.Should().NotBeNull();
        }


        [Test]
        public async Task ObterUnidadeComEspacoPorIdAsync_Test()
        {

            _mockContext.Setup(c => c.GetCollection<UnidadeNotification>("Unidade"))
                .Returns(GlobalTestSetup.MongoDatabase.GetCollection<UnidadeNotification>("Unidade"));

            _mockContext.Setup(c => c.GetCollection<EspacoNotification>("Espaco"))
                .Returns(GlobalTestSetup.MongoDatabase.GetCollection<EspacoNotification>("Espaco"));

            var entidade = new EspacoNotification { Id = 2, Nome = "Audit Log 1" };
            var unidade = new UnidadeNotification { Id = 1, IdEspaco = 2, Rede = "Unidade 1" };

            var colEspaco = _mockContext.Object.GetCollection<EspacoNotification>("Espaco");
            colEspaco.InsertOne(entidade);

            var colUnidade = _mockContext.Object.GetCollection<UnidadeNotification>("Unidade");
            colUnidade.InsertOne(unidade);

            //await _repository.AddAsync(entidade, "Espaco");

            var result = await _repository.ObterUnidadeComEspacoPorIdAsync(1);

            result.Should().NotBeNull();
        }


        [TearDown]
        public void TearDown()
        {
            if (_repository is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _repository = null;
            _mockCollection = null;
            _mockContext = null;
            _collection = null;
        }
    }
}
