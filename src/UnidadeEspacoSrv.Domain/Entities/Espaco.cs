using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Entities
{
    public class Espaco : EntityBase
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }

        public virtual ICollection<Unidade> Unidades { get; set; }

    }
}
