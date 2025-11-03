using Backend.Dominio;
using DataAccess;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Servicios;

namespace TestServicios;

[TestClass]
public class ServicioProyectoTest
{
    private RepositorioProyectos repoProyectos;
    private RepositorioUsuarios repoUsuarios;
    private RepositorioTareas repoTareas;
    private RepositorioNotificaciones repoNotificaciones;
    private ServicioNotificaciones servicioNotificaciones;
    private ServicioUsuario servicioUsuario;
    private ServicioProyecto servicioProyecto;
    private Usuario usuario1;
    private Proyecto proyecto;
    private SqlContext context;

    private string _rutaTemporal = "";
    private string _nombreArchivo = "";
    private string _direccionCompletaJson = "";
    private string _direccionCompletaCsv = "";
    
    private SqlContext CrearContextoInMemory()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        return new SqlContext(options);
    }

    [TestInitialize]
    public void SetUp()
    {
        var context = CrearContextoInMemory();
        repoProyectos = new RepositorioProyectos(context);

        repoTareas = new RepositorioTareas(context);
        repoUsuarios = new RepositorioUsuarios(context); 
        repoNotificaciones = new RepositorioNotificaciones(context);
        servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        

        servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);


        usuario1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
        proyecto = new Proyecto("Proyecto X", "Desc X", DateTime.Today);
        
        var usuarioLider = new Usuario("Lider", "Test", "lider@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuario1);
        repoUsuarios.Agregar(usuarioLider);
        repoProyectos.Agregar(proyecto);
        
        _rutaTemporal = Path.GetTempPath();
        _nombreArchivo = "archivoTest";
        _direccionCompletaJson = _rutaTemporal + "/" + _nombreArchivo + ".json";
            _direccionCompletaCsv = _rutaTemporal + "/" + _nombreArchivo + ".csv";
        
        if (File.Exists(_direccionCompletaJson))
            File.Delete(_direccionCompletaJson);
        
        if (File.Exists(_direccionCompletaCsv))
            File.Delete(_direccionCompletaCsv);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_direccionCompletaJson))
            File.Delete(_direccionCompletaJson);
        
        if (File.Exists(_direccionCompletaCsv))
            File.Delete(_direccionCompletaCsv);
    }

    [TestMethod]
    public void CrearArchivoJson()
    { 
        servicioProyecto.CrearArchivoJson(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaJson), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaJson);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoJson_ProyectoCon1Tarea()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        proyecto.AgregarTarea(t1);
        
        servicioProyecto.CrearArchivoJson(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaJson), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaJson);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoJson_ProyectoCon1Tarea_ConRecurso()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        Recurso r1 = new Recurso("rec1", "tipo1", "des", "fun");
        t1.AgregarRecurso(r1);
        
        proyecto.AgregarTarea(t1);
        
        servicioProyecto.CrearArchivoJson(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaJson), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaJson);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoJson_ProyectoCon2Tareas()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        Tarea t2 = new Tarea("t2", "des", 1, proyecto);
        proyecto.AgregarTarea(t1);
        proyecto.AgregarTarea(t2);
        
        servicioProyecto.CrearArchivoJson(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaJson), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaJson);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoJson_Con2Proyectos()
    {
        Proyecto proyecto2 = new Proyecto("p2", "des", DateTime.Today);
        proyecto2.LiderEmail = "lider@test.com";
        repoProyectos.Agregar(proyecto2);
        
        servicioProyecto.CrearArchivoJson(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaJson), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaJson);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoCsv()
    { 
        servicioProyecto.CrearArchivoCsv(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaCsv), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaCsv);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoCsv_ProyectoCon1Tarea()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        proyecto.AgregarTarea(t1);
        
        servicioProyecto.CrearArchivoCsv(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaCsv), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaCsv);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoCsv_ProyectoCon1Tarea_ConRecurso()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        Recurso r1 = new Recurso("rec1", "tipo1", "des", "fun");
        t1.AgregarRecurso(r1);
        
        proyecto.AgregarTarea(t1);
        
        servicioProyecto.CrearArchivoCsv(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaCsv), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaCsv);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoCsv_ProyectoCon2Tareas()
    {
        Tarea t1 = new Tarea("t1", "des", 1, proyecto);
        Tarea t2 = new Tarea("t2", "des", 1, proyecto);
        proyecto.AgregarTarea(t1);
        proyecto.AgregarTarea(t2);
        
        servicioProyecto.CrearArchivoCsv(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaCsv), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaCsv);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void CrearArchivoCsv_Con2Proyectos()
    {
        Proyecto proyecto2 = new Proyecto("p2", "des", DateTime.Today);
        proyecto2.LiderEmail = "lider@test.com";
        repoProyectos.Agregar(proyecto2);
        
        servicioProyecto.CrearArchivoCsv(_rutaTemporal, _nombreArchivo);
        Assert.IsTrue(File.Exists(_direccionCompletaCsv), $"El archivo no fue creado en {_rutaTemporal}");
        string contenido = File.ReadAllText(_direccionCompletaCsv);
        Assert.IsFalse(string.IsNullOrEmpty(contenido), "El archivo esta vacío");
    }
    
    [TestMethod]
    public void TraerProyectoPorNombre_ProyectoExiste_DevuelveDtoConMiembros()
    {
        Proyecto proyectoGuardado = repoProyectos.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                                    ?? throw new KeyNotFoundException("Proyecto no encontrado");
        proyectoGuardado.AgregarMiembroAlProyecto(usuario1);

        var dto = servicioProyecto.TraerProyectoPorNombre("Proyecto X");

        Assert.AreEqual(2, dto.MiembrosProyecto.Count);
    }


    [TestMethod]
    public void AgregarMiembroAProyecto_AgregaUsuarioAlProyectoYProyectoAlUsuario()
    {
        
        var nuevoUsuario = new Usuario("Lucia", "Martinez", "angela@mail.com", "Passw0rd1!", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
        repoUsuarios.Agregar(nuevoUsuario);
        
        servicioProyecto.AgregarMiembroAProyecto("Proyecto X", "angela@mail.com");

        Assert.IsTrue(proyecto.ListaUsuarios.Contains(nuevoUsuario));
    }

    [TestMethod]
    public void CrearYAgregarTareaAProyecto_TareaValida_SeAgregaAlProyecto()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context);
        var repoTareas = new RepositorioTareas(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        

        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);
        var tareaDto = new TareaDto { Titulo = "Tarea1", Descripcion = "Desc", Duracion = 3 };
        proyecto = new Proyecto("Proyecto X", "Desc X", DateTime.Today);
        
        var usuarioLider = new Usuario("Lider", "Test", "lider@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
repoProyectos.Agregar(proyecto);
        servicioProyecto.CrearYAgregarTareaAProyecto("Proyecto X", tareaDto);

        Proyecto proyectoActual = repoProyectos.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                                    ?? throw new KeyNotFoundException("Proyecto no encontrado");
        Assert.AreEqual(1, proyectoActual.ListaDeTareas.Count);
    }

    [TestMethod]
    public void TraerTareasDeProyecto_DevuelveSoloTareasNoRealizadas()
    {
        using var context = CrearContextoInMemory();
        var repoProyectos = new RepositorioProyectos(context);
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,repoNotificaciones, servicioNotificaciones, repoTareas);
        
        var nuevoProyecto = new Proyecto("Proyecto", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        nuevoProyecto.AsignarLider(usuarioLider);
        repoProyectos.Agregar(nuevoProyecto);

        var t2 = new Tarea("ttt2", "Desc", 2, nuevoProyecto);
        var t3 = new Tarea("ttt3", "Desc", 3, nuevoProyecto);

        t2.MarcarTareaRealizada(DateTime.Today); 
        repoTareas.Agregar(t2);
        repoTareas.Agregar(t3);
        

        Assert.AreEqual("Proyecto", nuevoProyecto.Nombre);

        var tareas = servicioProyecto.TraerTareasDeProyecto("Proyecto");

        Assert.AreEqual(2, tareas.Count);
        Assert.AreEqual("ttt3", tareas[1].Titulo);
    }



    [TestMethod]
    public void MarcarTareaDeProyectoComoRealizada_CambiaEstadoDeLaTarea()
    {
        var nuevoProyecto = new Proyecto("Proyecto Test", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1),
            RolUsuario.LiderProyecto);

        nuevoProyecto.AsignarLider(usuarioLider);

        repoUsuarios.Agregar(usuarioLider);
        var tarea = new Tarea("TareaA", "Desc", 2, nuevoProyecto);
        nuevoProyecto.AgregarTarea(tarea);

        repoProyectos.Agregar(nuevoProyecto);

        var dto = new TareaDto { Titulo = "TareaA" };
        servicioProyecto.MarcarTareaDeProyectoComoRealizada(dto, "Proyecto Test", DateTime.Today);

        Proyecto proyectoActualizado = repoProyectos.EncontrarElemento(p => p.Nombre == nuevoProyecto.Nombre)
                                       ?? throw new KeyNotFoundException("Proyecto no encontrado");
        var tareaActualizada = proyectoActualizado.BuscarTareaPorNombre("TareaA");

        Assert.IsTrue(tareaActualizada.Realizada);
    }

    [TestMethod]
    [ExpectedException(typeof(ApplicationException))]
    public void CrearYAgregarTareaAProyecto_DatosInvalidos_LanzaApplicationException()
    {
        var tareaDtoInvalida = new TareaDto { Titulo = "", Descripcion = "Sin título", Duracion = 3 };

        servicioProyecto.CrearYAgregarTareaAProyecto("Proyecto X", tareaDtoInvalida);
    }
    

    [TestMethod]
    public void CalcularFechaFinProyecto_ConTareas_RetornaFechaFinCorrecta()
    {
        using var context = CrearContextoInMemory();
        var repoProyectos = new RepositorioProyectos(context);
        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,
            repoNotificaciones, servicioNotificaciones, repoTareas);

        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);

        var tarea = new Tarea("T1", "Desc", 4, proyecto);
        proyecto.AgregarTarea(tarea);
        
        
        repoProyectos.Agregar(proyecto);
        
        var fecha = servicioProyecto.TraerFechaFinProyecto("Proyecto");

        Assert.AreEqual(DateTime.Today.AddDays(3), fecha);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TraerUsuariosSinElProyecto_ProyectoDtoNulo_LanzaArgumentNullException()
    {
        servicioProyecto.TraerUsuariosSinElProyecto(null);
    }

    [TestMethod]
    public void CrearProyecto_CreaYAsociaMiembrosCorrectamente()
    {
        var admin = new Usuario("Admin", "Uno", "admin@mail.com", "Clave1@22", DateTime.Today.AddYears(-30), RolUsuario.AdminSistema);
        var miembro = new Usuario("Miembro", "User", "miembroo@mail.com", "Clave1@22", DateTime.Today.AddYears(-20), RolUsuario.MiembroProyecto);
        var usuarioLider = new Usuario("Lider", "Test", "lider22@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        
        repoUsuarios.Agregar(admin);
        repoUsuarios.Agregar(miembro);
        repoUsuarios.Agregar(usuarioLider);
        servicioUsuario.UsuarioLogueado = admin;

        var dto = new ProyectoDto
        {
            NombreProyecto = "Proyecto_Test" + Guid.NewGuid(),
            DescripcionProyecto = "Descripción",
            FechaInicio = DateTime.Today,
            MiembrosProyecto = new List<UsuarioDto>(),
            LiderProyecto = new UsuarioDto{Email = usuarioLider.Email}
        };
        
        servicioProyecto.CrearProyecto(dto);
        servicioProyecto.AgregarMiembroAProyecto(dto.NombreProyecto, miembro.Email);
        
        Proyecto creado = repoProyectos.EncontrarElemento(p => p.Nombre == dto.NombreProyecto) 
                          ?? throw new KeyNotFoundException("Proyecto no encontrado");

        Assert.AreEqual(3, creado.ListaUsuarios.Count); 
        Assert.AreEqual("lider22@test.com", creado.LiderEmail);
    }


    
    [TestMethod]
    public void MarcarTareaDeProyectoComoRealizada_TareaExiste_CambiaEstado()
    {
        var proyecto = new Proyecto("P1", "desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);

        var tarea = new Tarea("Tarea A", "desc", 2, proyecto);
        proyecto.AgregarTarea(tarea);
        repoProyectos.Agregar(proyecto);

        var dto = new TareaDto { Titulo = "Tarea A" };
        servicioProyecto.MarcarTareaDeProyectoComoRealizada(dto, "P1", DateTime.Today);

        Assert.IsTrue(tarea.Realizada);
    }
    [TestMethod]
    public void TraerUsuariosSinElProyecto_UsuarioNoTieneProyecto_DeberiaIncluirloEnResultado()
    {
        using var context = CrearContextoInMemory();

        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        var servicio = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,
            repoNotificaciones, servicioNotificaciones, repoTareas);

        var usuario = new Usuario("Pepe", "Perez", "pepe@mail.com", "Clave123!", DateTime.Today.AddYears(-20), RolUsuario.MiembroProyecto);
        repoUsuarios.Agregar(usuario);

        var proyectoDto = new ProyectoDto
        {
            NombreProyecto = "ProyectoPrueba"
        };

        var resultado = servicio.TraerUsuariosSinElProyecto(proyectoDto);

        Assert.IsTrue(resultado.Any(u => u.Email == "pepe@mail.com"));
    }



    [TestMethod]
    public void ConstructorSoloConRepositorioProyectos_SeCreaCorrectamente()
    {
        using var context = CrearContextoInMemory();
        var repo = new RepositorioProyectos(context);
        var servicio = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,
            repoNotificaciones, servicioNotificaciones, repoTareas);

        Assert.IsNotNull(servicio);
    }

    [TestMethod]
    public void CrearYAgregarTareaAProyecto_ConDependencias_AgregaCorrectamente()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context);
        var repoTareas = new RepositorioTareas(context);
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        
        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);
        
        var proyecto = new Proyecto("ProyectoDep", "desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        var tareaBase = new Tarea("TareaBase", "desc base", 2, proyecto);
        proyecto.AgregarTarea(tareaBase);
        repoProyectos.Agregar(proyecto);

        var dto = new TareaDto
        {
            Titulo = "TareaNueva",
            Descripcion = "desc nueva",
            Duracion = 3,
            DependenciasNombres = new List<string> { "TareaBase" }
        };
        servicioProyecto.CrearYAgregarTareaAProyecto("ProyectoDep", dto);

        Proyecto proyectoActual = repoProyectos.EncontrarElemento(p => p.Nombre == proyecto.Nombre) 
                                  ?? throw new KeyNotFoundException("Proyecto no encontrado");
        var tareaCreada = proyectoActual.BuscarTareaPorNombre("TareaNueva");

        Assert.AreEqual("TareaBase", tareaCreada.Dependencias.First().Titulo);
    }

    [TestMethod]
    public void AgregarMiembroAProyecto_DeberiaGenerarNotificacionAlUsuario()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context); 
        var repoNotificaciones = new RepositorioNotificaciones(context); 
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        
        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);

        var admin = new Usuario("Admin", "Principal", "admin@mail.com", "Admin123!", DateTime.Today.AddYears(-30), RolUsuario.AdminProyecto);
        var miembro2 = new Usuario("Nuevo", "Miembro", "miembro2@mail.com", "Clave123!", DateTime.Today.AddYears(-20), RolUsuario.MiembroProyecto);

        repoUsuarios.Agregar(admin);
        repoUsuarios.Agregar(miembro2);

        var proyecto = new Proyecto("ProyectoNotif", "Con notificaciones", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        proyecto.AgregarMiembroAlProyecto(admin);
        repoProyectos.Agregar(proyecto);

        servicioUsuario.UsuarioLogueado = admin;

        servicioProyecto.AgregarMiembroAProyecto("ProyectoNotif", "miembro2@mail.com");

        var notificaciones = repoNotificaciones.EncontrarLista(n => n.UsuarioEmail == miembro2.Email && !n.Vista);
        Assert.AreEqual(1, notificaciones.Count);
        Assert.AreEqual("Fuiste agregado al proyecto \"ProyectoNotif\".", notificaciones[0].Mensaje);
        Assert.IsFalse(notificaciones[0].Vista);
    }

    [TestMethod]
    public void ActualizarTarea_DescripcionYDuracion_DeberiaModificarCorrectamente()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context);
        var repoNotificaciones = new RepositorioNotificaciones(context);
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,
            repoNotificaciones, servicioNotificaciones, repoTareas);

        var proyecto1 = new Proyecto("ProyectoA", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto1.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        var tarea = new Tarea("Tarea1", "Desc original", 4, proyecto1);

        proyecto1.AgregarTarea(tarea);
        repoProyectos.Agregar(proyecto1);

        var tareaDto = new TareaDto
        {
            Titulo = "Tarea1",
            Descripcion = "Desc modificada",
            Duracion = 6
        };

        servicioProyecto.ActualizarTarea("ProyectoA", tareaDto);

        Assert.AreEqual("Desc modificada", tarea.Descripcion);
        Assert.AreEqual(6, tarea.DuracionEnDias);
    }

    
    [TestMethod]
    public void ActualizarTarea_DeberiaActualizarDescripcionDuracionYDependencias()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context); 
        var repoTareas = new RepositorioTareas(context);
        var repoNotificaciones = new RepositorioNotificaciones(context);
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        

        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);
        
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        this.repoUsuarios.Agregar(usuarioLider);
        
        var proyecto = new Proyecto("ProyectoEditarDeps", "Editar deps", DateTime.Today);
        proyecto.AsignarLider(usuarioLider);
        repoProyectos.Agregar(proyecto);
        
        var tareaBase = new Tarea("TareaBase", "desc", 2, proyecto);
        var dep1 = new Tarea("Dep1", "dep1", 1, proyecto);
        var dep2 = new Tarea("Dep2", "dep2", 1, proyecto);

        repoTareas.Agregar(tareaBase);
        repoTareas.Agregar(dep1);
        repoTareas.Agregar(dep2);
        
        tareaBase.AgregarDependencia(dep1);


        var dto = new TareaDto
        {
            Titulo = "TareaBase",
            Descripcion = "desc actualizada",
            Duracion = 3,
            DependenciasNombres = new List<string> { "Dep2" }
        };

        servicioProyecto.ActualizarTarea("ProyectoEditarDeps", dto);

        var tareaActualizada = proyecto.BuscarTareaPorNombre("TareaBase");

        Assert.AreEqual("desc actualizada", tareaActualizada.Descripcion);
        Assert.AreEqual(3, tareaActualizada.DuracionEnDias);
        Assert.AreEqual(2, tareaActualizada.Dependencias.Count());
        Assert.AreEqual("Dep2", tareaActualizada.Dependencias.First().Titulo);
    }

    [TestMethod]
    public void QuitarTodasLasDependencias_EliminaTodasCorrectamente()
    {
        var proyecto = new Proyecto("Proyecto A", "Descripcion", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);

        var tarea1 = new Tarea("Tarea 1", "Descripcion", 3, proyecto);
        var tarea2 = new Tarea("Tarea 2", "Descripcion", 2, proyecto);
        var tarea3 = new Tarea("Tarea 3", "Descripcion", 4, proyecto);

        proyecto.AgregarTarea(tarea1);
        proyecto.AgregarTarea(tarea2);
        proyecto.AgregarTarea(tarea3);

        tarea1.AgregarDependencia(tarea2);
        tarea1.AgregarDependencia(tarea3);

        Assert.AreEqual(2, tarea1.Dependencias.Count());

        tarea1.QuitarTodasLasDependencias();

        Assert.AreEqual(0, tarea1.Dependencias.Count());
    }

    [TestMethod]
    public void Editar_ConDatosValidos_CambiaDescripcionYDuracion()
    {
        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        var tarea = new Tarea("Tarea", "Desc original", 5, proyecto);

        tarea.Editar("Nueva desc", 10);

        Assert.AreEqual("Nueva desc", tarea.Descripcion);
        Assert.AreEqual(10, tarea.DuracionEnDias);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Editar_DescripcionInvalida_LanzaExcepcion()
    {
        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        var tarea = new Tarea("Tarea", "Desc original", 5, proyecto);

        tarea.Editar("", 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Editar_DuracionInvalida_LanzaExcepcion()
    {
        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider2@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

        proyecto.AsignarLider(usuarioLider);
    
        repoUsuarios.Agregar(usuarioLider);
        var tarea = new Tarea("Tarea", "Desc original", 5, proyecto);

        tarea.Editar("Desc nueva", 0);
    }

    

    [TestMethod]
    public void CaminoCritico_DeberiaDevolverRutaCriticaMasLarga()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context); 
        var repoNotificaciones = new RepositorioNotificaciones(context);
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        var servicio = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario,
            repoNotificaciones, servicioNotificaciones, repoTareas);

        var proyecto = new Proyecto("ProyectoX", "desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);
        proyecto.AsignarLider(usuarioLider);
        repoUsuarios.Agregar(usuarioLider);
        
        var t1 = new Tarea("A", "desc", 3, proyecto);
        var t2 = new Tarea("B", "desc", 2, proyecto);
        var t3 = new Tarea("C", "desc", 3, proyecto);

        t2.AgregarDependencia(t1);
        t3.AgregarDependencia(t1);

        proyecto.AgregarTarea(t1);
        proyecto.AgregarTarea(t2);
        proyecto.AgregarTarea(t3);
        repoProyectos.Agregar(proyecto);
        
        var rutasCriticas = servicio
            .CaminoCritico("ProyectoX")
            .Select(r => r.ToList())
            .ToList();
        
        Assert.AreEqual(1, rutasCriticas.Count, "Debe haber solo una ruta crítica (la más larga)");       
    }
    
    [TestMethod]
    public void CrearProyecto_DeberiaGenerarNotificacionAlLiderDelProyecto()
    {
        using var context = CrearContextoInMemory();
        var repoUsuarios = new RepositorioUsuarios(context);
        var repoProyectos = new RepositorioProyectos(context);
        var repoNotificaciones = new RepositorioNotificaciones(context);
        var servicioNotificaciones = new ServicioNotificaciones(repoNotificaciones);
        var servicioUsuario = new ServicioUsuario(repoUsuarios, servicioNotificaciones);
        

        var servicioProyecto = new ServicioProyecto(repoProyectos, repoUsuarios, servicioUsuario, repoNotificaciones, servicioNotificaciones, repoTareas);

        var admin = new Usuario("Admin", "Principal", "admin@mail.com", "Admin123!", DateTime.Today.AddYears(-30), RolUsuario.AdminProyecto);
        var lider = new Usuario("Lider", "Proyecto", "lider@mail.com", "ClaveLider123!", DateTime.Today.AddYears(-25), RolUsuario.LiderProyecto);

        repoUsuarios.Agregar(admin);
        repoUsuarios.Agregar(lider);

        servicioUsuario.UsuarioLogueado = admin;

        var proyectoDto = new ProyectoDto
        {
            NombreProyecto = "ProyectoLiderNotif",
            DescripcionProyecto = "Proyecto para testear notificación a líder",
            FechaInicio = DateTime.Today,
            LiderProyecto = new UsuarioDto { Email = lider.Email },
            MiembrosProyecto = new List<UsuarioDto>()
        };

        servicioProyecto.CrearProyecto(proyectoDto);

        var notificaciones = repoNotificaciones.EncontrarLista(n => n.UsuarioEmail == lider.Email && !n.Vista);
        Assert.AreEqual(1, notificaciones.Count);
        Assert.AreEqual("Fuiste asignado como Líder del proyecto \"ProyectoLiderNotif\".", notificaciones[0].Mensaje);
        Assert.IsFalse(notificaciones[0].Vista);
    }
    
    [TestMethod]
    public void ActualizarTarea_RecalculaFechasDeCPM()
    {
        var proyecto = new Proyecto("ProyectoCPM", "desc", DateTime.Today);
        var usuarioLider = new Usuario("Lider", "Test", "lider@testUy.com", "Password123.", DateTime.Today.AddYears(-20), RolUsuario.LiderProyecto);
        proyecto.AsignarLider(usuarioLider);
        repoUsuarios.Agregar(usuarioLider);

        var tareaA = new Tarea("Tarea A", "desc", 3, proyecto);
        var tareaB = new Tarea("Tarea B", "desc", 2, proyecto);

        tareaB.AgregarDependencia(tareaA);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        repoProyectos.Agregar(proyecto);

        var fechaInicioAntes = tareaB.FechaInicioTemprano;
        
        var dto = new TareaDto
        {
            Titulo = "Tarea A",
            Descripcion = "Editada",
            Duracion = 6
        };

        servicioProyecto.ActualizarTarea("ProyectoCPM", dto);

        var proyectoActualizado = repoProyectos.EncontrarElemento(p => p.Nombre == "ProyectoCPM")
                                  ?? throw new Exception("Proyecto no encontrado");

        var tareaBActualizada = proyectoActualizado.BuscarTareaPorNombre("Tarea B");
        var fechaInicioDespues = tareaBActualizada.FechaInicioTemprano;

        Assert.IsTrue(fechaInicioDespues > fechaInicioAntes, "La fecha de inicio debería haberse movido por el cambio de duración en la dependencia.");
    }
    
    [TestMethod]
    public void CPM_DeberiaDetectarRutaCritica_A_B_C()
    {
        var inicio = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("CPM Test", "desc", inicio);

        var a = new Tarea("A", "desc", 2, proyecto);
        var b = new Tarea("B", "desc", 3, proyecto);
        var c = new Tarea("C", "desc", 1, proyecto);

        b.AgregarDependencia(a);
        c.AgregarDependencia(b);

        proyecto.AgregarTarea(a);
        proyecto.AgregarTarea(b);
        proyecto.AgregarTarea(c);
        
        Assert.AreEqual(0, a.Holgura, "A debe ser crítica");
        Assert.AreEqual(0, b.Holgura, "B debe ser crítica");
        Assert.AreEqual(0, c.Holgura, "C debe ser crítica");
    }
    
    [TestMethod]
    public void CPM_NoDeberiaMarcarComoCriticasTareasIndependientes()
    {
        var inicio = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("Test Rutas Paralelas", "desc", inicio);

        var a = new Tarea("A", "desc", 5, proyecto);
        var b = new Tarea("B", "desc", 4, proyecto);

        proyecto.AgregarTarea(a);
        proyecto.AgregarTarea(b);
        
        Assert.AreEqual(0, a.Holgura, "A debe ser crítica (ruta más larga)");
        Assert.IsTrue(a.EsCritica, "A debe ser crítica");

        Assert.IsTrue(b.Holgura > 0, "B no debe ser crítica");
        Assert.IsFalse(b.EsCritica, "B no debe ser crítica");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearProyecto_ErrorSiFechaAnteriorAHoy()
    {
        ProyectoDto p = new();
        servicioProyecto.CrearProyecto(p);
    }

}
