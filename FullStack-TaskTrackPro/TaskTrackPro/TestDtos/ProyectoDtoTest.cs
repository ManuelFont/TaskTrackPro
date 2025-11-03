using Dtos;


namespace TestDtos
{
    [TestClass]
    public class ProyectoDtoTest
    {
        [TestMethod]
        public void FechaInicio_DeberiaAsignarYRetornarCorrectamente()
        {
            var dto = new ProyectoDto();
            var fechaEsperada = new DateTime(2024, 1, 1);
            
            dto.FechaInicio = fechaEsperada;
            
            Assert.AreEqual(fechaEsperada, dto.FechaInicio);
        }
    }
}