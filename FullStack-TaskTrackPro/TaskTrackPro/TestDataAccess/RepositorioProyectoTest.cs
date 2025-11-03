using Backend.Dominio;
using DataAccess;

namespace TestDataAccess;

[TestClass]
public class RepositorioProyectosTests
{
    private SqlContext _context;
    private RepositorioProyectos _repositorio;
    private readonly InMemoryAppContextFactory _factory = new();

    [TestInitialize]
    public void Setup()
    {
        string dbName = "TestDb_" + Guid.NewGuid();
        _context = _factory.CreateDbContext(dbName);
        _repositorio = new RepositorioProyectos(_context);
    }

    [TestMethod]
    public void AgregarProyecto_DeberiaGuardarseYRecuperarse()
    {

        var proyecto = new Proyecto("Proyecto", "Descripcion del proyecto", DateTime.Today.AddDays(1));
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        _repositorio.Agregar(proyecto);

        Proyecto resultado = _repositorio.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                            ?? throw new KeyNotFoundException("Proyecto no encontrado");

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Proyecto", resultado.Nombre);
    }

    [TestMethod]
    public void TraerProyectoPorNombre_ProyectoNoExiste_DeberiaRetornarNull()
    {
        Proyecto? resultado = _repositorio.EncontrarElemento(p => p.Nombre == "Inexistente");
        Assert.IsNull(resultado);
    }
    
    [TestMethod]
    public void AgregarProyectoConUsuariosYTraer_VerificaInclude()
    {

        var usuario = new Usuario("Ana", "Tester", "ana2@mail.com", "Test123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
        var proyecto = new Proyecto("ProyectoUsuarios", "Con usuarios", DateTime.Today.AddDays(1));
        proyecto.AgregarMiembroAlProyecto(usuario);
        var usuarioLider = new Usuario("Lider", "Test", "lider4@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
        _repositorio.Agregar(proyecto);

        Proyecto resultado = _repositorio.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                             ?? throw new KeyNotFoundException("Proyecto no encontrado");

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.ListaUsuarios.Any(u => u.Email == "ana2@mail.com"));
    }
    
    [TestMethod]
    public void AgregarProyectoConTareasYTraer_VerificaInclude()
    {
        var proyecto = new Proyecto("ProyectoTareas", "Con tareas", DateTime.Today.AddDays(1));
        var tarea = new Tarea("x", "hola",2, proyecto);
        proyecto.AgregarTarea(tarea);
        var usuarioLider = new Usuario("Lider", "Test", "lider3@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
        
        _repositorio.Agregar(proyecto);

        Proyecto resultado = _repositorio.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                             ?? throw new KeyNotFoundException("Proyecto no encontrado");

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.ListaDeTareas.Any(t => t.Titulo == "x"));
    }
    
    [TestMethod]
    public void BorrarProyecto_DeberiaEliminarloDeLaBase()
    {
        var proyecto = new Proyecto("ProyectoAEliminar", "Prueba de borrado", DateTime.Today.AddDays(1));
        var usuarioLider = new Usuario("Lider", "Test", "lider5@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
        _repositorio.Agregar(proyecto);

        _repositorio.Eliminar(p => p.Nombre == proyecto.Nombre);

        Proyecto? resultado = _repositorio.EncontrarElemento(p => p.Nombre == proyecto.Nombre);
        Assert.IsNull(resultado);
    }


}
