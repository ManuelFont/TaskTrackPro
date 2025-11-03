using Backend.Dominio;

namespace TestBackend.TestDominio;


[TestClass]
public class UsuarioTest
{
    
    [TestMethod]
    public void SetNombre_ModificaNombreCorrectamente()
    {
        var usuario = new Usuario("Juan", "Pérez", "juan@mail.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        usuario.Nombre = "Carlos";
        Assert.AreEqual("Carlos", usuario.Nombre);
    }

    [TestMethod]
    public void SetApellido_ModificaApellidoCorrectamente()
    {
        var usuario = new Usuario("Juan", "Pérez", "juan@mail.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        usuario.Apellido = "García";
        Assert.AreEqual("García", usuario.Apellido);
    }

    [TestMethod]
    public void SetEmail_ModificaEmailCorrectamente()
    {
        var usuario = new Usuario("Juan", "Pérez", "juan@mail.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        usuario.Email = "nuevo@mail.com";
        Assert.AreEqual("nuevo@mail.com", usuario.Email);
    }

    [TestMethod]
    public void SetFechaNacimiento_ModificaFechaCorrectamente()
    {
        var usuario = new Usuario("Juan", "Pérez", "juan@mail.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        var nuevaFecha = new DateTime(2000, 5, 20);
        usuario.FechaNacimiento = nuevaFecha;
        Assert.AreEqual(nuevaFecha, usuario.FechaNacimiento);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirNombreVacio()
    {
        new Usuario("", "Apellido", "mail@mail.com", "Contrasena123", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirEmailInvaldio()
    {
        Usuario u = new Usuario("Nombre", "Apellido", "", "Pass123@", DateTime.Today, RolUsuario.MiembroProyecto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirApellidoVacio()
    {
        new Usuario("Nombre", "", "mail@gmail.com", "Contrasena123", new DateTime(2000, 1, 1),
            RolUsuario.MiembroProyecto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirContraseñaSinMayuscula()
    {
        new Usuario("Test", "User", "user@mail.com", "contraseña1@", new DateTime(2000, 1, 1), RolUsuario.AdminSistema);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirContraseñaSinMinuscula()
    {
        new Usuario("Test", "User", "mail@mail.com", "PASSWORD1@", new DateTime(2000, 1, 1), RolUsuario.AdminSistema);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirContraseñaSinNumero()
    {
        new Usuario("Test", "User", "mail@mail.com", "Password@", new DateTime(2000, 1, 1), RolUsuario.AdminSistema);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirContraseñaSinCaracterEspecial()
    {
        new Usuario("Test", "User", "mail@mail.com", "Password1", new DateTime(2000, 1, 1), RolUsuario.AdminSistema);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirContraseñaConMenosDe8Caracteres()
    {
        new Usuario("Test", "User", "mail@mail.com", "P1@a", new DateTime(2000, 1, 1), RolUsuario.AdminSistema);
    }

    [TestMethod]
    public void DebePermitirContraseñaValida()
    {
        Usuario u = new Usuario("Test", "User", "mail@mail.com", "Password1@", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
        Assert.IsNotNull(u);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirUsuarioMenorDeEdad()
    {
        DateTime fechaMenor = DateTime.Today.AddYears(-17);
        new Usuario("Test", "User", "test@mail.com", "Password1@", fechaMenor, RolUsuario.MiembroProyecto);
    }

    [TestMethod]
    public void DebePermitirUsuarioMayorDeEdad()
    {
        DateTime fechaValida = DateTime.Today.AddYears(-20);

        Usuario usuario = new Usuario("Test", "User", "test@mail.com", "Password1@", fechaValida,
            RolUsuario.AdminProyecto);

        Assert.IsNotNull(usuario);
        Assert.AreEqual("test@mail.com", usuario.Email);
    }



    [TestMethod]
    public void ElProyectoSeAgregaCorrectamenteALaListaDeProyectos()
    {
        Proyecto p = new Proyecto("Proyecto 1", "Descripción 1", DateTime.Now);
        Usuario u = new Usuario("Test", "User", "mail@mail.com", "Password1@", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
        u.AgregarProyecto(p);
        Proyecto p2 = u.BuscarProyectoPorNombre("Proyecto 1");
        Assert.AreEqual(p2.Nombre, p.Nombre);
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoDebePermitirRolInvalido()
    {
        RolUsuario rolInvalido = (RolUsuario)999;
        new Usuario("Test", "User", "mail@mail.com", "Password1@", DateTime.Today.AddYears(-20), rolInvalido);
    }

    [TestMethod]
    public void DebeIndicarQueUsuarioTieneRol()
    {
        Usuario usuario = new Usuario("Test", "User", "user@mail.com", "Password1@", DateTime.Today.AddYears(-20),
            RolUsuario.MiembroProyecto);
        Assert.IsTrue(usuario.TieneRol(RolUsuario.MiembroProyecto));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NoDebePermitirAgregarRolDuplicado()
    {
        Usuario usuario = new Usuario("Test", "User", "mail@mail.com", "Password1@", DateTime.Today.AddYears(-20),
            RolUsuario.AdminSistema);
        usuario.AgregarRol(RolUsuario.AdminSistema);
    }

    [TestMethod]
    public void DebeEliminarRolCorrectamente()
    {
        Usuario usuario = new Usuario("Test", "User", "user@mail.com", "Password1@", DateTime.Today.AddYears(-20),
            RolUsuario.AdminSistema);
        usuario.EliminarRol(RolUsuario.AdminSistema);
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdminSistema));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NoDebePermitirEliminarRolNoExistente()
    {
        Usuario usuario = new Usuario("Test", "User", "user@mail.com", "Password1@", DateTime.Today.AddYears(-20),
            RolUsuario.MiembroProyecto);
        usuario.EliminarRol(RolUsuario.AdminSistema);
    }

    [TestMethod]
    public void ActualizarDatosDebeActualizarCamposCorrectamente()
    {
        var usuario = new Usuario("Nombre", "Apellido", "original@mail.com", "Pass123@", new DateTime(1995, 5, 5),
            RolUsuario.MiembroProyecto);

        var nuevoNombre = "NuevoNombre";
        var nuevoApellido = "NuevoApellido";
        var nuevoEmail = "nuevo@mail.com";
        var nuevaContrasena = "NewPass123@";
        var nuevaFechaNacimiento = new DateTime(2000, 1, 1);

        usuario.ActualizarDatos(nuevoNombre, nuevoApellido, nuevoEmail, nuevaContrasena, nuevaFechaNacimiento);

        Assert.AreEqual(nuevoNombre, usuario.Nombre);
        Assert.AreEqual(nuevoApellido, usuario.Apellido);
        Assert.AreEqual(nuevoEmail, usuario.Email);
        Assert.AreEqual(nuevaContrasena, usuario.Password);
        Assert.AreEqual(nuevaFechaNacimiento, usuario.FechaNacimiento);
    }
    [TestMethod]
    public void AgregarProyecto_ProyectoDuplicado_LanzaExcepcionConMensajeCorrecto()
    {
        var usuario = new Usuario("Nombre", "Apellido", "test@correo.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.MiembroProyecto);
        Proyecto p = new Proyecto("Proyecto 1", "Descripción 1", DateTime.Now);
        usuario.AgregarProyecto(p);

        try
        {
            usuario.AgregarProyecto(p);
            Assert.Fail("Se esperaba una excepción pero no se lanzó.");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Ya existe una proyecto con ese título", ex.Message);
        }
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_EmailVacio_LanzaExcepcion()
    {
        var usuario = new Usuario("Nombre", "Apellido", "", "Password123", DateTime.Now.AddYears(-18), RolUsuario.AdminProyecto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_EmailNulo_LanzaExcepcion()
    {
        var usuario = new Usuario("Nombre", "Apellido", null, "Password123", DateTime.Now.AddYears(-18), RolUsuario.AdminProyecto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_EmailInvalido_LanzaExcepcion()
    {
        var usuario = new Usuario("Nombre", "Apellido", "email-invalido", "Password123", DateTime.Now.AddYears(-18), RolUsuario.AdminProyecto);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_EmailConFormatoInvalido_LanzaExcepcion()
    {
        var usuario = new Usuario(
            "Nombre",
            "Apellido",
            "correo con espacios",  
            "Password123.",
            new DateTime(2000, 1, 1),
            RolUsuario.AdminProyecto
        );
    }
    
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarProyectoPorNombre_ProyectoNoExiste_LanzaExcepcion()
    {
        var usuario = new Usuario("Nombre", "Apellido", "test@correo.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.MiembroProyecto);
        usuario.BuscarProyectoPorNombre("Proyecto Inexistente");
    }
    
    [TestMethod]
    public void RedefinrContraseñaErronea_LanzaArgumentExceptionConMensajeCorrecto()
    {
        Usuario usuario = new Usuario(
            "Nombre",
            "Apellido",
            "correo@valido.com",
            "Password123.",
            new DateTime(2000, 1, 1),
            RolUsuario.AdminProyecto
        );

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            usuario.RedefinirContraseña("123")
        );

        Assert.AreEqual("La nueva contraseña no cumple con los requisitos.", ex.Message);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AgregarRol_RolDuplicado_LanzaExcepcion()
    {
        Usuario usuario = new Usuario(
            "Nombre", 
            "Apellido", 
            "test@correo.com", 
            "Password123.", 
            new DateTime(2000, 1, 1), 
            RolUsuario.AdminProyecto
        );

        usuario.AgregarRol(RolUsuario.AdminProyecto);
    }
    
    [TestMethod]
    public void SiNoTieneElRolSeAsignaCorrectamente()
    {
        Usuario usuario = new Usuario("Nombre", "Apellido", "test@correo.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.MiembroProyecto); 
        usuario.AgregarRol(RolUsuario.AdminSistema);
    }

    [TestMethod]
    public void SettearProyecto()
    {
        Usuario usuario = new Usuario("Nombre", "Apellido", "test@correo.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.MiembroProyecto); 
        List<Proyecto> proyectos = new List<Proyecto>();
        Proyecto p = new Proyecto("Proyecto 1", "Descripción 1", DateTime.Now);
        proyectos.Add(p);
        usuario.ListaProyectos = proyectos;
    }

    [TestMethod]
    public void SettearYGetRolUsuario()
    {
        Usuario usuario = new Usuario("Nombre", "Apellido", "test@correo.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.MiembroProyecto);
        List<RolUsuario> roles = usuario.Roles;
        List<RolUsuario> nueva = new List<RolUsuario>();
        usuario.Roles = nueva;
    }
    
}





