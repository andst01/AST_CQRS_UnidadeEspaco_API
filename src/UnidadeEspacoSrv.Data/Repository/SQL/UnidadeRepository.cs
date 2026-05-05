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
    /// Unidade Repository: Implementação do repositório para a entidade "Unidade" utilizando SQL.
    /// </summary>
    public class UnidadeRepository 
        : SQLBaseRepository<Unidade>
        , IUnidadeRepository
    {
        public UnidadeRepository(SQLDbContext context) : base(context)
        {
        }
    }
}
