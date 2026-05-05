using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Domain
{
    public abstract class CommandHandler
    {
        protected ValidationResult ValidationResult;

        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        //public void Commit<T>(T @object)
        //{
        //    throw new NotImplementedException();
        //}

        protected void AddError(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        
        protected virtual async Task<ValidationResult> Commit<T>(ISQLBaseRepository<T> repository, string? message = null) where T : class
        {
            var hasChanges = await repository.SaveChangesAsync();

            if (!hasChanges)
            {
                AddError(message ?? $"Houve um erro ao persistir os dados de {typeof(T).Name}");
            }

            return ValidationResult;
        }
    }
}
