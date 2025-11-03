using Backend.Dominio;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Servicios;

namespace TestServicios;

[TestClass]
public class ServicioNotificacionesTests
{
    private SqlContext _context;
    private ServicioNotificaciones _servicio;
    private RepositorioNotificaciones _repo;

    [TestInitialize]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase("TestDb_Notificaciones_" + Guid.NewGuid())
            .Options;

        _context = new SqlContext(options);
        _repo = new RepositorioNotificaciones(_context);
        _servicio = new ServicioNotificaciones(_repo);
    }

    [TestMethod]
    public void Notificar_DeberiaAgregarUnaNotificacion()
    {
        var usuario = new Usuario("Test", "User", "test@mail.com", "Pass123@", DateTime.Today.AddYears(-20), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        _servicio.Notificar("Mensaje de prueba", usuario);

        var notificaciones = _context.Notificaciones.Where(n => n.UsuarioEmail == "test@mail.com").ToList();
        Assert.AreEqual(1, notificaciones.Count);
        Assert.AreEqual("Mensaje de prueba", notificaciones[0].Mensaje);
        Assert.IsFalse(notificaciones[0].Vista);
    }

    [TestMethod]
    public void TraerNoVistas_DeberiaRetornarSoloLasNoVistas()
    {
        var usuario = new Usuario("Ana", "Tester", "ana@mail.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        _context.Notificaciones.Add(new Notificacion("Vista", "ana@mail.com") { Vista = true });
        _context.Notificaciones.Add(new Notificacion("No vista", "ana@mail.com") { Vista = false });
        _context.SaveChanges();

        var resultado = _servicio.TraerNoVistas("ana@mail.com");

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("No vista", resultado[0].Mensaje);
    }

    [TestMethod]
    public void MarcarComoVista_DeberiaActualizarNotificacion()
    {
        var usuario = new Usuario("Pepita", "Perez", "pepita@mail.com", "Pass123@", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        var noti = new Notificacion("Sin leer", "pepita@mail.com");
        _context.Notificaciones.Add(noti);
        _context.SaveChanges();

        _servicio.MarcarComoVista(noti.Id);

        var actualizada = _context.Notificaciones.Find(noti.Id);
        Assert.IsTrue(actualizada.Vista);
    }
}
