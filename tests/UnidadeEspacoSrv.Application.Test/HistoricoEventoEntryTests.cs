using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Application.Test
{
    [TestFixture]
    public class HistoricoEventoEntryTests
    {
        [Test]
        public void HasTemporaryProperties_QuandoListaVazia_DeveRetornarFalse()
        {
            // Arrange
            var entry = new HistoricoEventoEntry(null!); // Passando null pois não usaremos o Entry neste teste

            // Assert
            Assert.IsFalse(entry.HasTemporaryProperties);
        }

        [Test]
        public void ToHistoricoEvento_DeveMapearPropriedadesCorretamente()
        {
            // Arrange
            var entry = new HistoricoEventoEntry(null!)
            {
                Codigo = 1,
                NomeTabela = "Usuarios",
                CodigoUsuario = 10,
                TipoOperacao = "Insert",
                ValoresChaves = new Dictionary<string, object> { { "Id", 1 } },
                ValoresNovos = new Dictionary<string, object> { { "Nome", "Teste" } }
            };

            // Act
            var resultado = entry.ToHistoricoEvento();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(entry.Codigo, resultado.Codigo);
                Assert.AreEqual(entry.NomeTabela, resultado.NomeTabela);
                Assert.AreEqual(entry.CodigoUsuario, resultado.CodigoUsuario);
                Assert.AreEqual(entry.TipoOperacao, resultado.TipoOperacao);

                // Verifica a serialização JSON
                Assert.AreEqual(JsonConvert.SerializeObject(entry.ValoresChaves), resultado.ValoresChaves);
                Assert.AreEqual(JsonConvert.SerializeObject(entry.ValoresNovos), resultado.ValoresNovos);

                // Valores antigos não foram preenchidos, deve ser null
                Assert.IsNull(resultado.ValoresAntigos);
            });
        }

        [Test]
        public void ToHistoricoEventoUpdate_DeveIncluirOIdInformado()
        {
            // Arrange
            var entry = new HistoricoEventoEntry(null!);
            int idEsperado = 99;

            // Act
            var resultado = entry.ToHistoricoEventoUpdate(idEsperado);

            // Assert
            Assert.AreEqual(idEsperado, resultado.Id);
        }
    }
}
