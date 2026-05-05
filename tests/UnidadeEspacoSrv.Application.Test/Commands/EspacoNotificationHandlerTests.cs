using MongoDB.Driver;
using Moq;
using UnidadeEspacoSrv.Application.Commands;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Test.Commands
{
    [TestFixture]
    public class EspacoNotificationHandlerTests
    {
        private Mock<IEspacoMongoDbRepository> _repositoryMock;
        private EspacoNotificationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IEspacoMongoDbRepository>();
            _handler = new EspacoNotificationHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_CreateNotification_ShouldCallAddAsync()
        {
            // Arrange
            var notification = new EspacoCreateNotification { /* preencha os dados */ };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r =>
                r.AddAsync(notification, "Espaco"), Times.Once);
        }

        [Test]
        public async Task Handle_UpdateNotification_ShouldCallUpdateAsync()
        {
            // Arrange
            var id = 2;
            var notification = new EspacoUpdateNotification { Id = id };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            // Como o filtro do MongoDB é um objeto complexo, verificamos se o método foi chamado
            // enviando a notificação e a coleção correta.
            _repositoryMock.Verify(r =>
                r.UpdateAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    notification,
                    "Espaco"),
                Times.Once);
        }

        [Test]
        public async Task Handle_DeleteNotification_ShouldCallRemoveAsync()
        {
            // Arrange
            var id = 2;
            var notification = new EspacoDeleteNotification { Id = id };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r =>
                r.RemoveAsync(
                    It.IsAny<FilterDefinition<EspacoNotification>>(),
                    "Espaco"),
                Times.Once);
        }

    }
}
