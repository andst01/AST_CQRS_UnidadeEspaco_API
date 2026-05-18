using FluentValidation;
using FluentValidation.Results;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class EspacoCommand : BaseCommand<Espaco>
    {
       // public int Id { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }

        protected static T MapFrom<T>(EspacoRequest request) where T : EspacoCommand, new()
        {
            var mapped = new T
            {
                Id = request.Id,
                Nome = request.Nome,
                Endereco = request.Endereco
            };
            return mapped;
        }

        public static implicit operator Espaco(EspacoCommand command)
        {
            if (command == null) return null;
            return new Espaco
            {
                Id = command.Id,
                Nome = command.Nome,
                Endereco = command.Endereco
            };
        }

    }


    public class EspacoCreateCommand : EspacoCommand, IMappableFrom<EspacoRequest>
    {
        public void MapFrom(EspacoRequest request)
        {
            this.Id = request.Id;
            this.Nome = request.Nome;
            this.Endereco = request.Endereco;
        }

        //public static explicit operator EspacoCreateCommand(EspacoRequest request)
        //{
        //   return  MapFrom<EspacoCreateCommand>(request);
        //}
    }
    public class EspacoUpdateCommand : EspacoCommand, IMappableFrom<EspacoRequest>
    {
        public void MapFrom(EspacoRequest request)
        {
            this.Id = request.Id;
            this.Nome = request.Nome;
            this.Endereco = request.Endereco;
        }

        //public static explicit operator EspacoUpdateCommand(EspacoRequest request)
        //{
        //    return MapFrom<EspacoUpdateCommand>(request);
        //}
    }

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
