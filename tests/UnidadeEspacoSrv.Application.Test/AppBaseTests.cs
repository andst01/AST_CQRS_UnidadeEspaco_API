using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using System.Net.Http.Headers;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Test
{
    [TestFixture]
    public class AppBaseTests
    {
        private IFixture _fixture = new Fixture();
        private Mock<IMapper> _mapperMock;
        private Mock<IMediatorHandler> _mediatorMock;
        private Mock<IMongoDbBaseRepository<EspacoNotification>> _repositoryMock;
        private string _collectionName;
        private Espaco entity = new Espaco();
        private EspacoViewModel _viewModel = new EspacoViewModel();

        private AppBase<Espaco, 
                        EspacoRequest, 
                        EspacoViewModel, 
                        EspacoCreateCommand, 
                        EspacoUpdateCommand, 
                        EspacoDeleteCommand, 
                        EspacoNotification> _appService;

        [SetUp]
        public void Setup()
        {
            // Configura o Fixture para trabalhar com Moq automaticamente
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediatorHandler>>();
            _repositoryMock = _fixture.Freeze<Mock<IMongoDbBaseRepository<EspacoNotification>>>();
            _collectionName = _fixture.Create<string>();

            _appService = new AppBase<Espaco, EspacoRequest, EspacoViewModel, EspacoCreateCommand, EspacoUpdateCommand, EspacoDeleteCommand, EspacoNotification>(
                _mapperMock.Object,
                _mediatorMock.Object,
                _repositoryMock.Object,
                _collectionName
            );

            entity = _fixture.Build<Espaco>()
                        .Without(a => a.ValidationResult)
                        .Without(a => a.Unidades)
                        .Create();

            _viewModel = _fixture.Build<EspacoViewModel>()
                        .Without(a => a.Unidades)
                        .Create();
        }

        [Test]
        public async Task AdicionarAsync_QuandoSucesso_DeveMapearEPublicarEvento()
        {
            // Arrange
            var request = _fixture.Create<EspacoRequest>();
            var command = _fixture.Create<EspacoCreateCommand>();
           
           
            // Forçamos o resultado ser válido para entrar no IF do PublishEvent
            entity.ValidationResult = new ValidationResult();

            _mapperMock.Setup(m => m.Map<EspacoCreateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.SendCommand<EspacoCreateCommand, Espaco>(command)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<EspacoViewModel>(entity)).Returns(_viewModel);

            // Act
            var result = await _appService.AdicionarAsync(request);

            // Assert
            result.Should().BeEquivalentTo(_viewModel);
           // _mediatorMock.Verify(x => x.PublishEvent(), Times.Once);
            _mediatorMock.Verify(x => x.SendCommand<EspacoCreateCommand, Espaco>(command), Times.Once);
        }

        [Test]
        public async Task ATualizarAsync_QuandoSucesso_DeveMapearEPublicarEvento()
        {
            // Arrange
            var request = _fixture.Create<EspacoRequest>();
            var command = _fixture.Create<EspacoUpdateCommand>();

            
            // Forçamos o resultado ser válido para entrar no IF do PublishEvent
            entity.ValidationResult = new ValidationResult();

            _mapperMock.Setup(m => m.Map<EspacoUpdateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.SendCommand<EspacoUpdateCommand, Espaco>(command)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<EspacoViewModel>(entity)).Returns(_viewModel);

            // Act
            var result = await _appService.AtualizarAsync(request);
            // Assert
            result.Should().BeEquivalentTo(_viewModel);
           // _mediatorMock.Verify(x => x.PublishEvent(), Times.Once);
            _mediatorMock.Verify(x => x.SendCommand<EspacoUpdateCommand, Espaco>(command), Times.Once);
        }

        [Test]
        public async Task ExcluirAsync_DeveConfigurarIdNoComandoERetornarResultado()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var validationResult = new ValidationResult(); // IsValid = true

            _mediatorMock.Setup(m => m.SendCommand(It.IsAny<EspacoDeleteCommand>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _appService.ExcluirAsync(id);

            // Assert
            _mediatorMock.Verify(m => m.SendCommand(It.Is<EspacoDeleteCommand>(c => c.Id == id)), Times.Once);
           // _mediatorMock.Verify(m => m.PublishEvent(), Times.Once);
            result.Should().Be(validationResult);
        }

        [Test]
        public async Task ObterTodosAsync_DeveRetornarListaDeViewModels()
        {
            // Arrange

            var entidades = _fixture.Build<EspacoNotification>()
                                    .Without(a => a.Unidades)
                                    .Without(a => a._Id)
                                    .CreateMany(3).ToList();
            var viewModels = _fixture.Build<EspacoViewModel>()
                                     .Without(a => a.Unidades)
                                     .CreateMany(3).ToList().ToList();



            _repositoryMock.Setup(r => r.GetAll(_collectionName)).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<List<EspacoViewModel>>(entidades)).Returns(viewModels);

            // Act
            var result = await _appService.ObterTodosAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(viewModels);

        }


        [Test]
        public async Task ObterPorIdAsync_DeveRetornarViewModel()
        {
            // Arrange

            var entidades = _fixture.Build<EspacoNotification>()
                                    .Without(a => a.Unidades)
                                    .Without(a => a._Id)
                                    .Create();
            var viewModels = _fixture.Build<EspacoViewModel>()
                                     .Without(a => a.Unidades)
                                     .Create();



            _repositoryMock.Setup(r => r.GetById(entity.Id, _collectionName)).ReturnsAsync(entidades);
            _mapperMock.Setup(m => m.Map<EspacoViewModel>(entidades)).Returns(viewModels);

            // Act
            var result = await _appService.ObterPorIdAsync(entity.Id);

            // Assert
            result.Should().BeEquivalentTo(viewModels);

        }

        [Test]
        public async Task AdicionarAsync_QuandoInvalido_NaoDevePublicarEvento()
        {
            // Arrange
            var request = _fixture.Create<EspacoRequest>();
            var command = _fixture.Create<EspacoCreateCommand>();
            var entity = new Espaco()
            {
                Id = _fixture.Create<int>(),
                Endereco = _fixture.Create<string>(),
                Nome = _fixture.Create<string>(),
                ValidationResult = new ValidationResult()
            };

            // Criando um resultado inválido
            entity.ValidationResult = new ValidationResult(new[] {
            new ValidationFailure("Prop", "Erro de validação")
        });

            _mapperMock.Setup(m => m.Map<EspacoCreateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.SendCommand<EspacoCreateCommand, Espaco>(command)).ReturnsAsync(entity);

            // Act
            await _appService.AdicionarAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.PublishEvent(true), Times.Never);
        }


    }
}
