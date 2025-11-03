using Backend.Dominio;
using DataAccess;

using InterfacesDataAccess;
using Microsoft.EntityFrameworkCore;


namespace TestDataAccess;

[TestClass]
public class RepositorioNotificacionesTest
{
    private SqlContext _context;
    private RepositorioNotificaciones _repositorio;
    private readonly InMemoryAppContextFactory _factory = new();

    
    [TestInitialize]
    public void Setup()
    {
        string dbName = "TestDb_" + Guid.NewGuid();
        _context = _factory.CreateDbContext(dbName);
        _repositorio = new RepositorioNotificaciones(_context);
    }


    [TestMethod]
    public void AgregarNotificacion_DeberiaGuardarse()
    {
        var usuario = new Usuario("Juan", "Tester", "juan@mail.com", "Hola123!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        var notificacion = new Notificacion("Mensaje 1", usuario.Email);
        _repositorio.Agregar(notificacion); 

        var guardadas = _context.Notificaciones.ToList();

        Assert.AreEqual(1, guardadas.Count);
        Assert.AreEqual("Mensaje 1", guardadas[0].Mensaje);
    }

    [TestMethod]
    public void ObtenerNoVistas_DeberiaTraerSoloNoVistas()
    {

        var usuario = new Usuario("Ana", "Tester", "ana@mail.com", "Hola123_", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        var noti1 = new Notificacion("Vista", usuario.Email);
        noti1.MarcarComoVista();
        var noti2 = new Notificacion("No vista", usuario.Email);

        _context.Notificaciones.AddRange(noti1, noti2);
        _context.SaveChanges();

        var resultado = _repositorio.EncontrarLista(n => n.UsuarioEmail == usuario.Email && !n.Vista);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("No vista", resultado[0].Mensaje);
    }

    [TestMethod]
    public void MarcarComoVista_DeberiaActualizarEstado()
    {
        var usuario = new Usuario("Pepe", "Prueba", "pepe@mail.com", "Hola123_", DateTime.Today.AddYears(-28), RolUsuario.MiembroProyecto);
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        var noti = new Notificacion("Verificar vista", usuario.Email);
        _context.Notificaciones.Add(noti);
        _context.SaveChanges();

        noti.Vista = true;
        _repositorio.Actualizar(noti);
        _context.SaveChanges();

        var notiActualizada = _context.Notificaciones.First(n => n.Id == noti.Id);

        Assert.IsTrue(notiActualizada.Vista);
    }
}

