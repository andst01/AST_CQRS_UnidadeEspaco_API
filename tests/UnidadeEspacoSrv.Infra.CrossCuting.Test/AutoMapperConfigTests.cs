using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using UnidadeEspacoSrv.CrossCuting.Configuration;

namespace UnidadeEspacoSrv.Infra.CrossCuting.Test
{
    [TestFixture]
    public class AutoMapperConfigTests
    {
        [Test]
        public void AddAutoMappingConfig_ShouldRegisterAutoMapper()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddAutoMapperConfiguration();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();
            var configuration = serviceProvider.GetService<IConfigurationProvider>();

            Assert.Multiple(() =>
            {
                Assert.That(mapper, Is.Not.Null, "IMapper não foi registrado.");
                Assert.That(configuration, Is.Not.Null, "IConfigurationProvider não foi registrado.");
            });
        }

        [Test]
        public void AddAutoMappingConfig_ShouldThrowException_WhenServicesIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                AutoMapperConfiguration.AddAutoMapperConfiguration(null!));
        }

        [Test]
        public void AutoMapper_Configuration_Is_Valid()
        {
            // Este teste garante que todos os mapeamentos definidos nos Profiles 
            // (DomainToDTO e DTOToDomain) são compatíveis e não faltam propriedades.

            var services = new ServiceCollection();
            services.AddAutoMapperConfiguration();
            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetRequiredService<IConfigurationProvider>();

            // Assert
            Assert.DoesNotThrow(() => config.AssertConfigurationIsValid(),
                "A configuração do AutoMapper possui mapeamentos inválidos ou propriedades não mapeadas.");
        }
    }
}
