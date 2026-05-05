using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Events;

namespace UnidadeEspacoSrv.Domain.Interfaces.MongoDb
{
    public interface IEspacoMongoDbRepository : IMongoDbBaseRepository<EspacoNotification>
    {
        Task<List<EspacoReadModel>> ObterEspacosComUnidadesAsync();
    }
}
