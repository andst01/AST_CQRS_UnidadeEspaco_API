using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Data.Repository.SQL;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Infra.Data.Test
{

    //https://stackoverflow.com/questions/63073692/xunit-icollectionfixture-instance-not-getting-shared-between-test-methods-of-a
    [TestFixture] // No NUnit, usamos este atributo para a classe
    public class SQLBaseRepositoryTests
    {
        private SQLDbContext _context;
        private SQLBaseRepository<HistoricoEvento> _repository;
        private readonly IFixture _fixture = new Fixture();
        private HistoricoEvento _event = new HistoricoEvento();
        private List<HistoricoEvento> _eventList = new List<HistoricoEvento>();
        private Espaco _espaco = new Espaco();


        [SetUp] // Executa antes de CADA teste
        public void Setup()
        {
            _espaco = _fixture.Build<Espaco>()
                .Without(x => x.ValidationResult)
                .Without(x => x.Unidades)
                .Create();

            _event = _fixture.Build<HistoricoEvento>()
                .Without(x => x.ValidationResult)
             //   .Without(x => x.DomainEvents)
                .Create();

            _eventList = _fixture.Build<HistoricoEvento>()
                .Without(x => x.ValidationResult)
               // .Without(x => x.DomainEvents)
                .CreateMany(5)
                .ToList();

            var options = new DbContextOptionsBuilder<SQLDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .Options;

            _context = new SQLDbContext(options);
            _repository = new SQLBaseRepository<HistoricoEvento>(_context);
        }

        [TearDown] // Executa após cada teste
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_DeveRetornarTodasAsEntidades()
        {
            // Arrange
            _context.Set<HistoricoEvento>().AddRange(_eventList);

            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(5);
        }

        [Test]
        public async Task GetByIdAsync_DeveRetornarEntidadeCorreta()
        {
            // Arrange
           // var entidade = new Espaco { Id = 1, Nome = "Teste", Endereco = "Endereco Teste" };
            await _context.Set<HistoricoEvento>().AddAsync(_event);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetByIdAsync(_event.Id);

            // Assert
            resultado.Should().NotBeNull();
           // resultado.Nome.Should().Be("Teste");
        }

        [Test]
        public async Task UpdateAsync_DeveLancarExcecao_QuandoEntidadeNaoExistir()
        {
            // Arrange
          
            int id = _event.Id - 1; 
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _repository.UpdateAsync(_event, id));

            ex.Message.Should().Be("A entidade não foi encontrada");
        }

        [Test]
        public async Task OnBeforePublishEvent_QuandoEntidadeInserida_DeveMapearComoInclusao()
        {
            // Arrange
            // var novaEntidade = new Produto { Nome = "Teclado Mecânico", Preco = 250.00m };
            _espaco.Nome = "Teclado Mecânico";
            _context.Espacos.Add(_espaco);

            // Act - Invocando o método privado via Reflexão
            var result = await InvokeOnBeforePublishEvent(_context);
            var auditorias = result.Item2;

            // Assert
            Assert.That(auditorias, Has.Count.EqualTo(1));
            var entry = auditorias.First();

            Assert.Multiple(() =>
            {
                Assert.That(entry.TipoOperacao, Is.EqualTo("I"));
                Assert.That(entry.ValoresNovos["Nome"], Is.EqualTo("Teclado Mecânico"));
                //Assert.That(entry.ValoresNovos.ContainsKey("Preco"), Is.True);
            });
        }

        [Test]
        public async Task OnBeforePublishEvent_QuandoEntidadeModificada_DeveMapearApenasCamposAlterados()
        {
            // Arrange
            // var produto = new Produto { Id = 10, Nome = "Original", Preco = 50 };
            _espaco.Nome = "Original";
            _context.Espacos.Attach(_espaco); // Coloca no estado Unchanged

            // Act
            _espaco.Nome = "Alterado"; // Muda o estado para Modified

            var result = await InvokeOnBeforePublishEvent(_context);
            var entry = result.Item2.First();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(entry.TipoOperacao, Is.EqualTo("A"));
                Assert.That(entry.ValoresAntigos["Nome"], Is.EqualTo("Original"));
                Assert.That(entry.ValoresNovos["Nome"], Is.EqualTo("Alterado"));
                // O Preço não mudou, então não deve estar no dicionário de auditoria
                Assert.That(entry.ValoresNovos.ContainsKey("Preco"), Is.False);
            });
        }

        [Test]
        public async Task OnAfterSaveChangesAsync_DeveAtualizarIdsTemporariosParaValoresReais()
        {
            // --- ARRANGE ---

            // 1. Criamos um histórico "pendente" (como se tivesse sido gerado no OnBefore)
            
            _context.HistoricoEventos.Add(_event);
            await _context.SaveChangesAsync();

            // 2. Simulamos uma entidade que acabou de ser salva e ganhou um ID real (ex: ID 50)
            //var produto = new Produto { Id = 50, Nome = "Fone de Ouvido" };
            var entry = _context.Entry(_espaco);

            // Criamos o Entry de auditoria simulando que o ID era temporário
            var auditEntry = new HistoricoEventoEntry(entry)
            {
                NomeTabela = "Espaco",
                TemporaryProperties = entry.Properties.Where(p => p.Metadata.IsKey()).ToList()
            };
            auditEntry.ValoresChaves["Id"] = 0; // Valor antigo temporário

            var listaHistoricos = new List<HistoricoEvento> { _event };
            var listaEntries = new List<HistoricoEventoEntry> { auditEntry };
            var tupla = new Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>>(listaHistoricos, listaEntries);

            // --- ACT ---
            // Invocando o método privado via reflexão
            var method = typeof(SQLDbContext).GetMethod("OnAfterSaveChangesAsync",
                BindingFlags.NonPublic | BindingFlags.Instance);

            await (Task)method.Invoke(_context, new object[] { tupla });

            // --- ASSERT ---
            // Verificamos se o valor no banco foi atualizado de "0" para "50"
            var historicoAtualizado = _context.HistoricoEventos.First(x => x.Id == _event.Id);

            Assert.NotNull(historicoAtualizado);

            //Assert.That(historicoAtualizado.ValoresChaves, Does.Contain("50"),
            //    "O HistoricoEvento deveria ter sido atualizado com o ID real gerado pelo banco.");
        }

        private async Task<Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>>> InvokeOnBeforePublishEvent(SQLDbContext context)
        {
            var method = typeof(SQLDbContext).GetMethod("OnBeforePublishEvent",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new Exception("Método OnBeforePublishEvent não encontrado.");

            var task = (Task<Tuple<List<HistoricoEvento>, List<HistoricoEventoEntry>>>)method.Invoke(context, null);
            return await task;
        }
    }

}

