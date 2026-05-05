using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class NotificationToViewModelMapping : Profile
    {
        public NotificationToViewModelMapping()
        {
            CreateMap<UnidadeNotification, UnidadeViewModel>()
                .ForMember(x => x.Espaco, opt => opt.MapFrom(o => o.Espaco));
            CreateMap<EspacoNotification, EspacoViewModel>()
                .ForMember(x => x.Unidades, opt => opt.MapFrom(o => o.Unidades));

        }
    }
}
