using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Entities
{
    public class Unidade : EntityBase
    {
        public int Id { get; set; }

        public int IdEspaco { get; set; }

        public string Rede { get; set; }

        public virtual Espaco Espaco { get; set; }
    }
}
