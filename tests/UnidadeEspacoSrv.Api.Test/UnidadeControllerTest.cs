using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Api.Controllers;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;

namespace UnidadeEspacoSrv.Api.Test
{
    [TestFixture]
    public class UnidadeControllerTest
    {
        private Mock<ILogger<UnidadeController>> _logger;
        private Mock<IUnidadeApp> _app;
        private UnidadeController _controller;
        private IFixture _fixture = new Fixture();
        private List<UnidadeViewModel> _unidadeListViewModel = new List<UnidadeViewModel>();
        private UnidadeViewModel _unidadeViewModel = new UnidadeViewModel();

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<UnidadeController>>();
            _app = new Mock<IUnidadeApp>();
            _controller = new UnidadeController(_logger.Object, _app.Object);
            _unidadeListViewModel = _fixture.Build<UnidadeViewModel>()
                .Without(x => x.Espaco)
                .CreateMany(3)
                .ToList();
            _unidadeViewModel = _fixture.Build<UnidadeViewModel>()
                .Without(x => x.Espaco)
                .Create();
        }

        [Test]
        public async Task Obter()
        {
            _app.Setup(x => x.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(_unidadeViewModel);
            var response = await _controller.Obter(1);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task ObterTodos()
        {
            _app.Setup(x => x.ObterTodasUnidadeComEspaco()).ReturnsAsync(_unidadeListViewModel);
            var response = await _controller.ObterTodos();
            Assert.IsNotNull(response);

        }

        [Test]
        public async Task Adicionar()
        {
            var request = _fixture.Create<UnidadeRequest>();
            _app.Setup(x => x.AdicionarAsync(request)).ReturnsAsync(_unidadeViewModel);
            var response = await _controller.Adicionar(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Atualizar()
        {
            var request = _fixture.Create<UnidadeRequest>();
            _app.Setup(x => x.AtualizarAsync(request)).ReturnsAsync(_unidadeViewModel);
            var response = await _controller.Atualizar(request);
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Excluir()
        {
            var validationResult = new FluentValidation.Results.ValidationResult();
            _app.Setup(x => x.ExcluirAsync(It.IsAny<int>())).ReturnsAsync(validationResult);
            var response = await _controller.Excluir(1);
            Assert.IsNotNull(response);
        }
    }
}
