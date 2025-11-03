using Dtos;


namespace TestDtos
{
    [TestClass]
    public class NotificacionDtoTest
    {
        [TestMethod]
        public void NotificacionDto_AsignarValores_PropiedadesAsignadasCorrectamente()
        {
            var fecha = new DateTime(2025, 5, 12);
            var dto = new NotificacionDto
            {
                Mensaje = "Tarea crítica detectada",
                Fecha = fecha
            };

            Assert.AreEqual("Tarea crítica detectada", dto.Mensaje);
            Assert.AreEqual(fecha, dto.Fecha);
        }
    }
}