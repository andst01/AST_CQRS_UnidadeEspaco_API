using FluentValidation;
using FluentValidation.Results;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class EspacoCommand : BaseCommand<Espaco>
    {
       // public int Id { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }
    }


    public class EspacoCreateCommand : EspacoCommand { }
    public class EspacoUpdateCommand : EspacoCommand { }

    public class EspacoDeleteCommand : BaseCommand<ValidationResult>
    {
      
    }


    // Validador base para compartilhar regras de Nome e Endereco
    public abstract class EspacoValidator<T> : AbstractValidator<T> where T : EspacoCommand
    {
        protected void ValidateNome() =>
            RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do espaço é obrigatório.");

        protected void ValidateEndereco() =>
            RuleFor(x => x.Endereco).NotEmpty().WithMessage("O endereço do espaço é obrigatório.");
    }

    public class EspacoCreateCommandValidator : EspacoValidator<EspacoCreateCommand>
    {
        public EspacoCreateCommandValidator()
        {
           ValidateNome();
           ValidateEndereco();
        }
    }

    public class EspacoUpdateCommandValidator : EspacoValidator<EspacoUpdateCommand>
    {
        public EspacoUpdateCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID do espaço deve ser maior que zero.");
            ValidateNome();
            ValidateEndereco();
        }
    }

    public class EspacoDeleteCommandValidator : AbstractValidator<EspacoDeleteCommand>
    {
        public EspacoDeleteCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID do espaço deve ser maior que zero.");
        }
    }

}
