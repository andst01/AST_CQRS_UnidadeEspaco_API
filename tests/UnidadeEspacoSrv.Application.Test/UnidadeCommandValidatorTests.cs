using FluentValidation.TestHelper;
using System.Runtime.ConstrainedExecution;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Domain.Entities;

namespace UnidadeEspacoSrv.Application.Test
{
    [TestFixture]
    public class UnidadeCommandValidatorTests
    {
        private UnidadeCreateCommandValidator _createValidator;
        private UnidadeUpdateCommandValidator _updateValidator;
        private UnidadeDeleteCommandValidator _deleteValidator;

        [SetUp]
        public void Setup()
        {
            _createValidator = new UnidadeCreateCommandValidator();
            _updateValidator = new UnidadeUpdateCommandValidator();
            _deleteValidator = new UnidadeDeleteCommandValidator();
        }

        [Test]
        public void CreateCommand_DeveSerValido_QuandoDadosEstaoCorretos()
        {
            var command = new UnidadeCreateCommand { Rede = "Rede X A", IdEspaco = 2, Id = 1 };
            var result = _createValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CreateCommand_DeveRetornarErro_QuandoNomeEEnderecoForemVazios()
        {
            var command = new UnidadeCreateCommand {  Id = 0 };
            var result = _createValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Rede)
                  .WithErrorMessage("A rede da unidade é obrigatória.");

            result.ShouldHaveValidationErrorFor(x => x.IdEspaco)
                  .WithErrorMessage("O ID do espaço deve ser maior que zero.");
        }

        [Test]
        public void UpdateCommand_DeveRetornarErro_QuandoIdForInvalido()
        {
            var command = new UnidadeUpdateCommand {  Id = 1 };
            var result = _updateValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Rede)
                  .WithErrorMessage("A rede da unidade é obrigatória.");

            result.ShouldHaveValidationErrorFor(x => x.IdEspaco)
                  .WithErrorMessage("O ID do espaço deve ser maior que zero.");
        }
    }
}
