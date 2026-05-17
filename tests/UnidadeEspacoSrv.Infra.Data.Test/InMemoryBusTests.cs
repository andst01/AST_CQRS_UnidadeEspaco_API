using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Data;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;


namespace UnidadeEspacoSrv.Data.Test
{
    [TestFixture]
    public class InMemoryBusTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<SQLDbContext> _sqlDbContextMock;
        private SQLDbContext _context;
        private InMemoryBus _inMemoryBus;
        private IFixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _sqlDbContextMock = new Mock<SQLDbContext>();

            // Configura um DbContext em memória
            var options = new DbContextOptionsBuilder<SQLDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SQLDbContext(options);

            _inMemoryBus = new InMemoryBus(_mediatorMock.Object, _context);
        }


        [Test]
        public async Task PublishEvent_ShouldCallMediatorPublish_AndReturnNull()
        {
            // Arrange
            var myEvent = _fixture.Build<EspacoCreateNotification>()
                .Without(a => a.Unidades)
                .Without(a => a._Id)
                .Create();

            // Act
            var result = await _inMemoryBus.PublishEvent(myEvent);

            // Assert
            // Verifica se o Mediator.Publish foi chamado exatamente uma vez com o evento correto
            _mediatorMock.Verify(x => x.Publish(
                It.Is<EspacoCreateNotification>(e => e == myEvent),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Verifica se o retorno é nulo (conforme definido no seu código atual)
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task PublishEvent_DevePublicarELimparEventosDasEntidadesNoContexto(bool hasCommit)
        {

            // Arrange
            var entidade = _fixture.Build<Espaco>()
                                .Without(x => x.Unidades)
                                .Without(x => x.ValidationResult)
                                .Create();
            var entidade2 = _fixture.Build<HistoricoEvento>()
                                .With(x => x.Id, 0)
                               // .Without(x => x.Unidades)
                                .Without(x => x.ValidationResult)
                                .Create();
            var evento1 = new EspacoCreateNotification();
            //var evento2 = new 
             
            entidade.AddDomainEvent(evento1);

            _context.Add(entidade);
          
            // Act
            try
            {
                await _inMemoryBus.PublishEvent(hasCommit);
            }
            catch (Exception ex)
            {

                // Isso vai imprimir o erro real no console do NUnit
                Console.WriteLine($"MENSAGEM: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                Console.WriteLine($"STACK: {ex.StackTrace}");
                throw;
            }
            

            
            _mediatorMock.Verify(m => m.Publish(It.IsAny<EventBase>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            entidade.DomainEvents.Should().BeEmpty();


        }

        
        [Test]
        public async Task SendCommand_DeveChamarMediatorERetornarValidationResult()
        {
            // Arrange
            var comando = new EspacoDeleteCommand();
            var expectedResult = new ValidationResult();

            _mediatorMock.Setup(m => m.Send(comando, default))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _inMemoryBus.SendCommand(comando);

            // Assert
            result.Should().Be(expectedResult);
            _mediatorMock.Verify(m => m.Send(comando, default), Times.Once);
        }


        [Test]
        public async Task SendCommand_DeveChamarMediatorERetornarActionCommand()
        {
            // Arrange
            var comando = new EspacoCreateCommand();
            var expectedResult = new Espaco();

            _mediatorMock.Setup(m => m.Send(comando, default))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _inMemoryBus.SendCommand<EspacoCreateCommand, Espaco>(comando);

            // Assert
            result.Should().Be(expectedResult);
            _mediatorMock.Verify(m => m.Send(comando, default), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
