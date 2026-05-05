using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class CommandToEntityMapping : Profile
    {
        public CommandToEntityMapping()
        {
           

            CreateMap<EspacoCommand, Espaco>()
                .ForMember(m => m.ValidationResult, opt => opt.Ignore())
                .ForMember(m => m.DomainEvents, opt => opt.Ignore())
                .ForMember(m => m.Unidades, opt => opt.Ignore());
            CreateMap<UnidadeCommand, Unidade>()
                .ForMember(m => m.ValidationResult, opt => opt.Ignore())
                .ForMember(m => m.DomainEvents, opt => opt.Ignore())
                .ForMember(m => m.Espaco, opt => opt.Ignore());
        }
    }
}
