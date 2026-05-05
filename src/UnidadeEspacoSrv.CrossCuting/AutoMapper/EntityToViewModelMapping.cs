using AutoMapper;
using System;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class EntityToViewModelMapping : Profile
    {
        public EntityToViewModelMapping()
        {
            CreateMap<Unidade, UnidadeViewModel>()
                .ForMember(x => x.Espaco, opt => opt.MapFrom(o => o.Espaco)); 
            CreateMap<Espaco, EspacoViewModel>()
                .ForMember(x => x.Unidades, opt => opt.MapFrom(o => o.Unidades)); ;
        }
    }
}
