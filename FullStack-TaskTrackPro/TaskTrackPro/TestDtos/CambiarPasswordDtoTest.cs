using Dtos;
using System.ComponentModel.DataAnnotations;

namespace TestDtos
{
    [TestClass]
    public class CambiarPasswordDtoTest
    {
        private bool EsValido(object obj, out List<ValidationResult> results)
        {
            var context = new ValidationContext(obj);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, context, results, true);
        }

        [TestMethod]
        public void CambiarPasswordDto_ConCamposValidos_EsValido()
        {
            var dto = new CambiarPasswordDto
            {
                ContraseniaActual = "Actual123!",
                NuevaContrasenia = "Nueva123!",
                ConfirmarContrasenia = "Nueva123!"
            };

            var esValido = EsValido(dto, out var resultados);

            Assert.IsTrue(esValido);
            Assert.AreEqual(0, resultados.Count);
        }

        [TestMethod]
        public void CambiarPasswordDto_SinContraseniaActual_NoEsValido()
        {
            var dto = new CambiarPasswordDto
            {
                NuevaContrasenia = "Nueva123!",
                ConfirmarContrasenia = "Nueva123!"
            };

            var esValido = EsValido(dto, out var resultados);

            Assert.IsFalse(esValido);
            Assert.IsTrue(resultados.Exists(r => r.MemberNames.Contains(nameof(CambiarPasswordDto.ContraseniaActual))));
        }

        [TestMethod]
        public void CambiarPasswordDto_SinNuevaContrasenia_NoEsValido()
        {
            var dto = new CambiarPasswordDto
            {
                ContraseniaActual = "Actual123!",
                ConfirmarContrasenia = "Nueva123!"
            };

            var esValido = EsValido(dto, out var resultados);

            Assert.IsFalse(esValido);
            Assert.IsTrue(resultados.Exists(r => r.MemberNames.Contains(nameof(CambiarPasswordDto.NuevaContrasenia))));
        }

        [TestMethod]
        public void CambiarPasswordDto_SinConfirmarContrasenia_NoEsValido()
        {
            var dto = new CambiarPasswordDto
            {
                ContraseniaActual = "Actual123!",
                NuevaContrasenia = "Nueva123!"
            };

            var esValido = EsValido(dto, out var resultados);

            Assert.IsFalse(esValido);
            Assert.IsTrue(resultados.Exists(r => r.MemberNames.Contains(nameof(CambiarPasswordDto.ConfirmarContrasenia))));
        }
    }
}
