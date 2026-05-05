using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Application.Request
{
    public class UnidadeRequest
    {
        public int Id { get; set; }

        public int IdEspaco { get; set; }

        public string Rede { get; set; }
    }
}
