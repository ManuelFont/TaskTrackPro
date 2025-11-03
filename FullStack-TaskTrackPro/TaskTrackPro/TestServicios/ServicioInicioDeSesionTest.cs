using Backend.Dominio;
using DataAccess;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicios;

namespace TestServicios;

[TestClass]
public class ServicioInicioDeSesionTest
{
    private SqlContext CrearContextoInMemory()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid()) 
            .Options;

        return new SqlContext(options);
    }

    [TestMethod]
    public void Login_ConCredencialesValidas_DeberiaRetornarTrue()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var servicio = new ServicioInicioDeSesion(repo);

        var usuario = new Usuario("Guille", "Test", "guille@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        repo.Agregar(usuario);

        var loginDto = new LoginDto
        {
            Email = "guille@correo.com",
            Password = "Password123."
        };

        var resultado = servicio.Login(loginDto);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Login_EmailIncorrecto_DeberiaRetornarFalse()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var servicio = new ServicioInicioDeSesion(repo);

        var usuario = new Usuario("Ana", "Tester", "ana@correo.com", "Password123.", new DateTime(1991, 1, 1), RolUsuario.MiembroProyecto);
        repo.Agregar(usuario);

        var loginDto = new LoginDto
        {
            Email = "otro@correo.com",
            Password = "Password123."
        };

        var resultado = servicio.Login(loginDto);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Login_ContrasenaIncorrecta_DeberiaRetornarFalse()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var servicio = new ServicioInicioDeSesion(repo);
        
        var usuario = new Usuario(
            "Luis", 
            "Test", 
            "luis@correo.com", 
            "CorrectPass123.", 
            new DateTime(1992, 1, 1), 
            RolUsuario.MiembroProyecto
        );
        repo.Agregar(usuario);
        
        var todos = context.Usuarios.ToList();
        Assert.AreEqual(1, todos.Count);
        
        var loginDto = new LoginDto
        {
            Email = "luis@correo.com",
            Password = "Incorrecta"
        };

        var resultado = servicio.Login(loginDto);
        
        Assert.IsFalse(resultado);
    }

}


