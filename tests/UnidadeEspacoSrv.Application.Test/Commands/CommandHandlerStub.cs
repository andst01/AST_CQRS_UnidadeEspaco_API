using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Application.Test.Commands
{
    public class CommandHandlerStub : CommandHandler
    {
        public void TestAddError(string mensagem) => AddError(mensagem);

        public async Task<ValidationResult> TestCommit<T>(ISQLBaseRepository<T> repo, string? msg = null) where T : class
        {
            return await Commit(repo, msg);
        }

        // Propriedade para validar o estado interno
        public ValidationResult GetValidationResult() => ValidationResult;
    }

    [TestFixture]
    public class CommandHandlerTests
    {
        private CommandHandlerStub _handler;
        private Mock<ISQLBaseRepository<object>> _repoMock;

        [SetUp]
        public void SetUp()
        {
            _handler = new CommandHandlerStub();
            _repoMock = new Mock<ISQLBaseRepository<object>>();
        }

        [Test]
        public void AddError_DeveAdicionarErroAoValidationResult()
        {
            // Arrange
            var mensagemErro = "Erro de teste";

            // Act
            _handler.TestAddError(mensagemErro);
            var result = _handler.GetValidationResult();

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(mensagemErro, result.Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Commit_QuandoFalhar_DeveAdicionarMensagemPadrao()
        {
            // Arrange
            // Simula o SaveChangesAsync retornando false (falha)
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            var result = await _handler.TestCommit(_repoMock.Object);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("Houve um erro ao persistir os dados")));
        }

        [Test]
        public async Task Commit_QuandoSucesso_NaoDeveAdicionarErros()
        {
            // Arrange
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _handler.TestCommit(_repoMock.Object);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
