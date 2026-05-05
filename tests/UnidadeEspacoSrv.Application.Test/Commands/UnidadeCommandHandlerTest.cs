using AutoMapper;
using FluentValidation.Results;
using Moq;
using Moq.Protected;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;
using static UnidadeEspacoSrv.Application.Test.Commands.EspacoCommandHandlerTest;

namespace UnidadeEspacoSrv.Application.Test.Commands
{
    [TestFixture]
    public class UnidadeCommandHandlerTest
    {
        private Mock<IUnidadeRepository> _repositoryMock;
        private Mock<UnidadeCommandHandler> _handlerMock;
        private Mock<IMapper> _mockMapper;
        private UnidadeCommandHandler _handler;

        public interface ICommandHandlerProxy
        {
            Task<ValidationResult> Commit<T>(ISQLBaseRepository<T> repository, string message = null) where T : class;
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUnidadeRepository>();
            _mockMapper = new Mock<IMapper>();
            //_mockCommandHandler = new Mock<CommandHandler>();

            // Criar um mock parcial do handler para interceptar o Commit protegido.
            _handlerMock = new Mock<UnidadeCommandHandler>(_repositoryMock.Object, _mockMapper.Object);

            _handlerMock.Protected()
                .As<ICommandHandlerProxy>()
                .Setup(m => m.Commit(It.IsAny<ISQLBaseRepository<Unidade>>(), It.IsAny<string>()))
                               .ReturnsAsync(new ValidationResult());

            _handler = _handlerMock.Object;
        }

        [Test]
        public async Task Handle_CreateCommand_Valid_ShouldReturnUnidadeAndAddEvent()
        {
            // Arrange
            var command = new UnidadeCreateCommand 
            { 
                Id = 1,
                IdEspaco = 1,
                Rede = "Unidade Teste",
                _ValidationResult = new ValidationResult()
            };
            var unidadeEntity = new Unidade
            {
                Id = command.Id,
                IdEspaco = command.IdEspaco,
                Rede = command.Rede
            };

            var notification = new UnidadeCreateNotification()
            {
                Id = command.Id,
                IdEspaco = command.IdEspaco,
                Rede = command.Rede
            };

            _mockMapper.Setup(m => m.Map<Unidade>(It.IsAny<UnidadeCreateCommand>()))
                       .Returns(unidadeEntity);

            _mockMapper.Setup(m => m.Map<UnidadeCreateNotification>(It.IsAny<Espaco>()))
                      .Returns(notification);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Unidade>()))
                .ReturnsAsync(unidadeEntity);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Unidade>()), Times.Once);
                Assert.That(result.DomainEvents, Is.Not.Empty, "Deveria ter adicionado um evento de domínio.");
            });
        }

        [Test]
        public async Task Handle_UpdateCommand_Valid_ShouldReturnUnidadeAndAddEvent()
        {
            // Arrange
            var command = new UnidadeUpdateCommand
            {
                Id = 1,
                IdEspaco = 1,
                Rede = "Unidade Teste",
                _ValidationResult = new ValidationResult()
            };
            var unidadeEntity = new Unidade
            {
                Id = command.Id,
                IdEspaco = command.IdEspaco,
                Rede = command.Rede
            };

            var notification = new UnidadeUpdateNotification()
            {
                Id = command.Id,
                IdEspaco = command.IdEspaco,
                Rede = command.Rede
            };

            _mockMapper.Setup(m => m.Map<Unidade>(It.IsAny<UnidadeUpdateCommand>()))
                       .Returns(unidadeEntity);

            _mockMapper.Setup(m => m.Map<UnidadeUpdateNotification>(It.IsAny<Espaco>()))
                      .Returns(notification);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Unidade>(), It.IsAny<int>()))
                .ReturnsAsync(unidadeEntity);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Unidade>(), It.IsAny<int>()), Times.Once);
                Assert.That(result.DomainEvents, Is.Not.Empty, "Deveria ter adicionado um evento de domínio.");
            });
        }

        [Test]
        public async Task Handle_DeleteCommand_ShouldExecuteRepositoryAndDelete()
        {
            // Arrange
            var unidadeId = 2;
            var command = new UnidadeDeleteCommand() { Id = unidadeId, _ValidationResult = new ValidationResult() };
            var unidadeFake = new Unidade   
            {
                Id = unidadeId,
                Rede = "teste 2",
                IdEspaco = 1,
                ValidationResult = new ValidationResult()
            };


            _repositoryMock.Setup(r => r.GetByIdAsync(unidadeId)).ReturnsAsync(unidadeFake);
            // Assert
            _repositoryMock.Setup(r => r.DeleteAsync(unidadeId)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.IsValid, Is.True);
        }

    }
}
