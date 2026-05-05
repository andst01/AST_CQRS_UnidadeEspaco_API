using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Test
{
    [TestFixture]
    public class UnidadeAppTests
    {
        private IFixture _fixture = new Fixture();
        private Mock<IMapper> _mapperMock;
        private Mock<IMediatorHandler> _mediatorMock;
        private Mock<IUnidadeMongoDbRepository> _repositoryMock;
        private UnidadeApp _unidadeApp;
        private EspacoReadModel _espacoReadModel = new EspacoReadModel();
        private EspacoViewModel _espacoViewModel = new EspacoViewModel();
        private List<UnidadeReadModel> _unidadeListReadModel = new List<UnidadeReadModel>();
        private List<UnidadeViewModel> _unidadeListViewModel = new List<UnidadeViewModel>();
        private UnidadeReadModel _unidadeReadModel = new UnidadeReadModel();
        private UnidadeViewModel _unidadeViewModel = new UnidadeViewModel();


        [SetUp]
        public void Setup()
        {
            // Setup do Fixture com AutoMoq
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediatorHandler>>();
            _repositoryMock = _fixture.Freeze<Mock<IUnidadeMongoDbRepository>>();

            // Instanciação da classe alvo
            _unidadeApp = new UnidadeApp(
                _mapperMock.Object,
                _mediatorMock.Object,
                _repositoryMock.Object);

            _espacoReadModel = _fixture.Build<EspacoReadModel>()
                .Without(a => a._Id)
                .Without(a => a.Unidades)
                .Create();

            _espacoViewModel = _fixture.Build<EspacoViewModel>()
                .Without(a => a.Unidades)
                .Create();

            _unidadeListReadModel = _fixture.Build<UnidadeReadModel>()
                .Without(a => a._Id)
                .With(a => a.Espaco, _espacoReadModel)
                .CreateMany(3)
                .ToList();

            _unidadeListViewModel = _fixture.Build<UnidadeViewModel>()
                .With(a => a.Espaco, _espacoViewModel)
                .CreateMany(3)
                .ToList();

            _unidadeReadModel = _fixture.Build<UnidadeReadModel>()
               .With(e => e.Espaco, _espacoReadModel)
               .Without(a => a._Id)
               .Create();

            _unidadeViewModel = _fixture.Build<UnidadeViewModel>()
               .With(e => e.Espaco, _espacoViewModel)
               .Create();
        }

        [Test]
        public async Task ObterTodasUnidadeComEspacoAsync_DeveRetornarListaDeViewModels_QuandoExistiremDados()
        {
            // Arrange
            // Criamos dados fictícios para a entidade e para o retorno esperado (ViewModel)
            //var unidadesDomain = _fixture.Build<UnidadeReadModel>()
            //    .Without(e => e.Espaco)
            //    .Without(a => a._Id)
            //    .CreateMany(3)
            //    .ToList();

            //var unidadesViewModel = _fixture.Build<UnidadeViewModel>()
            //    .Without(a => a.Espaco)
            //    .CreateMany(3)
            //    .ToList();

            _repositoryMock
                .Setup(r => r.ObterTodasUnidadeComEspaco())
                .ReturnsAsync(_unidadeListReadModel);

            _mapperMock
                .Setup(m => m.Map<List<UnidadeViewModel>>(_unidadeListReadModel))
                .Returns(_unidadeListViewModel);

            // Act
            var result = await _unidadeApp.ObterTodasUnidadeComEspaco();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_unidadeListViewModel);

            _repositoryMock.Verify(r => r.ObterTodasUnidadeComEspaco(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<UnidadeViewModel>>(_unidadeListReadModel), Times.Once);
        }



        [Test]
        public async Task ObterUnidadeComEspacoPorIdAsync_DeveRetornarListaDeViewModels_QuandoExistiremDados()
        {
            // Arrange
            // Criamos dados fictícios para a entidade e para o retorno esperado (ViewModel)
            //var unidadesDomain = _fixture.Build<UnidadeReadModel>()
            //    .Without(e => e.Espaco)
            //    .Without(a => a._Id)
            //    .Create();


            //var unidadesViewModel = _fixture.Build<UnidadeViewModel>()
            //    .Without(a => a.Espaco)
            //    .Create();
                

            _repositoryMock
                .Setup(r => r.ObterUnidadeComEspacoPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_unidadeReadModel);

            _mapperMock
                .Setup(m => m.Map<UnidadeViewModel>(_unidadeReadModel))
                .Returns(_unidadeViewModel);

            // Act
            var result = await _unidadeApp.ObterUnidadeComEspacoPorIdAsync(It.IsAny<int>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(_unidadeViewModel);

            _repositoryMock.Verify(r => r.ObterUnidadeComEspacoPorIdAsync(It.IsAny<int>()), Times.Once);
            _mapperMock.Verify(m => m.Map<UnidadeViewModel>(_unidadeReadModel), Times.Once);
        }


    }
}
