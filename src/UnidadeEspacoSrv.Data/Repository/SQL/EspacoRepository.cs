using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Data.Repository.SQL
{
    /// <summary>
    /// Espaço Repository: Implementação do repositório para a entidade "Espaço" utilizando SQL.
    /// </summary>
    public class EspacoRepository 
        : SQLBaseRepository<Espaco>, 
        IEspacoRepository
    {
        public EspacoRepository(SQLDbContext context) : base(context)
        {
        }
    }
}
