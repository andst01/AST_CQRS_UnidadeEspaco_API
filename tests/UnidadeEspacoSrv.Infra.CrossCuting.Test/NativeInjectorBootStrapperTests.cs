using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.CrossCuting;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Infra.CrossCuting.Test
{

    [TestFixture]
    public class NativeInjectorBootStrapperTests
    {
        private ServiceCollection _services;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();

            // Algumas dependências exigem configurações externas (como DBContext)
            // Se o seu SQLDbContext não tiver um construtor vazio, você precisa mockar as opções:
            var options = new DbContextOptionsBuilder<SQLDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _services.AddSingleton(options);

            // Mock básico de IConfiguration se algum serviço precisar
            var configMock = new Mock<IConfiguration>();
            _services.AddSingleton(configMock.Object);

            // Act - Chama o seu método de extensão
            _services.RegisteredServices();

            _serviceProvider = _services.BuildServiceProvider();
        }

        [Test]
        [TestCase(typeof(IEspacoApp))]
        [TestCase(typeof(IUnidadeApp))]
        [TestCase(typeof(IMediatorHandler))]
        [TestCase(typeof(IUnidadeRepository))]
        [TestCase(typeof(IUnidadeMongoDbRepository))]
        public void RegisteredServices_ShouldResolveImportantInterfaces(Type serviceType)
        {
            var exists = _services.Any(s => s.ServiceType == serviceType);

            Assert.That(exists, Is.True, $"Serviço {serviceType.Name} não foi registrado.");
        }

        [Test]
        public void RegisteredServices_ShouldRegisterMediatR()
        {
            // Verifica se o MediatR (IMediator) foi registrado corretamente pelo método
            var mediator = _serviceProvider.GetService<IMediator>();
            Assert.That(mediator, Is.Not.Null);
        }

        [Test]
        public void RegisteredServices_ShouldRegisterGenericsCorrectly()
        {
            // Testa se repositórios genéricos estão funcionando
            // Como o repositório é ISQLBaseRepository<T>, testamos com uma entidade real
            var genericRepo = _serviceProvider.GetService<ISQLBaseRepository<UnidadeEspacoSrv.Domain.Entities.Unidade>>();

            Assert.That(genericRepo, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
