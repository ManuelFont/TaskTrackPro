using Backend.Dominio;
using DataAccess;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Servicios;

namespace TestServicios;
[TestClass]
public class ServicioUsuarioTest
{
    private RepositorioUsuarios _repo;
    private ServicioUsuario _servicio;
    private ServicioNotificaciones _servicioNotificaciones;


    private SqlContext CrearContextoInMemory()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        return new SqlContext(options);
    }
    [TestInitialize]
    public void Setup()
    {
        var context = CrearContextoInMemory();
        _repo = new RepositorioUsuarios(context);

        var usuario1 = new Usuario("Ana", "Pérez", "ana@prueba.com", "Pass123@.", DateTime.Today.AddYears(-18), RolUsuario.AdminProyecto);
        var usuario2 = new Usuario("Luis", "García", "luis@prueba.com", ".Pass123@.", DateTime.Today.AddYears(-18), RolUsuario.MiembroProyecto);

        _repo.Agregar(usuario1);
        _repo.Agregar(usuario2);

        var repoNotificaciones = new RepositorioNotificaciones(context);
        _servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);

        _servicio = new ServicioUsuario(_repo, _servicioNotificaciones);
    }
    [TestMethod]
    public void UsuarioActivoEsAdminProyecto_CuandoUsuarioEsAdminProyecto_RetornaTrue()
    {
        var login = new LoginDto { Email = "ana@prueba.com", Password = "Pass123@." };
        _servicio.SetUsuario(login);
        bool esAdminProyecto = _servicio.UsuarioActivoEsAdminProyecto();
        Assert.IsTrue(esAdminProyecto);
    }

    [TestMethod]
    public void UsuarioActivoEsAdminProyecto_CuandoUsuarioNoEsAdminProyecto_RetornaFalse()
    {
        var login = new LoginDto { Email = "luis@prueba.com", Password = ".Pass123@." };
        _servicio.SetUsuario(login);
        bool esAdminProyecto = _servicio.UsuarioActivoEsAdminProyecto();
        Assert.IsFalse(esAdminProyecto);
    }
    
  [TestMethod]
  public void EliminarUsuario_EmailExistente_DeberiaEliminarUsuarioDeLaBase()
  {
      using var context = CrearContextoInMemory();
      var repo = new RepositorioUsuarios(context);
      var repoNotificaciones = new RepositorioNotificaciones(context); 
      var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
      var servicio = new ServicioUsuario(repo, servicioNotificaciones);
  
      var usuario = new Usuario(
          "María", "López",
          "maria@prueba.com",
          "Pass123!",
          DateTime.Today.AddYears(-25),
          RolUsuario.MiembroProyecto
      );
  
      repo.Agregar(usuario);
  
      servicio.EliminarUsuario(usuario.Email);
  
      var usuariosRestantes = repo.EncontrarLista(u => u.Email == "maria@prueba.com");
      Assert.AreEqual(0, usuariosRestantes.Count, "El usuario debería haber sido eliminado de la base de datos.");
  }


    [TestMethod]
    public void EliminarUsuario_UsuarioExistente_SeEliminaCorrectamente()
    {
        var usuario = new Usuario("Juan", "Pérez", "juan@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        _repo.Agregar(usuario);
    
        _servicio.EliminarUsuario("juan@correo.com");
    
        var usuariosEncontrados = _repo.EncontrarLista(u => u.Email == "juan@correo.com");
        Assert.AreEqual(0, usuariosEncontrados.Count, "El usuario debería haber sido eliminado.");
    }

    
    [TestMethod]
    public void TraerTodosLosUsuariosMiembros_DeberiaRetornarSoloUsuariosConRolMiembro()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var u1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        var u2 = new Usuario("Juan", "Perez", "juan@correo.com", "Password123.", new DateTime(1980, 1, 1), RolUsuario.AdminSistema);

        repo.Agregar(u1);
        repo.Agregar(u2);

        var resultado = servicio.TraerTodosLosUsuariosMiembros();

        Assert.AreEqual(1, resultado.Count); 
        Assert.IsTrue(resultado.Any(u => u.Email == "ana@correo.com"));
        Assert.IsFalse(resultado.Any(u => u.Email == "juan@correo.com"));
    }

    
    [TestMethod]
    public void AgregarProyectoUsuarioLogueado_ProyectoSeAgregaCorrectamente()
    {
        var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today);

        var usuario = new Usuario(
            "Juan", "Pérez", "juan@correo.com", "Password123.", new DateTime(1990, 1, 1),
            RolUsuario.MiembroProyecto
        );
        _servicio.UsuarioLogueado = usuario;

        _servicio.AgregarProyectoUsuarioLogueado(proyecto);

        Assert.IsTrue(usuario.ListaProyectos.Any(p => p.Nombre == "Proyecto Test"));
    }
    
    [TestMethod]
    public void TraerTodosLosUsuariosMiembros_DevuelveSoloUsuariosConRolMiembro()
    {
        var resultado = _servicio.TraerTodosLosUsuariosMiembros();
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("luis@prueba.com", resultado[0].Email);
        Assert.AreEqual("Luis", resultado[0].Nombre);
        Assert.AreEqual("García", resultado[0].Apellido);
    }
    
    [TestMethod]
    public void UsuarioTieneRolMiembro_UsuarioConRolMiembro_RetornaTrue()
    {
        var resultado = _servicio.UsuarioTieneRolMiembro("luis@prueba.com");
        Assert.IsTrue(resultado);
    }

    
    [TestMethod]
    public void CambiarRolUsuario_RolNuevo_SeAgregaCorrectamente()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Mario", "Sosa", "mario@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.AdminProyecto);
        repo.Agregar(usuario);

        servicio.CambiarRolUsuario("mario@correo.com", (int)RolUsuario.MiembroProyecto);

        Assert.IsTrue(usuario.Roles.Contains(RolUsuario.MiembroProyecto));
        Assert.AreEqual(2, usuario.Roles.Count); 
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CambiarRolUsuario_RolDuplicado_LanzaExcepcion()
    {

        var usuario = new Usuario("Carla", "Gomez", "carla@correo.com", "Password123.", new DateTime(1985, 1, 1), RolUsuario.AdminSistema);
        _repo.Agregar(usuario);
        _servicio.CambiarRolUsuario("carla@correo.com", (int)RolUsuario.AdminSistema);
    }
    
    [TestMethod]
    public void AgregarRol_RolNuevo_LoAgregaCorrectamente()
    {
        var usuario = new Usuario("Ana", "Lopez", "ana@mail.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);

        usuario.AgregarRol(RolUsuario.AdminSistema);

        Assert.IsTrue(usuario.Roles.Contains(RolUsuario.AdminSistema));
    }

    
    [TestMethod]
    public void GenerarContrasenaAleatoria_GeneraContrasenaValida()
    {
        int longitud = 10;

        string contraseña = _servicio.GenerarContrasenaAleatoria(longitud); 
        Assert.AreEqual(longitud, contraseña.Length, "La longitud de la contraseña no es la esperada.");
        Assert.IsTrue(contraseña.Any(char.IsUpper), "Debe contener al menos una letra mayúscula.");
        Assert.IsTrue(contraseña.Any(char.IsLower), "Debe contener al menos una letra minúscula.");
        Assert.IsTrue(contraseña.Any(char.IsDigit), "Debe contener al menos un número.");
        Assert.IsTrue(contraseña.Any(c => "!@#$%&*".Contains(c)), "Debe contener al menos un carácter especial.");
    }

    [TestMethod]
    public void AutogenerarContrasena_CambiaLaContrasenaDelUsuario()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Pedro", "Luna", "pedro@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        repo.Agregar(usuario);

        string contraseñaAnterior = usuario.Password; 

        servicio.AutogenerarContrasena("pedro@correo.com");

        Assert.AreNotEqual(contraseñaAnterior, usuario.Password); 
        Assert.IsTrue(usuario.Password.Length >= 8); 
    }
    
    [TestMethod]
    public void ReiniciarContrasena_UsuarioExistente_ContrasenaSeActualiza()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Laura", "Martinez", "laura@correo.com", "Password123.", new DateTime(1992, 1, 1), RolUsuario.MiembroProyecto);
        repo.Agregar(usuario);

        servicio.ReiniciarContrasena("laura@correo.com");

        Assert.AreEqual(Usuario.HashSHA256("Change123."), usuario.Password); 
    }

    
    [TestMethod]
    public void ListarRolesDisponiblesParaGestionado_DevuelveRolesQueNoTiene()
    {
        var usuario = new Usuario(
            "Guille", "Test", "guille@correo.com", "Password123.", new DateTime(1990, 1, 1),
            RolUsuario.AdminSistema
        );
        usuario.AgregarRol(RolUsuario.MiembroProyecto);

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);
        UsuarioDto u = new UsuarioDto();
        u.Email = usuario.Email;
        u.Nombre = usuario.Nombre;
        u.Apellido = usuario.Apellido;
        
        
        servicio.UsuarioGestionado = u; 
        repo.Agregar(usuario); 

        var rolesDisponibles = servicio.ListarRolesDisponiblesParaGestionado();

        var rolesActuales = usuario.Roles.Select(r => (int)r).ToHashSet();
        var todos = Enum.GetValues<RolUsuario>().Cast<int>().ToHashSet();
        var esperados = todos.Except(rolesActuales).ToList();

        var devueltos = rolesDisponibles.Select(r => r.Id).ToList();
        CollectionAssert.AreEquivalent(esperados, devueltos);
    }
    
    [TestMethod]
    public void TraerUsuarioPorEmail_UsuarioExistente_DevuelveDtoCorrecto()
    {
        var usuario = new Usuario(
            "Guille", "Test", "guille@correo.com", "Password123.", new DateTime(1990, 1, 1),
            RolUsuario.AdminSistema
        );

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        repo.Agregar(usuario); 

        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var dto = servicio.TraerUsuarioPorEmail("guille@correo.com");

        Assert.AreEqual("Guille", dto.Nombre);
        Assert.AreEqual("Test", dto.Apellido);
        Assert.AreEqual("guille@correo.com", dto.Email);
    }
    
    [TestMethod]
    public void SetUsuarioAGestionar_AsignaCorrectamenteElUsuarioDto()
    {
        var usuarioDto = new UsuarioDto
        {
            Nombre = "Guille",
            Apellido = "Test",
            Email = "guille@correo.com"
        };

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones); 

        servicio.SetUsuarioAGestionar(usuarioDto);

        Assert.IsNotNull(servicio.UsuarioGestionado);
        Assert.AreEqual("Guille", servicio.UsuarioGestionado.Nombre);
        Assert.AreEqual("Test", servicio.UsuarioGestionado.Apellido);
        Assert.AreEqual("guille@correo.com", servicio.UsuarioGestionado.Email);
    }
    
    [TestMethod]
    public void Limpiar_EstableceUsuarioLogueadoEnNull()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones); 

        var usuario = new Usuario(
            "Guille", "Test", "guille@correo.com", "Password123.", new DateTime(1990, 1, 1),
            RolUsuario.MiembroProyecto
        );

        servicio.UsuarioLogueado = usuario;
        Assert.IsNotNull(servicio.UsuarioLogueado);
        servicio.Limpiar();
        Assert.IsNull(servicio.UsuarioLogueado);
    }
    
    [TestMethod]
    public void CrearUsuario_ConDatosValidos_CreaYAgregaUsuarioCorrectamente()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);       
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var dto = new CrearUsuarioDto
        {
            Nombre = "Guille",
            Apellido = "Tester",
            Email = "guille@correo.com",
            Password = "Password123.",
            FechaNacimiento = new DateTime(1990, 1, 1),
            RolId = 2 
        };

        servicio.CrearUsuario(dto);
        var usuario = repo.EncontrarElemento(u => u.Email == "guille@correo.com");

        Assert.IsNotNull(usuario);
        Assert.AreEqual("Guille", usuario.Nombre);
        Assert.AreEqual("Tester", usuario.Apellido);
        Assert.AreEqual("guille@correo.com", usuario.Email);
        Assert.AreEqual(RolUsuario.AdminProyecto, usuario.Roles.First());
    }

    [TestMethod]
    public void ListarProyectosUsuarioLogueado_DevuelveProyectosConMiembrosCorrectos()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var miembro1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        var miembro2 = new Usuario("Luis", "Gomez", "luis@correo.com", "Password123.", new DateTime(1991, 1, 1), RolUsuario.MiembroProyecto);

        var proyecto = new Proyecto("Proyecto Test", "Descripción test", DateTime.Today);
        proyecto.ListaUsuarios.Add(miembro1);
        proyecto.ListaUsuarios.Add(miembro2);

        var usuario = new Usuario("Guille", "Test", "guille@correo.com", "Password123.", new DateTime(1988, 1, 1), RolUsuario.AdminProyecto);
        usuario.AgregarProyecto(proyecto);

        servicio.UsuarioLogueado = usuario;

        
        var resultado = servicio.ListarProyectosUsuarioLogueado();

        Assert.AreEqual(1, resultado.Count);
        var dto = resultado.First();

        Assert.AreEqual("Proyecto Test", dto.NombreProyecto);
        Assert.AreEqual("Descripción test", dto.DescripcionProyecto);
        Assert.AreEqual(2, dto.MiembrosProyecto.Count);
        Assert.IsTrue(dto.MiembrosProyecto.Any(u => u.Email == "ana@correo.com"));
        Assert.IsTrue(dto.MiembrosProyecto.Any(u => u.Email == "luis@correo.com"));
    }

    [TestMethod]
    public void TraerTodosLosUsuarios_DevuelveListaDeUsuarioDto()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        var usuario2 = new Usuario("Luis", "Gomez", "luis@correo.com", "Password123.", new DateTime(1985, 1, 1), RolUsuario.MiembroProyecto);

        repo.Agregar(usuario1);
        repo.Agregar(usuario2);

        var resultado = servicio.TraerTodosLosUsuarios();

        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.Any(u => u.Email == "ana@correo.com" && u.Nombre == "Ana" && u.Apellido == "Lopez"));
        Assert.IsTrue(resultado.Any(u => u.Email == "luis@correo.com" && u.Nombre == "Luis" && u.Apellido == "Gomez"));
    }
    
    [TestMethod]
    public void UsuarioActivoEsAdminSistema_UsuarioEsAdminSistema_DevuelveTrue()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Guille", "Admin", "admin@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.AdminSistema);
        servicio.UsuarioLogueado = usuario;

        bool esAdmin = servicio.UsuarioActivoEsAdminSistema();

        Assert.IsTrue(esAdmin);
    }
    
    [TestMethod]
    public void UsuarioActivoEsAdminSistema_UsuarioNoEsAdminSistema_DevuelveFalse()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Ana", "Perez", "ana@correo.com", "Password123.", new DateTime(1995, 1, 1), RolUsuario.MiembroProyecto);
        servicio.UsuarioLogueado = usuario;

        bool esAdmin = servicio.UsuarioActivoEsAdminSistema();

        Assert.IsFalse(esAdmin);
    }

    [TestMethod]
    public void CrearUsuario_RolId1_CreaUsuarioConRolAdminSistema()
    {
        var dto = new CrearUsuarioDto
        {
            Nombre = "Guille",
            Apellido = "Admin",
            Email = "admin@correo.com",
            Password = "Password123.",
            FechaNacimiento = new DateTime(1990, 1, 1),
            RolId = 1
        };

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        servicio.CrearUsuario(dto);

        var creado = repo.EncontrarElemento(u => u.Email == "admin@correo.com");
        Assert.IsNotNull(creado);
        Assert.IsTrue(creado.Roles.Contains(RolUsuario.AdminSistema));
    }

    
    [TestMethod]
    public void CrearUsuario_Rol4_CreaUsuarioConRolLiderProyecto()
    {
        var dto = new CrearUsuarioDto
        {
            Nombre = "Luis",
            Apellido = "Miembro",
            Email = "miembro@correo.com",
            Password = "Password123.",
            FechaNacimiento = new DateTime(1988, 1, 1),
            RolId = 4
        };

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        servicio.CrearUsuario(dto);

        var creado = repo.EncontrarElemento(u => u.Email == "miembro@correo.com");
        Assert.IsTrue(creado.Roles.Contains(RolUsuario.LiderProyecto));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CambiarPasswordUsuario_ContraseniaActualIncorrecta_LanzaExcepcion()
    {
        var usuario = new Usuario("Ana", "Perez", "ana@mail.com", "Clave123!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        repo.Agregar(usuario);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);
        servicio.UsuarioLogueado = usuario;

        var dto = new CambiarPasswordDto
        {
            ContraseniaActual = "otraClave!",
            NuevaContrasenia = "NuevaClave123!",
            ConfirmarContrasenia = "NuevaClave123!"
        };

        servicio.CambiarPasswordUsuario(dto);
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CambiarPasswordUsuario_ContraseniaNuevaIgual_LanzaExcepcion()
    {
        var usuario = new Usuario("Ana", "Perez", "ana@mail.com", "Clave123!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        repo.Agregar(usuario);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);
        servicio.UsuarioLogueado = usuario;

        var dto = new CambiarPasswordDto
        {
            ContraseniaActual = "Clave123!",
            NuevaContrasenia = "Clave123!",
            ConfirmarContrasenia = "Clave123!"
        };

        servicio.CambiarPasswordUsuario(dto);
    }
    [TestMethod]
    public void CambiarPasswordUsuario_ContraseniaValida_CambiaContrasenia()
    {
        var usuario = new Usuario("Ana", "Perez", "ana@mail.com", "Clave123!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        repo.Agregar(usuario);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);
        servicio.UsuarioLogueado = usuario;

        var dto = new CambiarPasswordDto
        {
            ContraseniaActual = "Clave123!",
            NuevaContrasenia = "Nueva123!",
            ConfirmarContrasenia = "Nueva123!"
        };

        servicio.CambiarPasswordUsuario(dto);

        Assert.AreEqual(Usuario.HashSHA256("Nueva123!"), usuario.Password);

    }
    [TestMethod]
    public void ObtenerProyectosDeUsuarioLogueado_DevuelveProyectosCorrectamente()
    {
        var usuario = new Usuario("Lucas", "Fernandez", "lucas@mail.com", "Clave123!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);

        var proyecto1 = new Proyecto("Proyecto A", "desc", DateTime.Today);
        var proyecto2 = new Proyecto("Proyecto B", "desc", DateTime.Today);
        
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        var usuarioLider2 = new Usuario("Lider", "Test", "lider3@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto1.AsignarLider(usuarioLider);
        proyecto2.AsignarLider(usuarioLider2);


        _repo.Agregar(usuarioLider);
        _repo.Agregar(usuarioLider2);


        usuario.AgregarProyecto(proyecto1);
        usuario.AgregarProyecto(proyecto2);

        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        repo.Agregar(usuario);

        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);
        servicio.UsuarioLogueado = usuario;

        var resultado = servicio.ObtenerProyectosDeUsuarioLogueado();

        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.Any(p => p.NombreProyecto == "Proyecto A"));
        Assert.IsTrue(resultado.Any(p => p.NombreProyecto == "Proyecto B"));
    }

    [TestMethod]
    public void ListarProyectosUsuarioLogueado_ListaProyectosEsNull_RetornaListaVacia()
    {
        var usuario = new Usuario("Test", "User", "test@correo.com", "Pass123@.", DateTime.Today.AddYears(-20), RolUsuario.AdminProyecto)
        {
            ListaProyectos = null 
        };
        _servicio.UsuarioLogueado = usuario;
        var resultado = _servicio.ListarProyectosUsuarioLogueado();
        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }
    [TestMethod]
    public void ListarProyectosUsuarioLogueado_ConUnProyecto_RetornaProyectoDto()
    {
        var proyecto = new Proyecto("Sistema", "Sistema de gestión", DateTime.Today);
        var usuario = new Usuario("Test", "User", "test@correo.com", "Pass123@.", DateTime.Today.AddYears(-20), RolUsuario.AdminProyecto);

        proyecto.AgregarMiembroAlProyecto(usuario); 
        usuario.ListaProyectos.Add(proyecto);
        _servicio.UsuarioLogueado = usuario;

        var resultado = _servicio.ListarProyectosUsuarioLogueado();

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Sistema", resultado[0].NombreProyecto);
        Assert.AreEqual("Sistema de gestión", resultado[0].DescripcionProyecto);
        Assert.AreEqual(1, resultado[0].MiembrosProyecto.Count);
        Assert.AreEqual("Test", resultado[0].MiembrosProyecto[0].Nombre);
    }
    [TestMethod]
    public void ListarProyectosUsuarioLogueado_ConUnProyectoYUnUsuario_RetornaDtoCorrecto()
    {
        var usuario = new Usuario("Test", "User", "test@correo.com", "Pass123@.", DateTime.Today.AddYears(-20), RolUsuario.AdminProyecto);
        var proyecto = new Proyecto("TaskTrack", "Gestión de tareas", DateTime.Today);
        proyecto.AgregarMiembroAlProyecto(usuario); 
        usuario.ListaProyectos.Add(proyecto); 
        _servicio.UsuarioLogueado = usuario;

        var resultado = _servicio.ListarProyectosUsuarioLogueado();

        Assert.AreEqual(1, resultado.Count);
        var dto = resultado[0];
        Assert.AreEqual("TaskTrack", dto.NombreProyecto);
        Assert.AreEqual("Gestión de tareas", dto.DescripcionProyecto);
        Assert.AreEqual(proyecto.FechaInicioEstimada, dto.FechaInicio);
        Assert.AreEqual(1, dto.MiembrosProyecto.Count);
        Assert.AreEqual("Test", dto.MiembrosProyecto[0].Nombre);
    }
    [TestMethod]
    public void ListarProyectosUsuarioLogueado_ProyectoSinUsuarios_RetornaDtoConListaVacia()
    {
        var proyecto = new Proyecto("Proyecto A", "Desc", DateTime.Today);
        var usuario = new Usuario("Juan", "Pérez", "juan@correo.com", "Pass123@.", DateTime.Today.AddYears(-20), RolUsuario.AdminProyecto);

        usuario.ListaProyectos.Add(proyecto);
        _servicio.UsuarioLogueado = usuario;

        var resultado = _servicio.ListarProyectosUsuarioLogueado();

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(0, resultado[0].MiembrosProyecto.Count);
    }
    
    [TestMethod]
    public void ListarProyectosUsuarioLogueado_ProyectoConUnUsuario_MapeaCorrectamente()
    {
        var usuario = new Usuario("Ana", "López", "ana@correo.com", "Pass123@.", DateTime.Today.AddYears(-20), RolUsuario.MiembroProyecto);
        var proyecto = new Proyecto("Proyecto B", "Descripción", DateTime.Today);

        proyecto.AgregarMiembroAlProyecto(usuario);
        usuario.ListaProyectos.Add(proyecto);

        _servicio.UsuarioLogueado = usuario;

        var resultado = _servicio.ListarProyectosUsuarioLogueado();

        Assert.AreEqual(1, resultado.Count);
        var dto = resultado[0];

        Assert.AreEqual("Proyecto B", dto.NombreProyecto);
        Assert.AreEqual(1, dto.MiembrosProyecto.Count);
        Assert.AreEqual("Ana", dto.MiembrosProyecto[0].Nombre);
        Assert.AreEqual("López", dto.MiembrosProyecto[0].Apellido);
        Assert.AreEqual("ana@correo.com", dto.MiembrosProyecto[0].Email);
    }
    [TestMethod]
    public void UsuarioActivoEsLiderProyecto_DevuelveTrue()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Ana", "Perez", "ana@correo.com", "Password123.", new DateTime(1995, 1, 1), RolUsuario.LiderProyecto);
        servicio.UsuarioLogueado = usuario;

        bool esLider = servicio.UsuarioActivoEsLiderProyecto();

        Assert.IsTrue(esLider);
    }
    
    [TestMethod]
    public void UsuarioActivoEsLiderProyecto_DevuelveFalse()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioUsuarios(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones); 
        var servicio = new ServicioUsuario(repo, servicioNotificaciones);

        var usuario = new Usuario("Ana", "Perez", "ana@correo.com", "Password123.", new DateTime(1995, 1, 1), RolUsuario.MiembroProyecto);
        servicio.UsuarioLogueado = usuario;

        bool esLider = servicio.UsuarioActivoEsLiderProyecto();

        Assert.IsFalse(esLider);
    }
}