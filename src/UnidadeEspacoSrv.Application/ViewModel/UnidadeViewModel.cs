using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Application.ViewModel
{
    public class UnidadeViewModel
    {
        public int Id { get; set; }

        public int IdEspaco { get; set; }

        public string Rede { get; set; }

        public EspacoViewModel Espaco { get; set; }

        public static explicit operator UnidadeViewModel(UnidadeNotification notification)
        {
            if (notification == null) return null;
            return new UnidadeViewModel
            {
                Id = notification.Id,
                IdEspaco = notification.IdEspaco,
                Rede = notification.Rede,
                Espaco = (EspacoViewModel)notification.Espaco
            };
        }

        public static explicit operator UnidadeViewModel(Unidade entity)
        {
            if (entity == null) return null;
            return new UnidadeViewModel
            {
                Id = entity.Id,
                IdEspaco = entity.IdEspaco,
                Rede = entity.Rede,
                Espaco = (EspacoViewModel)entity.Espaco
            };
        }
    }
}
