using FluentAssertions;
using MediatR;
using MongoDB.Driver;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Data.Repository.MongoDB;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class EspacoMongoRepositoryTests
    {
        private IMongoCollection<EspacoNotification> _collection;
        private IEspacoMongoDbRepository _repository;
        private Mock<IMongoContext> _mockContext;
        private Mock<IMongoCollection<EspacoNotification>> _mockCollection;
        private Mock<IMongoCollection<UnidadeNotification>> _mockUnidadeCollection;

        [SetUp]
        public void Setup()
        {

            _mockContext = new Mock<IMongoContext>();
            _mockCollection = new Mock<IMongoCollection<EspacoNotification>>();
            _mockUnidadeCollection = new Mock<IMongoCollection<UnidadeNotification>>();

            // Mock do banco de dados
            // var mockDatabase = GlobalTestSetup.MongoDatabase;
            _mockContext = new Mock<IMongoContext>();

            var database = GlobalTestSetup.MongoDatabase;
            if (database == null)
            {
                throw new Exception("O banco de dados de teste (GlobalTestSetup.MongoDatabase) não foi inicializado!");
            }

            _mockContext.Setup(c => c.Database).Returns(database);

            // Mock do GetCollection do Contexto
            _mockContext.Setup(c => c.GetCollection<EspacoNotification>(It.IsAny<string>()))
            .Returns((IMongoCollection<EspacoNotification>)_mockCollection.Object);



            _repository = new EspacoMongoDbRepository(_mockContext.Object);
        }

        [Test]
        public async Task ObeterEspacoComUnidade_Test()
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


            var result = await _repository.ObterEspacosComUnidadesAsync();

            result.Should().NotBeNull();
        }

        [Test]
        public async Task AddAsync_DevePersistirNoBancoReal()
        {

            // Arrange
            var entidade = new EspacoNotification
            {
                Nome = "Audit Log 1",
                Id = 1,
                Endereco = "Teste"
            };


            // / Você PRECISA do Setup para o Mock não dar erro ao ser chamado
            _mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<EspacoNotification>(),
                It.IsAny<InsertOneOptions>(),
                default(CancellationToken)))
            .Returns(Task.CompletedTask);

            // Act
            await _repository.AddAsync(entidade, "Espaco");


            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(
                It.Is<EspacoNotification>(c => c.Nome == "Audit Log 1"),
                It.IsAny<InsertOneOptions>(),
                default(CancellationToken)), Times.Once);


        }

        [Test]
        public async Task UpdateAsync_DevePersistirNoBancoReal()
        {
            // Arrange
            var entidade = new EspacoNotification { Nome = "Audit Log 1" };
            entidade._Id = new MongoDB.Bson.ObjectId(); // Simula um ID gerado pelo MongoDB

            // Ajuste o Setup para aceitar qualquer opção e token
            _mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<EspacoNotification>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

            await _repository.AddAsync(entidade, "Espaco");

            // Act

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IAsyncCursor<EspacoNotification>>())
                .Verifiable();

            _mockCollection.Setup(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<EspacoNotification>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, entidade.Id))
                .Verifiable();


            entidade.Nome = "Audit Log 1 - Updated";
            await _repository.UpdateAsync(Builders<EspacoNotification>.Filter.Where(e => e.Id == entidade.Id), entidade, "Espaco");

            // Assert
            _mockCollection.Verify(c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<EspacoNotification>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            // Dispose ou DisposeAsync se disponível
            if (_repository is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (_repository is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _collection = null;
        }
    }
}
