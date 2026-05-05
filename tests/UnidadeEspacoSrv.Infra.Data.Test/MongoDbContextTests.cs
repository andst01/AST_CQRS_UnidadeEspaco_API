using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Data.Contexto;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class MongoDbContextTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IConfigurationSection> _mockConnSection;
        private Mock<IConfigurationSection> _mockDbSection;
    

     [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConnSection = new Mock<IConfigurationSection>();
            _mockDbSection = new Mock<IConfigurationSection>();

            // Simula: MongoDBConnection:ConnectionString
            _mockConnSection.Setup(s => s.Value).Returns("mongodb://localhost:27017");
            _mockConfiguration
                .Setup(c => c.GetSection("MongoDBConnection:ConnectionString"))
                .Returns(_mockConnSection.Object);

            // Simula: MongoDBConnection:DataBaseName
            _mockDbSection.Setup(s => s.Value).Returns("TestDatabase");
            _mockConfiguration
                .Setup(c => c.GetSection("MongoDBConnection:DataBaseName"))
                .Returns(_mockDbSection.Object);
        }

        [Test]
        public void Construtor_DeveInicializarDatabaseCorretamente()
        {
            // Act
            var context = new MongoDbContext(_mockConfiguration.Object);

            // Assert
            context.Database.Should().NotBeNull();
            context.Database.DatabaseNamespace.DatabaseName.Should().Be("TestDatabase");
        }

        [Test]
        public void GetCollection_DeveRetornarColecaoTipada()
        {
            // Arrange
            var context = new MongoDbContext(_mockConfiguration.Object);

            // Act
            var collection = context.GetCollection<BsonDocument>("MinhaColecao");

            // Assert
            collection.Should().NotBeNull();
            collection.CollectionNamespace.CollectionName.Should().Be("MinhaColecao");
        }

    }
}
