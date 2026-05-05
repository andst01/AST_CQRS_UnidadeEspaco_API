using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace UnidadeEspacoSrv.Domain
{
    public class BaseCommand<T> : IRequest<T>, IBaseRequest where T : class
    {
        public int Id { get; set; }
        public ValidationResult _ValidationResult { get; set; } = new ValidationResult();

        public virtual bool IsValid()
        {
            return _ValidationResult?.IsValid ?? false;
        }
    }
}
