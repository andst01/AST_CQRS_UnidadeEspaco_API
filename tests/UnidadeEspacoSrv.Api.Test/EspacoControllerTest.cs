using AutoFixture;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System.Runtime.CompilerServices;
using UnidadeEspacoSrv.Api.Controllers;
using UnidadeEspacoSrv.Application;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnidadeEspacoSrv.Api.Test
{
    [TestFixture]
    public class EspacoControllerTest
    {
        private Mock<ILogger<EspacoController>> _logger;
        private Mock<IEspacoApp> _app;
        private EspacoController _controller;
        private IFixture _fixture = new Fixture();
        private List<EspacoViewModel> _espacoListViewModel = new List<EspacoViewModel>();
        private EspacoViewModel _espacoViewModel = new EspacoViewModel();
        private IEspacoApp _espacoApp;
        private Mock<IMapper> _mapper;
        private Mock<IMediatorHandler> _handler;
        private Mock<IEspacoMongoDbRepository> _repository;

        /*
         * 
         * public EspacoApp(IMapper mapper, 
                        IMediatorHandler mediator, 
                        IEspacoMongoDbRepository repository) : base(mapper, mediator, repository, "Espaco")
         */

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<EspacoController>>();
            _app = new Mock<IEspacoApp>();
            _mapper = new Mock<IMapper>();
            _handler = new Mock<IMediatorHandler>();
            _repository = new Mock<IEspacoMongoDbRepository>();
            _espacoApp = new EspacoApp(_mapper.Object, _handler.Object, _repository.Object);
            _controller = new EspacoController(_logger.Object, _app.Object);

            _espacoListViewModel = _fixture.Build<EspacoViewModel>()
                .Without(x => x.Unidades)
                .CreateMany(3)
                .ToList();

            _espacoViewModel = _fixture.Build<EspacoViewModel>()
                .Without(x => x.Unidades)
                .Create();
        }

        [Test]
        public async Task Obter()
        {
            _app.Setup(x => x.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(_espacoViewModel);
            var response = await _controller.Obter(1);

            Assert.IsNotNull(response);

        }

        [Test]
        public async Task ObterTodos()
        {

            _app.Setup(x => x.ObterEspacosComUnidadesAsync()).ReturnsAsync(_espacoListViewModel);
            var response = await _controller.ObterTodos();
            Assert.IsNotNull(response);
        }



        [Test]
        public async Task Atualizar()
        {
            var request = _fixture.Create<EspacoRequest>();
            _app.Setup(x => x.AtualizarAsync(request)).ReturnsAsync(_espacoViewModel);
            var response = await _controller.Atualizar(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Adicionar()
        {
            var request = _fixture.Create<EspacoRequest>();
            _app.Setup(x => x.AdicionarAsync(request)).ReturnsAsync(_espacoViewModel);
            var response = await _controller.Adicionar(request);
            Assert.IsNotNull(response);
        }

        public async Task Adicionar_DeveRetornarStatusCodeEsperado()
        {
            // Arrange
            var request = _fixture.Create<EspacoRequest>();
            // Simulando um cenário de sucesso (ValidationResult sem erros)
            var validationResult = new FluentValidation.Results.ValidationResult();
            _app.Setup(x => x.AdicionarAsync(request)).ReturnsAsync(_espacoViewModel);
            // Act
            var result = await _controller.Adicionar(request);
            // Assert
            var objectResult = result as IStatusCodeActionResult;
            Assert.IsNotNull(objectResult, "O resultado não deve ser nulo");
            Assert.AreEqual(200, objectResult.StatusCode);
        }

        [Test]
        public async Task Adicionar_DeveRetornarBadRequestQuandoValidationResultInvalido()
        {
            // Arrange
            var mockController = new EspacoController(_logger.Object, _espacoApp);
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
          {
                new ValidationFailure("Nome", "O campo Nome é obrigatório.")
            });
            var request = _fixture.Build<EspacoRequest>()
                .Without(x => x.Nome) 
                .Create();
            //var _espaco = _fixture.Build<Espaco>()
            //    .Without(x => x.Unidades)
            //    .With(x => x.ValidationResult, validationResult)
            //    .Create();
            //var createCommand = _fixture.Build<EspacoCreateCommand>()
            //    .With(x => x._ValidationResult, validationResult)
            //    .Create();

            //_mapper.Setup(m => m.Map<EspacoCreateCommand>(request)).Returns(createCommand);
            //_handler.Setup(h => h.SendCommand<EspacoCreateCommand, Espaco>(createCommand)).ReturnsAsync(_espaco);
            //_mapper.Setup(a => a.Map<EspacoViewModel>(_espaco)).Returns(_espacoViewModel);
            //await _espacoApp.AdicionarAsync(request);

            _app.Setup(x => x.AdicionarAsync(request)).ReturnsAsync(_espacoViewModel);

            var result = await _controller.Adicionar(null);

            // Assert
            var objectResult = result as IStatusCodeActionResult;
            
            Assert.IsNotNull(objectResult, "O resultado não deve ser nulo");
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [Test]
        public async Task Adicionar_ErrorModelState()
        {
            // Arrange
            var request = _fixture.Build<EspacoRequest>()
                .Without(x => x.Nome) 
                .Create();

            _controller.ModelState.AddModelError("Nome", "O campo Nome é obrigatório.");
            // Act
            var result = await _controller.Adicionar(request);
            // Assert
            var objectResult = result as IStatusCodeActionResult;
            Assert.IsNotNull(objectResult, "O resultado não deve ser nulo");
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [Test]
        public async Task Excluir()
        {
            var validationResult = new FluentValidation.Results.ValidationResult();
            _app.Setup(x => x.ExcluirAsync(It.IsAny<int>())).ReturnsAsync(validationResult);
            var response = await _controller.Excluir(1);
            Assert.IsNotNull(response);
        }

        [TestCase(1, 200)] // Sucesso: ID > 0 gera ValidationResult sem erros
        [TestCase(0, 400)] // Erro: ID 0 gera ValidationResult com erros
        public async Task Excluir_DeveRetornarStatusCodeEsperado(int id, int statusCodeEsperado)
        {
            // Arrange
            FluentValidation.Results.ValidationResult validationResult;

            if (statusCodeEsperado == 200)
            {
                // Cria um resultado de validação sem erros (IsValid = true)
                validationResult = new FluentValidation.Results.ValidationResult();
            }
            else
            {
                // Cria um resultado de validação com um erro (IsValid = false)
                validationResult = new FluentValidation.Results.ValidationResult(new[]
                {
            new ValidationFailure("Id", "Id inválido")
        });
            }

            _app.Setup(x => x.ExcluirAsync(id))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Excluir(id);

            // Assert
            var objectResult = result as IStatusCodeActionResult;

            //Assert.IsNotNull(objectResult, "O resultado não deve ser nulo");
            Assert.AreEqual(statusCodeEsperado, objectResult.StatusCode);
        }

       
    }
}
