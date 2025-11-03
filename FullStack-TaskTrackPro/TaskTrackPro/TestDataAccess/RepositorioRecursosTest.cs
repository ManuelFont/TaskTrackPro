using DataAccess;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
namespace TestDataAccess;

[TestClass]
public class RepositorioRecursosTest
{
    private SqlContext _context;
    private RepositorioRecursos _repositorio;
    private readonly InMemoryAppContextFactory _factory = new();

    
    [TestInitialize]
    public void Setup()
    {
        string dbName = "TestDb_" + Guid.NewGuid();
        _context = _factory.CreateDbContext(dbName);
        _repositorio = new RepositorioRecursos(_context);
    }
    
    [TestMethod]
    public void AgregarRecurso()
    { 
        Proyecto proyecto = new Proyecto("Nombre", "Des", DateTime.Today);

        var usuarioLider = new Usuario("Lider", "Test", "lider6@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        proyecto.AsignarLider(usuarioLider);

        var recurso = new Recurso("Recurso1", "Tipo1", "Desc", "Fun", proyecto);
        recurso.AsignarFuncionalidad("C++");
        _repositorio.Agregar(recurso);

        var resultado =  _repositorio.TraerTodos().FirstOrDefault();

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Recurso1", resultado.Nombre);
    }

    [TestMethod]
    public void Find_RetornaListaVaciaSiNadieCumple()
    {
        Proyecto proyecto = new Proyecto("Nombre1", "Des", DateTime.Today);

        var usuarioLider = new Usuario("Lider", "Test", "lider7@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        proyecto.AsignarLider(usuarioLider);
        var recurso = new Recurso("Recurso", "Tipo1", "Desc", "Fun", proyecto);
        recurso.AsignarFuncionalidad("C++");
        _repositorio.Agregar(recurso);
        
        var resultado =  _repositorio.EncontrarLista(r => r.Proyecto == null);
        Assert.AreEqual(resultado.Count, 0);
    }
    
    [TestMethod]
    public void TraerRecursosProyecto_RetornaSoloRecursosDelProyecto()
    {
        var proyecto1 = new Proyecto("Proyecto A", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider24@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        var usuarioLider2 = new Usuario("Lider", "Test", "lider32@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        var proyecto2 = new Proyecto("Proyecto B", "Desc", DateTime.Today);
        proyecto1.AsignarLider(usuarioLider);
        proyecto2.AsignarLider(usuarioLider2);


        var recurso1 = new Recurso("Recurso1", "Tipo1", "Desc1", "Func1", proyecto1);
        var recurso2 = new Recurso("Recurso2", "Tipo2", "Desc2", "Func2", proyecto2);
        var recurso3 = new Recurso("Recurso3", "Tipo3", "Desc3", "Func3", proyecto1);

        _context.Recursos.AddRange(recurso1, recurso2, recurso3);
        _context.SaveChanges();

        var resultado = _repositorio.EncontrarLista(r => r.ProyectoNombre == "Proyecto A");

        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.Any(r => r.Nombre == "Recurso1"));
        Assert.IsTrue(resultado.Any(r => r.Nombre == "Recurso3"));
        Assert.IsFalse(resultado.Any(r => r.Nombre == "Recurso2"));
    }
}