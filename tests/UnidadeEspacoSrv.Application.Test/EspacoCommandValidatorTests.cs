using FluentValidation.TestHelper;
using UnidadeEspacoSrv.Application.Commnds;

namespace UnidadeEspacoSrv.Application.Test
{
    [TestFixture]
    public class EspacoCommandValidatorTests
    {
        private EspacoCreateCommandValidator _createValidator;
        private EspacoUpdateCommandValidator _updateValidator;
        private EspacoDeleteCommandValidator _deleteValidator;

        [SetUp]
        public void Setup()
        {
            _createValidator = new EspacoCreateCommandValidator();
            _updateValidator = new EspacoUpdateCommandValidator();
            _deleteValidator = new EspacoDeleteCommandValidator();
        }

        [Test]
        public void CreateCommand_DeveSerValido_QuandoDadosEstaoCorretos()
        {
            var command = new EspacoCreateCommand { Nome = "Auditório A", Endereco = "Rua X, 123" };
            var result = _createValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CreateCommand_DeveRetornarErro_QuandoNomeEEnderecoForemVazios()
        {
            var command = new EspacoCreateCommand { Nome = "", Endereco = "" };
            var result = _createValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Nome)
                  .WithErrorMessage("O nome do espaço é obrigatório.");

            result.ShouldHaveValidationErrorFor(x => x.Endereco)
                  .WithErrorMessage("O endereço do espaço é obrigatório.");
        }

        [Test]
        public void UpdateCommand_DeveRetornarErro_QuandoIdForInvalido()
        {
            var command = new EspacoUpdateCommand { Id = 0, Nome = "Teste", Endereco = "Endereco" };
            var result = _updateValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("O ID do espaço deve ser maior que zero.");
        }

        [Test]
        public void DeleteCommand_DeveSerValido_QuandoIdForMaiorQueZero()
        {
            var command = new EspacoDeleteCommand { Id = 10 };
            var result = _deleteValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void DeleteCommand_DeveRetornarErro_QuandoIdForZeroOuNegativo()
        {
            var command = new EspacoDeleteCommand { Id = -1 };
            var result = _deleteValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }


    }
}
