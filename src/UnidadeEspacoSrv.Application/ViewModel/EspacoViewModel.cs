using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Application.ViewModel
{
    public class EspacoViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Endereco { get; set; }

        public virtual IEnumerable<UnidadeViewModel> Unidades { get; set; }
    }
}
