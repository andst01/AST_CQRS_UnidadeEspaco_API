using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Application.Commnds
{
    public class UnidadeCommand : BaseCommand<Unidade>
    {
        public int Id { get; set; }

        public int IdEspaco { get; set; }

        public string Rede { get; set; }
    }

    public class UnidadeCreateCommand : UnidadeCommand { }

    public class UnidadeUpdateCommand : UnidadeCommand { }

    public class UnidadeDeleteCommand : BaseCommand<ValidationResult>
    {
       
    }

    public class UnidadeValidator<T>
        : AbstractValidator<T> where T : UnidadeCommand
    {

        protected void ValidateIdEspaco() =>
            RuleFor(x => x.IdEspaco).GreaterThan(0).WithMessage("O ID do espaço deve ser maior que zero.");
        protected void ValidateRede() =>
            RuleFor(x => x.Rede).NotEmpty().WithMessage("A rede da unidade é obrigatória.");

    }

    public class UnidadeCreateCommandValidator 
        : UnidadeValidator<UnidadeCreateCommand>
    {
        public UnidadeCreateCommandValidator()
        {
            ValidateIdEspaco();
            ValidateRede();
        }
    }

    public class UnidadeUpdateCommandValidator 
        : UnidadeValidator<UnidadeUpdateCommand>
    {
        public UnidadeUpdateCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID da unidade deve ser maior que zero.");
            ValidateIdEspaco();
            ValidateRede();
        }
    }

    public class UnidadeDeleteCommandValidator 
        : AbstractValidator<UnidadeDeleteCommand>
    {
        public UnidadeDeleteCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O ID da unidade deve ser maior que zero.");
        }
    }





}
