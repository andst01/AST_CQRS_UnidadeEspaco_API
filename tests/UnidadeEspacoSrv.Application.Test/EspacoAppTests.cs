using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
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
    public class EspacoAppTests
    {
        // private IFixture _fixture;
        private Mock<IMapper> _mapperMock;
        private Mock<IMediatorHandler> _mediatorMock;
        private Mock<IEspacoMongoDbRepository> _repositoryMock;
        private EspacoApp _espacoApp;
        private EspacoReadModel _espacoReadModel = new EspacoReadModel();
        private List<UnidadeReadModel> _unidadeReadModels = new List<UnidadeReadModel>();
        private EspacoViewModel _espacoViewModel = new EspacoViewModel();
        private List<UnidadeViewModel> _unidadeViewModels = new List<UnidadeViewModel>();
        private IFixture _fixture = new Fixture();
        private List<EspacoReadModel> _espacoReadModels = new List<EspacoReadModel>();
        private List<EspacoViewModel> _espacoViewModels = new List<EspacoViewModel>();


        [SetUp]
        public void Setup()
        {
            // Setup do Fixture com AutoMoq
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediatorHandler>>();
            _repositoryMock = _fixture.Freeze<Mock<IEspacoMongoDbRepository>>();

            _unidadeReadModels = _fixture.Build<UnidadeReadModel>()
                .Without(a => a._Id)
                .Without(a => a.Espaco)
                .CreateMany(3)
                .ToList();

            _espacoReadModel = _fixture.Build<EspacoReadModel>()
                .With(e => e.Unidades, _unidadeReadModels)
                .Without(a => a._Id)
                .Create();

            _unidadeViewModels = _fixture.Build<UnidadeViewModel>()
                .Without(a => a.Espaco)
                .CreateMany(3)
                .ToList();

            _espacoViewModels = _fixture.Build<EspacoViewModel>()
                .With(a => a.Unidades, _unidadeViewModels)
                .CreateMany(3)
                .ToList();

            _espacoReadModels = _fixture.Build<EspacoReadModel>()
                .With(a => a.Unidades, _unidadeReadModels)
                .Without(a => a._Id)
                .CreateMany(3)
                .ToList();

            _espacoViewModel = _fixture.Build<EspacoViewModel>()
                 .With(a => a.Unidades, _unidadeViewModels)
                 .Create();
                


            // Instanciação da classe alvo
            _espacoApp = new EspacoApp(
                _mapperMock.Object,
                _mediatorMock.Object,
                _repositoryMock.Object);
        }

        [Test]
        public async Task ObterEspacosComUnidadesAsync_DeveRetornarListaDeViewModels_QuandoExistiremDados()
        {
            // Arrange
            // Criamos dados fictícios para a entidade e para o retorno esperado (ViewModel)
            //var espacosDomain = _fixture.Build<EspacoReadModel>()
            //    .Without(e => e.Unidades)
            //    .Without(a => a._Id)
            //    .CreateMany(3)
            //    .ToList();

            //var espacosViewModel = _fixture.Build<EspacoViewModel>()
            //    .Without(a => a.Unidades)
            //    .CreateMany(3)
            //    .ToList();

            _repositoryMock
                .Setup(r => r.ObterEspacosComUnidadesAsync())
                .ReturnsAsync(_espacoReadModels);

            _mapperMock
                .Setup(m => m.Map<List<EspacoViewModel>>(_espacoReadModels))
                .Returns(_espacoViewModels);

            // Act
            var result = await _espacoApp.ObterEspacosComUnidadesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_espacoViewModels);

            _repositoryMock.Verify(r => r.ObterEspacosComUnidadesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<EspacoViewModel>>(_espacoReadModels), Times.Once);
        }

        [Test]
        public async Task AdicionarAsync_DeveExecutarLógicaDaBaseCorretamente()
        {
            // Este teste valida se a herança de AppBase está funcionando para Espaco
            // Arrange
            var request = _fixture.Create<EspacoRequest>();
            var command = _fixture.Create<EspacoCreateCommand>();
            var entity = _fixture.Build<Espaco>()
                .Without(a => a.Unidades)
                .Create();
            var viewModel = _espacoViewModel;

            // Simulando entidade válida para disparar o PublishEvent da base
            entity.ValidationResult = new FluentValidation.Results.ValidationResult();

            _mapperMock.Setup(m => m.Map<EspacoCreateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.SendCommand<EspacoCreateCommand, Espaco>(command)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<EspacoViewModel>(entity)).Returns(viewModel);

            // Act
            var result = await _espacoApp.AdicionarAsync(request);

            // Assert
            result.Should().Be(viewModel);
           // _mediatorMock.Verify(m => m.PublishEvent(), Times.Once);
        }

        [Test]
        public async Task ObterEspacosComUnidadesAsync_DeveRetornarListaVazia_QuandoRepositoryRetornarVazio()
        {
            // Arrange
            var espacosVazios = new List<EspacoReadModel>();
            _repositoryMock.Setup(r => r.ObterEspacosComUnidadesAsync()).ReturnsAsync(espacosVazios);
            _mapperMock.Setup(m => m.Map<List<EspacoViewModel>>(espacosVazios)).Returns(new List<EspacoViewModel>());

            // Act
            var result = await _espacoApp.ObterEspacosComUnidadesAsync();

            // Assert
            result.Should().BeEmpty();
        }

    }
}
