using AutoFixture;
using Microsoft.EntityFrameworkCore;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Data.Repository.SQL;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class EspacoRepositoryTest
    {
        private SQLDbContext _mockContext;
        private EspacoRepository _repository;
        private Fixture _fixture = new Fixture();
        private Espaco _espaco = new Espaco();
        private List<Unidade> _unidadeList = new List<Unidade>();

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SQLDbContext>()
                       .UseInMemoryDatabase(Guid.NewGuid().ToString())
                       .Options;


            _mockContext = new SQLDbContext(options);
            _repository = new EspacoRepository(_mockContext);

            // Arrange
            _unidadeList = _fixture.Build<Unidade>()
                .Without(x => x.Espaco)
                .Without(x => x.ValidationResult)
                .CreateMany(2)
                .ToList();

            _espaco = _fixture.Build<Espaco>()
                .With(x => x.Unidades, _unidadeList)
                .Without(x => x.ValidationResult)
                .Create();
        }

        [TearDown]
        public void TearDown()
        {
            _mockContext?.Dispose();
            _mockContext = null;
            _repository = null;
        }

        [Test]
        public async Task AddEspacoAsyncTest()
        {

            // Act
            var result = await _repository.AddAsync(_espaco);
            // Assert
            Assert.IsNotNull(result);
            // _mockContext.Verify(m => m.Set<Espaco>().AddAsync(espaco, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task UpdateEspacoAsyncTest()
        {


            // Act
            await _repository.AddAsync(_espaco);
            _espaco.Nome = "Nome Atualizado";
            var result = await _repository.UpdateAsync(_espaco, _espaco.Id);
            // Assert
            Assert.IsNotNull(result);


        }

        [Test]
        public async Task DeleteEspacoAsyncTest()
        {
            // Arrange
            await _repository.AddAsync(_espaco);
            // Act
            await _repository.DeleteAsync(_espaco.Id);
            var result = await _repository.GetByIdAsync(_espaco.Id);
            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdEspacoAsyncTest()
        {
            // Arrange
            await _repository.AddAsync(_espaco);
            // Act
            var result = await _repository.GetByIdAsync(_espaco.Id);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_espaco.Id, result.Id);
        }

        [Test]
        public async Task GetAllEspacoAsyncTest()
        {
            // Arrange
           _mockContext.Set<Espaco>().AddRange(
                _fixture.Build<Espaco>()
                    .With(x => x.Unidades, _unidadeList)
                    .Without(x => x.ValidationResult)
                    .CreateMany(3)
            );

            _mockContext.SaveChangesAsync().Wait();
            _mockContext.Set<Espaco>().ToListAsync().Wait();
           
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }
}
