using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Application
{
    [ExcludeFromCodeCoverage]
    public class ValidationBehavior<TRequest, TResponse> : 
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        // Registra todos os validadores do FluentValidation que estão no assembly da Application
        // builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Registra o Behavior no MediatR
        // builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Executa todas as validações encontradas para este comando
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    // Se o seu TResponse for do tipo ValidationResult, retornamos os erros sem lançar Exception
                    if (typeof(TResponse) == typeof(ValidationResult))
                    {
                        var result = new ValidationResult();
                        failures.ForEach(f => result.Errors.Add(new ValidationFailure(f.PropertyName, f.ErrorMessage)));
                        return (TResponse)(object)result;
                    }

                    // Caso contrário, você pode lançar uma exception ou tratar conforme seu padrão
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }

        //public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
