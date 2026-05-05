using Microsoft.EntityFrameworkCore;
using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Data.Contexto;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [SetUpFixture]
    public class GlobalTestSetup
    {
        public static MongoDbFixture MongoFixture { get; private set; }
        public static MongoDbRunner MongoRunner;
        public static IMongoClient MongoClient;
        public static IMongoDatabase MongoDatabase;
        public static DbContextOptions<SQLDbContext> SqlOptions;


        [OneTimeSetUp]
        public void GlobalSetup()
        {
            MongoFixture = new MongoDbFixture();
            MongoRunner = MongoDbRunner.Start();
            MongoClient = new MongoClient(MongoRunner.ConnectionString);
            MongoDatabase = MongoClient.GetDatabase("UnidadeEspacoTestDb");
            SqlOptions = new DbContextOptionsBuilder<SQLDbContext>()
                .UseInMemoryDatabase(databaseName: "UnidadeEspacoTestDb")
                .Options;
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            MongoRunner.Dispose();
            MongoClient.Dispose();
            MongoFixture.Dispose();
        }
    }
}
