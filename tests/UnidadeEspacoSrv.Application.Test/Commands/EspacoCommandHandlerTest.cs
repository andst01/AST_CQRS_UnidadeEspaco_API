using AutoMapper;
using Moq;
using Moq.Protected;
using FluentValidation.Results;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;
using UnidadeEspacoSrv.Domain;

namespace UnidadeEspacoSrv.Application.Test.Commands
{
    [TestFixture]
    public class EspacoCommandHandlerTest
    {
        private Mock<IEspacoRepository> _repositoryMock;
        private Mock<EspacoCommandHandler> _handlerMock;
        private Mock<EspacoCreateCommand> _createCommandMock;
        private Mock<CommandHandler> _mockCommandHandler;
        private Mock<IMapper> _mockMapper;
        private EspacoCommandHandler _handler;

        public interface ICommandHandlerProxy
        {
            Task<ValidationResult> Commit<T>(ISQLBaseRepository<T> repository, string message = null) where T : class;
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IEspacoRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCommandHandler = new Mock<CommandHandler>();
            
            // Criar um mock parcial do handler para interceptar o Commit protegido.
            _handlerMock = new Mock<EspacoCommandHandler>(_repositoryMock.Object, _mockMapper.Object);

            _handlerMock.Protected()
                .As<ICommandHandlerProxy>()
                .Setup(m => m.Commit(It.IsAny<ISQLBaseRepository<Espaco>>(), It.IsAny<string>()))
                               .ReturnsAsync(new ValidationResult());
            
            _handler = _handlerMock.Object;
        }

        [Test]
        public async Task Handle_CreateCommand_Valid_ShouldReturnEspacoAndAddEvent()
        {
            // Arrange
            var command = new EspacoCreateCommand
            {
                // Importante: Preencha os campos para o request.IsValid() ser true
                Nome = "Teste",
                _ValidationResult = new ValidationResult() ,
                // Simula um resultado de validação válido
            };

            var espacoEntity = new Espaco { Id = 2, Nome = "Teste", Endereco = "Endereco Teste", ValidationResult = new ValidationResult() };
            var notification = new EspacoCreateNotification();

            // Setup do Mapper: Fundamental para não retornar null
            _mockMapper.Setup(m => m.Map<Espaco>(It.IsAny<EspacoCreateCommand>()))
                       .Returns(espacoEntity);

            _mockMapper.Setup(m => m.Map<EspacoCreateNotification>(It.IsAny<Espaco>()))
                       .Returns(notification);

            // Setup do Repositório
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Espaco>()))
                           .ReturnsAsync(espacoEntity);

           
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null, "O resultado não deveria ser nulo.");
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Espaco>()), Times.Once);
                Assert.That(result.DomainEvents, Is.Not.Empty, "Deveria ter adicionado um evento de domínio.");
            });
        }

        [Test]
        public async Task Handle_UpdateCommand_Valid_ShouldReturnEspacoAndAddEvent()
        {
            // Arrange
            var command = new EspacoUpdateCommand
            {
                // Importante: Preencha os campos para o request.IsValid() ser true
                Nome = "Teste",
                _ValidationResult = new ValidationResult(),
                // Simula um resultado de validação válido
            };

            var espacoEntity = new Espaco { Id = 2, Nome = "Teste", Endereco = "Endereco Teste", ValidationResult = new ValidationResult() };
            var notification = new EspacoUpdateNotification();

            // Setup do Mapper: Fundamental para não retornar null
            _mockMapper.Setup(m => m.Map<Espaco>(It.IsAny<EspacoUpdateCommand>()))
                       .Returns(espacoEntity);

            _mockMapper.Setup(m => m.Map<EspacoUpdateNotification>(It.IsAny<Espaco>()))
                       .Returns(notification);

            // Setup do Repositório
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Espaco>(), It.IsAny<int>()))
                           .ReturnsAsync(espacoEntity);


            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null, "O resultado não deveria ser nulo.");
                _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Espaco>(), It.IsAny<int>()), Times.Once);
                Assert.That(result.DomainEvents, Is.Not.Empty, "Deveria ter adicionado um evento de domínio.");
            });
        }

        [Test]
        public async Task Handle_CreateCommand_Invalid_ShouldReturnNull()
        {
            // Arrange
            var commandMock = new Mock<EspacoCreateCommand>();
            commandMock.Setup(c => c.IsValid()).Returns(false);

            // Act
            var result = await _handler.Handle(commandMock.Object, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Espaco>()), Times.Never);
        }

        [Test]
        public async Task Handle_DeleteCommand_ShouldExecuteRepositoryAndDelete()
        {
            // Arrange
            var espacoId = 2;
            var command = new EspacoDeleteCommand() { Id = espacoId, _ValidationResult =new ValidationResult() };
            var espacoFake = new Espaco { 
                Id = espacoId, Nome = "teste 2", 
                Endereco = "Endereço teste", 
                ValidationResult = new ValidationResult() };


            _repositoryMock.Setup(r => r.GetByIdAsync(espacoId)).ReturnsAsync(espacoFake);
            // Assert
            _repositoryMock.Setup(r => r.DeleteAsync(espacoId)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
           
            Assert.That(result.IsValid, Is.True);
        }


    }
}
