using MongoDB.Driver;
using Moq;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Test.Commands
{
    [TestFixture]
    public class UnidadeNotificationHandlerTests
    {
        private Mock<IUnidadeMongoDbRepository> _repositoryMock;
        private UnidadeNotificationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUnidadeMongoDbRepository>();
            _handler = new UnidadeNotificationHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_CreateNotification_ShouldCallAddAsync()
        {
            // Arrange
            var notification = new UnidadeCreateNotification { /* preencha os dados */ };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r =>
                r.AddAsync(notification, "Unidade"), Times.Once);
        }

        [Test]
        public async Task Handle_UpdateNotification_ShouldCallUpdateAsync()
        {
            // Arrange
            var id = 2;
            var notification = new UnidadeUpdateNotification { Id = id };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            // Como o filtro do MongoDB é um objeto complexo, verificamos se o método foi chamado
            // enviando a notificação e a coleção correta.
            _repositoryMock.Verify(r =>
                r.UpdateAsync(
                    It.IsAny<FilterDefinition<UnidadeNotification>>(),
                    notification,
                    "Unidade"),
                Times.Once);
        }

        [Test]
        public async Task Handle_DeleteNotification_ShouldCallRemoveAsync()
        {
            // Arrange
            var id = 2;
            var notification = new UnidadeDeleteNotification { Id = id };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r =>
                r.RemoveAsync(
                    It.IsAny<FilterDefinition<UnidadeNotification>>(),
                    "Unidade"),
                Times.Once);
        }
    }
}
