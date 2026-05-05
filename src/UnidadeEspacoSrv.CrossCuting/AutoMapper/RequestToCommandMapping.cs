using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Request;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class RequestToCommandMapping : Profile
    {
        public RequestToCommandMapping()
        {
            CreateMap<EspacoRequest, EspacoCreateCommand>()
                .ForMember(x => x._ValidationResult, opt => opt.Ignore());
            CreateMap<EspacoRequest, EspacoUpdateCommand>()
                .ForMember(x => x._ValidationResult, opt => opt.Ignore());
            CreateMap<UnidadeRequest, UnidadeCreateCommand>()
                .ForMember(x => x._ValidationResult, opt => opt.Ignore());
            CreateMap<UnidadeRequest, UnidadeUpdateCommand>()
                .ForMember(x => x._ValidationResult, opt => opt.Ignore());
        }
    }
}
