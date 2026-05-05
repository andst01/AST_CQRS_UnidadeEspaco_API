using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Entities
{
    public class HistoricoEvento : EntityBase
    {
        public HistoricoEvento() { }
        public  int Id { get; set; }
        public int Codigo { get; set; }
        public string NomeTabela { get; set; }
        public int? CodigoUsuario { get; set; }
        public DateTime DataCadastro { get; set; }
        public string ValoresChaves { get; set; }
        public string ValoresAntigos { get; set; }
        public string ValoresNovos { get; set; }
        public string TipoOperacao { get; set; }
    }
}
