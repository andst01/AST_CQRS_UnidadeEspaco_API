using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Application.ViewModel
{
    public class EspacoViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }

        public virtual IEnumerable<UnidadeViewModel> Unidades { get; set; }

        public static explicit operator EspacoViewModel(EspacoNotification notification)
        {
            if (notification == null) return null;
            return new EspacoViewModel
            {
                Id = notification.Id,
                Nome = notification.Nome,
                Endereco = notification.Endereco,
                Unidades = notification.Unidades == null 
                        ? null 
                        : notification.Unidades?.Select(u => (UnidadeViewModel)u).ToList()
            };
        }

        public static explicit operator EspacoViewModel(Espaco entity)
        {
            if (entity == null) return null;
            return new EspacoViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Endereco = entity.Endereco,
                Unidades = entity.Unidades == null
                        ? null 
                        : entity.Unidades?.Select(u => (UnidadeViewModel)u).ToList()
            };
        }
    }
}
