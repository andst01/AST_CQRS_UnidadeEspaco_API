using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.ViewModel;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class ViewlModelToCommandMapping : Profile
    {
        public ViewlModelToCommandMapping()
        {

            CreateMap<EspacoViewModel, EspacoCommand>()
                .ForMember(m => m._ValidationResult, opt => opt.Ignore());
                
            CreateMap<UnidadeViewModel, UnidadeCommand>()
                .ForMember(m => m._ValidationResult, opt => opt.Ignore());


        }
    }
}
