using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UnidadeEspacoSrv.Data.Repository.MongoDB;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class MongoDbBaseRepositoryTests
    {
        private Mock<IMongoContext> _mockContext;
        private Mock<IMongoCollection<EspacoNotification>> _mockCollection;
        private EspacoMongoDbRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IMongoContext>();
            _mockCollection = new Mock<IMongoCollection<EspacoNotification>>();

            // Mock do banco de dados
            var mockDatabase  = GlobalTestSetup.MongoDatabase;
            _mockContext.Setup(c => c.Database).Returns(mockDatabase);

            // Mock do GetCollection do Contexto
            _mockContext.Setup(c => c.GetCollection<EspacoNotification>(It.IsAny<string>()))
                        .Returns(_mockCollection.Object);

            _repository = new EspacoMongoDbRepository(_mockContext.Object);
        }

        private IAsyncCursor<EspacoNotification> CreateMockCursor(List<EspacoNotification> items)
        {
            var mockCursor = new Mock<IAsyncCursor<EspacoNotification>>();

            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                      .Returns(true)
                      .Returns(false);

            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);

            mockCursor.Setup(c => c.Current).Returns(items);

            return mockCursor.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _repository?.Dispose();
        }

        [Test]
        public async Task AddAsync_DeveChamarInsertOneAsyncDoDriver()
        {
            // Arrange
            var evento = new EspacoNotification { Nome = "Teste Evento" };

            // Act
            await _repository.AddAsync(evento, "Espaco");

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(
                evento,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

        [Test]
        public async Task GetById_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento"
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));

            
            // Act
            var resultado = await _repository.GetById(id, "Espaco");

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(id);
        }

        [Test]
        public async Task GetAll_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento"
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));


            // Act
            var resultado = await _repository.GetAll("Espaco");

            // Assert
            resultado.Should().NotBeNull();
           //  resultado.Id.Should().Be(id);
        }


        [Test]
        public async Task GetByCode_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento",
                _Id = ObjectId.GenerateNewId()
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));


            // Act
            var resultado = await _repository.GetByCode(evento._Id.ToString(),"Espaco");

            // Assert
            resultado.Should().NotBeNull();
            //  resultado.Id.Should().Be(id);
        }

        [Test]
        public async Task GetByFilter_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento",
                _Id = ObjectId.GenerateNewId()
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));


            // Act
            var filter = Builders<EspacoNotification>.Filter.Where(e => e.Id == evento.Id);
            var resultado = await _repository.GetByFilter(filter, "Espaco");

            // Assert
            resultado.Should().NotBeNull();
           
        }

        [Test]
        public async Task RemoveFilterAsync_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento",
                _Id = ObjectId.GenerateNewId()
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));

            _mockCollection
               .Setup(c => c.DeleteOneAsync(
                   It.IsAny<FilterDefinition<EspacoNotification>>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(new DeleteResult.Acknowledged(1));



            // Act
            var filter = Builders<EspacoNotification>.Filter.Where(e => e.Id == evento.Id);
            await _repository.RemoveAsync(filter, "Espaco");


            _mockCollection
              .Verify(c => c.DeleteOneAsync(
                  It.IsAny<FilterDefinition<EspacoNotification>>(),
                  It.IsAny<CancellationToken>()), Times.Once);
             
        }

        [Test]
        public async Task RemoveIdAsync_DeveRetornarObjeto_QuandoExistir()
        {
            // Arrange
            // Arrange
            var id = 123;
            var evento = new EspacoNotification
            {
                Id = id,
                Nome = "Teste Evento",
                _Id = ObjectId.GenerateNewId()
            };

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    It.IsAny<FindOptions<EspacoNotification, EspacoNotification>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockCursor(new List<EspacoNotification> { evento }));

            _mockCollection
               .Setup(c => c.DeleteOneAsync(
                   It.IsAny<FilterDefinition<EspacoNotification>>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(new DeleteResult.Acknowledged(1));



            // Act
            var filter = Builders<EspacoNotification>.Filter.Where(e => e.Id == evento.Id);
            await _repository.RemoveAsync(evento._Id.ToString(), "Espaco");


            _mockCollection
              .Verify(c => c.DeleteOneAsync(
                  It.IsAny<FilterDefinition<EspacoNotification>>(),
                  It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
