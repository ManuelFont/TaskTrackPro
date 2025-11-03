using Backend.Dominio;

namespace TestBackend.TestDominio
{
    [TestClass]
    public class NotificacionTest
    {
        [TestMethod]
        public void Constructor_Valido_CreaNotificacionCorrecta()
        {
            string mensaje = "Tarea crítica retrasada";
            string email = "usuario@mail.com";

            var notificacion = new Notificacion(mensaje, email);

            Assert.AreEqual(mensaje, notificacion.Mensaje);
            Assert.AreEqual(email, notificacion.UsuarioEmail);
            Assert.IsFalse(notificacion.Vista);
            Assert.IsTrue((DateTime.Now - notificacion.Fecha).TotalSeconds < 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_MensajeVacio_LanzaExcepcion()
        {
            string mensaje = "";
            string email = "usuario@mail.com";

            var notificacion = new Notificacion(mensaje, email);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_MensajeSoloEspacios_LanzaExcepcion()
        {
            string mensaje = "   ";
            string email = "usuario@mail.com";

            var notificacion = new Notificacion(mensaje, email);
        }

        [TestMethod]
        public void MarcarComoVista_CambiaEstadoDeVista()
        {
            var notificacion = new Notificacion("Alerta de retraso", "usuario@mail.com");

            notificacion.MarcarComoVista();

            Assert.IsTrue(notificacion.Vista);
        }
    }
}
