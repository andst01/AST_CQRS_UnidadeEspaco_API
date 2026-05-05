using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.CrossCuting.AutoMapper
{
    public class EntityToNotificationMapping : Profile
    {
        public EntityToNotificationMapping()
        {
            CreateMap<Espaco, EspacoNotification>()
                .ForMember(m => m._Id, opt => opt.Ignore());
            CreateMap<Espaco, EspacoCreateNotification>()
                .ForMember(m => m._Id, opt => opt.Ignore());
            CreateMap<Espaco, EspacoDeleteNotification>()
               .ForMember(m => m._Id, opt => opt.Ignore());

            CreateMap<Unidade, UnidadeNotification>()
                .ForMember(m => m._Id, opt => opt.Ignore());
            CreateMap<Unidade, UnidadeCreateNotification>()
                .ForMember(m => m._Id, opt => opt.Ignore());
            CreateMap<Unidade, UnidadeUpdateNotification>()
              .ForMember(m => m._Id, opt => opt.Ignore());
            CreateMap<Unidade, UnidadeDeleteNotification>()
               .ForMember(m => m._Id, opt => opt.Ignore());
        }
    }
}
