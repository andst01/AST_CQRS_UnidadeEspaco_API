using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Domain.Interfaces.MongoDb
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
