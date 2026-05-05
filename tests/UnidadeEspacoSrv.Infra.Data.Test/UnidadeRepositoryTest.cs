using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Data.Repository.SQL;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Infra.Data.Test
{
    [TestFixture]
    public class UnidadeRepositoryTest
    {

        private SQLDbContext _mockContext;
        private UnidadeRepository _repository;
        private Fixture _fixture = new Fixture();
        private Espaco _espaco = new Espaco();
        private Unidade _unidade = new Unidade();

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SQLDbContext>()
                       .UseInMemoryDatabase(Guid.NewGuid().ToString())
                       .Options;
            _mockContext = new SQLDbContext(options);
            _repository = new UnidadeRepository(_mockContext);

            _espaco = _fixture.Build<Espaco>()
                .Without(x => x.Unidades)
                .Without(x => x.ValidationResult)
                .Create();

            _unidade = _fixture.Build<Unidade>()
                .With(x => x.Espaco, _espaco)
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
        public async Task AddUnidadeAsyncTest()
        {

            // Act
            var result = await _repository.AddAsync(_unidade);
            // Assert
            Assert.IsNotNull(result);
            // _mockContext.Verify(m => m.Set<Espaco>().AddAsync(espaco, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task UpdateUnidadeAsyncTest()
        {


            // Act
            await _repository.AddAsync(_unidade);
            _unidade.Rede = "Rede Atualizado";
            var result = await _repository.UpdateAsync(_unidade, _unidade.Id);
            // Assert
            Assert.IsNotNull(result);


        }

        [Test]
        public async Task DeleteUnidadeAsyncTest()
        {
            // Arrange
            await _repository.AddAsync(_unidade);
            // Act
            await _repository.DeleteAsync(_unidade.Id);
            var result = await _repository.GetByIdAsync(_unidade.Id);
            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByIdUnidadeAsyncTest()
        {
            // Arrange
            await _repository.AddAsync(_unidade);
           // await _repository.SaveChangesAsync();
            // Act
            var result = await _repository.GetByIdAsync(_unidade.Id);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_unidade.Id, result.Id);
        }

        [Test]
        public async Task GetAllUnidadeAsyncTest()
        {
            // Arrange
            // await _repository.AddAsync(_unidade);
            //await _repository.SaveChangesAsync();
            _mockContext.Set<Unidade>().Add(_unidade);
            _mockContext.SaveChanges();
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }
}
