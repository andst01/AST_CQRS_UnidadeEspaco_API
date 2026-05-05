using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application.Test
{
    public class AppBaseTestsUnidade
    {
        private IFixture _fixture;
        private Mock<IMapper> _mapperMock;
        private Mock<IMediatorHandler> _mediatorMock;
        private Mock<IMongoDbBaseRepository<UnidadeNotification>> _repositoryMock;
        private string _collectionName;
        private AppBase<Unidade,
                        UnidadeRequest,
                        UnidadeViewModel,
                        UnidadeCreateCommand,
                        UnidadeUpdateCommand,
                        UnidadeDeleteCommand,
                        UnidadeNotification> _appService;

        [SetUp]
        public void Setup()
        {
            // Configura o Fixture para trabalhar com Moq automaticamente
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediatorHandler>>();
            _repositoryMock = _fixture.Freeze<Mock<IMongoDbBaseRepository<UnidadeNotification>>>();
            _collectionName = _fixture.Create<string>();

            _appService = new AppBase<Unidade,
                        UnidadeRequest,
                        UnidadeViewModel,
                        UnidadeCreateCommand,
                        UnidadeUpdateCommand,
                        UnidadeDeleteCommand,
                        UnidadeNotification>(
                _mapperMock.Object,
                _mediatorMock.Object,
                _repositoryMock.Object,
                _collectionName
            );
        }

        [Test]
        public async Task AdicionarAsync_QuandoSucesso_DeveMapearEPublicarEvento()
        {
            // Arrange
            var request = _fixture.Create<UnidadeRequest>();
            var command = _fixture.Create<UnidadeCreateCommand>();
            var entity = new Unidade()
            {
                Id = _fixture.Create<int>(),
                IdEspaco = _fixture.Create<int>(),
                Rede = _fixture.Create<string>(),
                ValidationResult = new ValidationResult()
            };
            var viewModel = new UnidadeViewModel()
            {
                Id = _fixture.Create<int>(),
                IdEspaco = _fixture.Create<int>(),
                Rede = _fixture.Create<string>(),
                // ValidationResult = new ValidationResult()
            };


            // Forçamos o resultado ser válido para entrar no IF do PublishEvent
            entity.ValidationResult = new ValidationResult();

            _mapperMock.Setup(m => m.Map<UnidadeCreateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.SendCommand<UnidadeCreateCommand, Unidade>(command)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<UnidadeViewModel>(entity)).Returns(viewModel);

            // Act
            var result = await _appService.AdicionarAsync(request);

            // Assert
            result.Should().BeEquivalentTo(viewModel);
            _mediatorMock.Verify(x => x.PublishEvent(), Times.Once);
            _mediatorMock.Verify(x => x.SendCommand<UnidadeCreateCommand, Unidade>(command), Times.Once);
        }


        [Test]
        public async Task ExcluirAsync_DeveConfigurarIdNoComandoERetornarResultado()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var validationResult = new ValidationResult(); // IsValid = true

            _mediatorMock.Setup(m => m.SendCommand(It.IsAny<UnidadeDeleteCommand>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _appService.ExcluirAsync(id);

            // Assert
            _mediatorMock.Verify(m => m.SendCommand(It.Is<UnidadeDeleteCommand>(c => c.Id == id)), Times.Once);
            _mediatorMock.Verify(m => m.PublishEvent(), Times.Once);
            result.Should().Be(validationResult);
        }
    }
}
