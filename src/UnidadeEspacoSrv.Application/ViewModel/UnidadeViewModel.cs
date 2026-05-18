using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Application.Interfaces;
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

        public UnidadeViewModel() { }

        public UnidadeViewModel(Unidade entity)
        {
            Id = entity.Id;
            IdEspaco = entity.IdEspaco;
            Rede = entity.Rede;
            Espaco = entity.Espaco == null 
                ? null 
                : new EspacoViewModel(entity.Espaco);
        }

        public UnidadeViewModel(UnidadeNotification notification)
        {
            Id = notification.Id;
            IdEspaco = notification.IdEspaco;
            Rede = notification.Rede;
            Espaco = notification.Espaco == null 
                ? null 
                : new EspacoViewModel(notification.Espaco);
        }

        //public void MapFromEntity(Unidade entity)
        //{
        //    Id = entity.Id;
        //    IdEspaco = entity.IdEspaco;
        //    Rede = entity.Rede;
        //    if (entity.Espaco == null)
        //    {
        //        Espaco = null;
        //    }
        //    else
        //    {
        //        var evm = new EspacoViewModel();
        //        evm.MapFromEntity(entity.Espaco);
        //        Espaco = evm;
        //    }
        //}

        //public void MapFromNotification(UnidadeNotification notification)
        //{
        //    Id = notification.Id;
        //    IdEspaco = notification.IdEspaco;
        //    Rede = notification.Rede;

        //    if (notification.Espaco == null)
        //    {
        //        Espaco = null;
        //    }
        //    else
        //    {
        //        var evm = new EspacoViewModel();
        //        evm.MapFromNotification(notification.Espaco);
        //        Espaco = evm;
        //    }
        //}

        //public static explicit operator UnidadeViewModel(UnidadeNotification notification)
        //{
        //    if (notification == null) return null;
        //    var vm = new UnidadeViewModel
        //    {
        //        Id = notification.Id,
        //        IdEspaco = notification.IdEspaco,
        //        Rede = notification.Rede
        //    };

        //    if (notification.Espaco != null)
        //    {
        //        var evm = new EspacoViewModel();
        //        evm.MapFromNotification(notification.Espaco);
        //        vm.Espaco = evm;
        //    }

        //    return vm;
        //}

        //public static explicit operator UnidadeViewModel(Unidade entity)
        //{
        //    if (entity == null) return null;
        //    var vm = new UnidadeViewModel
        //    {
        //        Id = entity.Id,
        //        IdEspaco = entity.IdEspaco,
        //        Rede = entity.Rede
        //    };

        //    if (entity.Espaco != null)
        //    {
        //        var evm = new EspacoViewModel();
        //        evm.MapFromEntity(entity.Espaco);
        //        vm.Espaco = evm;
        //    }

        //    return vm;
        //}
    }
}
