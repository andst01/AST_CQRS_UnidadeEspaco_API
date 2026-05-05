using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnidadeEspacoSrv.CrossCuting.Configuration;

namespace UnidadeEspacoSrv.Infra.CrossCuting.Test
{
    public class SwaggerConfigurationTests
    {
        private ServiceCollection _services;
        private Mock<IConfiguration> _configurationMock;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            _configurationMock = new Mock<IConfiguration>();

            // O SwaggerGen depende de serviços de Logging e de MVC/Routing para funcionar
            _services.AddLogging();
            _services.AddMvcCore();
        }

        [Test]
        public void AddSwaggerConfiguration_ShouldRegisterSwaggerServices()
        {
            // Act
            _services.AddSwaggerConfiguration(_configurationMock.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var swaggerGenOptions = serviceProvider.GetService<IOptions<SwaggerGenOptions>>();

            Assert.That(swaggerGenOptions, Is.Not.Null, "SwaggerGenOptions não foi registrado.");
        }

        [Test]
        public void AddSwaggerConfiguration_ShouldHaveCorrectInfoAndDoc()
        {
            // Act
            _services.AddSwaggerConfiguration(_configurationMock.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Obtemos as opções configuradas
            var options = serviceProvider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

            // Verificamos se o "v1" foi criado com os dados corretos
            Assert.Multiple(() =>
            {
                Assert.That(options.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey("v1"), Is.True);

                var info = options.SwaggerGeneratorOptions.SwaggerDocs["v1"];
                Assert.That(info.Title, Is.EqualTo("Espaço/Unidade API"));
                Assert.That(info.Version, Is.EqualTo("v1"));
                Assert.That(info.Description, Is.EqualTo("Espaço/Unidade API Swagger"));
            });
        }

        [Test]
        public void AddSwaggerConfiguration_ShouldThrowException_WhenServicesIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                SwaggerConfiguration.AddSwaggerConfiguration(null!, _configurationMock.Object));
        }
    }
}
