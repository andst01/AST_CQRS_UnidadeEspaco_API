using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    public class MongoDbFixture : IDisposable
    {
        public MongoDbRunner Runner { get; private set; }
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }

        public MongoDbFixture()
        {
            // Inicia um executável real do MongoDB em uma porta aleatória
            Runner = MongoDbRunner.Start();
            Client = new MongoClient(Runner.ConnectionString);
            Database = Client.GetDatabase("TestDb_" + Guid.NewGuid());
        }

        public void Dispose()
        {
            Runner.Dispose();
        }
    }
}
