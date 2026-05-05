using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EspacoController : APIController
    {
        private readonly ILogger<EspacoController> _logger;
        private readonly IEspacoApp _app;

        public EspacoController(ILogger<EspacoController> logger,
                                IEspacoApp app)
        {
            _logger = logger;
            _app = app;
        }

        [HttpGet]
        [Route("Obter/{id}")]
        [ProducesResponseType(typeof(EspacoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obter(int id)
        {
           
            var response = await _app.ObterPorIdAsync(id);
            return ProcessResponse(response);
        }


        [HttpGet]
        [Route("ObterTodos")]
        [ProducesResponseType(typeof(IEnumerable<EspacoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterTodos()
        {
            var response = await _app.ObterEspacosComUnidadesAsync();
            return ProcessResponse(response);
        }

        [HttpPost]
        [Route("Adicionar")]
        [ProducesResponseType(typeof(EspacoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Adicionar([FromBody] EspacoRequest request)
        {
            if (request == null)
            {
                AddError("O request não pode ser nulo.");
                return ProcessResponse(); 
            }

            if(!ModelState.IsValid)
                return ProcessResponse(ModelState); 
            

            var response = await _app.AdicionarAsync(request);
            return ProcessResponse(response);
        }

        [HttpPut]
        [Route("Atualizar")]
        [ProducesResponseType(typeof(EspacoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar([FromBody] EspacoRequest viewModel)
        {
            var response = await _app.AtualizarAsync(viewModel);
            return ProcessResponse(response);
        }

        [HttpDelete]
        [Route("Excluir/{id}")]
        [ProducesResponseType(typeof(EspacoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            var response = await _app.ExcluirAsync(id);
            return ProcessResponse(response);

        }
    }
}
