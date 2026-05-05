using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;

namespace UnidadeEspacoSrv.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadeController : APIController
    {
        private readonly ILogger<UnidadeController> _logger;
        private readonly IUnidadeApp _app;
        public UnidadeController(ILogger<UnidadeController> logger,
                                IUnidadeApp app)
        {
            _logger = logger;
            _app = app;
        }

        [HttpGet]
        [Route("Obter/{id}")]
        [ProducesResponseType(typeof(UnidadeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obter(int id)
        {
            var response = await _app.ObterUnidadeComEspacoPorIdAsync(id);
            return ProcessResponse(response);
        }

        [HttpGet]
        [Route("ObterTodos")]
        [ProducesResponseType(typeof(IEnumerable<UnidadeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterTodos()
        {
            var response = await _app.ObterTodasUnidadeComEspaco();
            return ProcessResponse(response);
        }

        [HttpPost]
        [Route("Adicionar")]
        [ProducesResponseType(typeof(UnidadeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Adicionar([FromBody] UnidadeRequest request)
        {
            var response = await _app.AdicionarAsync(request);
            return ProcessResponse(response);
        }

        [HttpPut]
        [Route("Atualizar")]
        [ProducesResponseType(typeof(UnidadeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar([FromBody] UnidadeRequest request)
        {
            var response = await _app.AtualizarAsync(request);
            return ProcessResponse(response);
        }

        [HttpDelete]
        [Route("Excluir/{id}")]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Excluir(int id)
        {
            var response = await _app.ExcluirAsync(id);
            return ProcessResponse(response);
        }
    }
}
