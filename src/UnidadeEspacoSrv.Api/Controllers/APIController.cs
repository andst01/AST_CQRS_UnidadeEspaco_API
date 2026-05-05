using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UnidadeEspacoSrv.Api.Controllers
{
    public class APIController : ControllerBase
    {
        private readonly ICollection<string> _errors = new List<string>();

        protected IActionResult ProcessResponse(object result = null)
        {
            
            if (IsOperationValid())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _errors.ToArray()
            });
        }

        protected IActionResult ProcessResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddError(error.ErrorMessage);
            }

            return ProcessResponse();
        }

        protected IActionResult ProcessResponse(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                AddError(error.ErrorMessage);
            }

            return ProcessResponse();
        }

        protected bool IsOperationValid()
        {
            return !_errors.Any();
        }

        protected virtual void AddError(string erro)
        {
            _errors.Add(erro);
        }

        protected void ClearErrors()
        {
            _errors.Clear();
        }
    }
}
