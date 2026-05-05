using Microsoft.Extensions.DependencyInjection;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.CrossCuting.Configuration;

namespace UnidadeEspacoSrv.Infra.CrossCuting.Test
{

    [TestFixture]
    public class DependencyInjectionConfigurationTests
    {
        [Test]
        public void AddDIConfiguration_ShouldThrowException_WhenServicesIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                DependencyInjectionConfiguration.AddDIConfiguration(null!));
        }

        [Test]
        [Ignore("Motivo do teste estar ignorado")]
        public void AddDIConfiguration_ShouldExecuteNativeInjectorAndRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Como o NativeInjectorBootStrapper registra o IHttpContextAccessor
            // e interfaces de App, vamos usá-los como prova de execução.

            // Act
           

            services.AddDIConfiguration();
            services.AddAutoMapperConfiguration();
           // services.AddSwaggerConfiguration(builder.Configuration);
           // services.AddDatabaseConfiguration(builder.Configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            // Verificamos se um serviço que sabidamente está no NativeInjector foi registrado
            var unityApp = serviceProvider.GetService<IUnidadeApp>();
            var httpContext = serviceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

            Assert.Multiple(() =>
            {
                Assert.That(unityApp, Is.Not.Null, "IUnidadeApp deveria ter sido registrado via NativeInjector.");
                Assert.That(httpContext, Is.Not.Null, "IHttpContextAccessor deveria ter sido registrado via NativeInjector.");
            });
        }
    }
}
