using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Interfaces.MongoDb
{
    public interface IUnidadeMongoDbRepository 
        : IMongoDbBaseRepository<UnidadeNotification>
    {
        /// <summary>
        /// Obtem todas as unidades que possuem espaço
        /// </summary>
        /// <returns></returns>
        Task<List<UnidadeReadModel>> ObterTodasUnidadeComEspaco();

        /// <summary>
        /// Asynchronously retrieves a unit by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the unit to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unit read model if found;
        /// otherwise, null.</returns>
        Task<UnidadeReadModel> ObterUnidadeComEspacoPorIdAsync(int id);
    }
}
