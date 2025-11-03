using Dtos;

namespace TestDtos
{
    [TestClass]
    public class RecursoDtoTest
    {
        [TestMethod]
        public void CrearRecursoDto_ValoresAsignadosCorrectamente()
        {
            var dto = new RecursoDto
            {
                Nombre = "Andamio",
                Tipo = "Equipamiento",
                Descripcion = "Soporte estructural temporal",
                Seleccionado = true,
                Funcionalidad = "C++"
            };

            Assert.AreEqual("Andamio", dto.Nombre);
            Assert.AreEqual("Equipamiento", dto.Tipo);
            Assert.AreEqual("Soporte estructural temporal", dto.Descripcion);
            Assert.IsTrue(dto.Seleccionado);
            Assert.AreEqual("C++", dto.Funcionalidad);
        }

        [TestMethod]
        public void ModificarSeleccionado_CambiaValorCorrectamente()
        {
            var dto = new RecursoDto { Seleccionado = false };
            dto.Seleccionado = true;
            Assert.IsTrue(dto.Seleccionado);
        }
    }
}