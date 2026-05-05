using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnidadeEspacoSrv.CrossCuting.Configuration;
using UnidadeEspacoSrv.Data.Contexto;

namespace UnidadeEspacoSrv.Infra.CrossCuting.Test
{
    [TestFixture]
    public class DataBaseConfigurationTests
    {
        private ServiceCollection _services;
        private Mock<IConfiguration> _configurationMock;
        private const string ConnectionString = "Server=myServer;Database=myDb;User Id=myUser;Password=myPassword;";

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            _configurationMock = new Mock<IConfiguration>();

            // Simula a leitura da ConnectionString do appsettings.json
            _configurationMock
                .Setup(c => c.GetSection("ConnectionStrings")["DefaultConnectionString"])
                .Returns(ConnectionString);
        }

        [Test]
        public void AddDatabaseConfiguration_ShouldRegisterDbContext()
        {
            // Act
            _services.AddDatabaseConfiguration(_configurationMock.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var dbContext = serviceProvider.GetService<SQLDbContext>();
            Assert.That(dbContext, Is.Not.Null, "O SQLDbContext não foi registrado no container.");
        }

        [Test]
        public void AddDatabaseConfiguration_ShouldUseCorrectOptions()
        {
            // Act
            _services.AddDatabaseConfiguration(_configurationMock.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Obtemos as opções que foram registradas para o SQLDbContext
            var options = serviceProvider.GetRequiredService<DbContextOptions<SQLDbContext>>();

            // Verificamos se o Lazy Loading foi habilitado
            // O UseLazyLoadingProxies adiciona uma extensão específica nas opções
            var hasLazyLoading = options.Extensions.Any(e => e.GetType().Name == "ProxiesOptionsExtension");

            // Verificamos se o SQL Server foi configurado
            var hasSqlServer = options.Extensions.Any(e => e.GetType().Name == "SqlServerOptionsExtension");

            Assert.Multiple(() =>
            {
                Assert.That(hasLazyLoading, Is.True, "Lazy Loading Proxies não foi configurado.");
                Assert.That(hasSqlServer, Is.True, "O provedor SQL Server não foi configurado.");
            });
        }

        [Test]
        public void AddDatabaseConfiguration_ShouldThrowException_WhenServicesIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                DataBaseConfiguration.AddDatabaseConfiguration(null!, _configurationMock.Object));
        }
    }
}
