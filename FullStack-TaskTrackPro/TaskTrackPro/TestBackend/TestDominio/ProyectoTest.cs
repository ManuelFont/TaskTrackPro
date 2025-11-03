using Backend.Dominio;

namespace TestBackend.TestDominio;

[TestClass]
public class ProyectoTest
{
    private Proyecto _proyecto;

    [TestInitialize]
    public void SetUp()
    {
        _proyecto = new Proyecto("Nombre", "Descripcion", DateTime.Today);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeRetornarErrorAtributoVacioConEspacio()
    {
        _proyecto = new Proyecto(" ", " ", DateTime.Today);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeRetornarErrorSiAtributoEsVacio()
    {
        _proyecto = new Proyecto("", "", DateTime.Today);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeRetornarErrorSiDescripcionExcede400Caracteres()
    {
        string descripcionLarga = new string('a', 401);
        _proyecto = new Proyecto("Proyecto", descripcionLarga, DateTime.Today);
    }

    [TestMethod]
    public void TareaSeAgregaCorrectamenteAProyecto()
    {
        Tarea t1 = new Tarea("Tarea1", "Descripcion1", 5, _proyecto);
        _proyecto.AgregarTarea(t1);
        Tarea t2 = _proyecto.BuscarTareaPorNombre(t1.Titulo);
        Assert.AreEqual(t1.Titulo, t2.Titulo);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void LanzaErrorSiAgregamosUnaTareaConTituloQueYaExista()
    {
        Tarea t1 = new Tarea("Tarea1", "Descripcion1", 5, _proyecto);
        Tarea t2 = new Tarea("Tarea1", "Descripcion2", 8, _proyecto);
        Proyecto p = new Proyecto("Proyecto", "Descripcion", DateTime.Today);
        p.AgregarTarea(t1);
        p.AgregarTarea(t2);
    }

    [TestMethod]
    public void MiembroSeAgregaCorrectamenteAProyecto()
    {
        Usuario u = new Usuario("Test", "User", "mail@mail.com", "Password1@", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
        _proyecto.AgregarMiembroAlProyecto(u);
        Usuario u1 = _proyecto.BuscarUsuarioEnProyectoPorMail(u.Email);
        Assert.AreEqual(u.Email, u1.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void LanzaErrorSiMiembroYaExiste()
    {
        Usuario u = new Usuario("Test", "User", "mail@mail.com", "Password1@", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
        Usuario u2 = new Usuario("Test", "User2", "mail@mail.com", "Password1@", new DateTime(2000, 1, 1),
            RolUsuario.AdminSistema);
        Proyecto p = new Proyecto("Proyecto", "Descripcion", DateTime.Today);
        p.AgregarMiembroAlProyecto(u);
        p.AgregarMiembroAlProyecto(u2);
    }

    [TestMethod]
    public void AdministradorDeProyectoModificaDescripcion()
    {
        Usuario administrador = new Usuario("Test", "User", "mail@mail.com", "Password1@",
            new DateTime(2000, 1, 1), RolUsuario.AdminProyecto);
        _proyecto.AgregarMiembroAlProyecto(administrador);
        _proyecto.ModificarProyecto(administrador, "NuevaDescripcion", null, _proyecto);
        Assert.AreEqual("NuevaDescripcion", _proyecto.Descripcion);
    }

    [TestMethod]
    public void AdministradorDeProyectoModificaFechaInicio()
    {
        Usuario administrador = new Usuario("Test", "User", "mail@mail.com", "Password1@",
            new DateTime(2000, 1, 1), RolUsuario.AdminProyecto);
        _proyecto.AgregarMiembroAlProyecto(administrador);
        _proyecto.ModificarProyecto(administrador, "", new DateTime(2025, 12, 23), _proyecto);
        Assert.AreEqual(new DateTime(2025, 12, 23), _proyecto.FechaInicioEstimada);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NoSeModificaNadaSiSePoneEnNullLosCampos()
    {
        Usuario administrador = new Usuario("Admin", "Test", "admin@mail.com", "Pass123!", DateTime.Today,
            RolUsuario.AdminProyecto);
        Proyecto proyecto = new Proyecto("Nombre", "Descripcion", DateTime.Today);
        proyecto.AgregarMiembroAlProyecto(administrador);
        proyecto.ModificarProyecto(administrador, null, null, proyecto);
        Assert.AreEqual(proyecto.Descripcion, "Descripcion");
        Assert.AreEqual(proyecto.FechaInicioEstimada, DateTime.Today);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UsuarioQueNoEsAdministradorDeProyectoNoPuedeModificar()
    {
        Usuario u = new Usuario("User", "Test", "user@mail.com", "Pass123!", new DateTime(2002, 12, 23),
            RolUsuario.AdminSistema);
        Proyecto proyecto = new Proyecto("Nombre", "Descripcion", DateTime.Today);
        proyecto.AgregarMiembroAlProyecto(u);
        proyecto.ModificarProyecto(u, "Nueva descripción", DateTime.Now, proyecto);
    }

    [TestMethod]
    public void FechaFinEstimado_DebeSerFechaInicioEstimado_SiNoHayTareas()
    {
        Proyecto proyecto = new Proyecto("Nombre", "Descrpcion", DateTime.Today);
        Assert.AreEqual(DateTime.Today, proyecto.FechaFinEstimada());
    }

    [TestMethod]
    public void FechaFinEstimado_ConUnaTarea()
    {
        Tarea tareaA = new Tarea("Nombre", "Descripcion", 3, _proyecto);
        _proyecto.AgregarTarea(tareaA);
        Assert.AreEqual(DateTime.Today.AddDays(2), _proyecto.FechaFinEstimada());
    }

    [TestMethod]
    public void FechaFinEstimado_ConUnaTareaCompletada_NoSeVeAfectadoPorLaFechaReal()
    {
        
        Tarea tareaA = new Tarea("Nombre", "Descripcion", 3, _proyecto);
        _proyecto.AgregarTarea(tareaA);

        tareaA.MarcarTareaRealizada(DateTime.Today.AddDays(250));

        DateTime fechaEstimado = _proyecto.FechaFinEstimada();

        Assert.AreEqual(
            DateTime.Today.AddDays(2),
            fechaEstimado,
            "La fecha estimada debe corresponder al plan (3 días), no a la fecha real de ejecución."
        );
    }
    
    [TestMethod]
    public void CaminoCritico_UnaTarea()
    {
        Tarea tareaA = new Tarea("Titulo", "Descripcion", 3, _proyecto);
        _proyecto.AgregarTarea(tareaA);
        List<Tarea> tareas = new List<Tarea>();
        tareas.Add(tareaA);
        CollectionAssert.AreEqual(tareas, _proyecto.CaminoCritico().ToList());
    }

    [TestMethod]
    public void CaminoCritico_DosTareasParalelas()
    {
        Tarea tareaA = new Tarea("TituloA", "Descripcion", 3, _proyecto);
        Tarea tareaB = new Tarea("TituloB", "Descripcion", 4, _proyecto);

        _proyecto.AgregarTarea(tareaA);
        _proyecto.AgregarTarea(tareaB);

        List<Tarea> tareasCaminoCritico = new List<Tarea>();
        tareasCaminoCritico.Add(tareaB);

        CollectionAssert.AreEqual(tareasCaminoCritico, _proyecto.CaminoCritico().ToList());
    }

    [TestMethod]
    public void CaminoCritico_DosTareasConsecutivasYUnaParalela()
    {
        Tarea tareaA = new Tarea("TituloA", "Descripcion", 1, _proyecto);
        Tarea tareaB = new Tarea("TituloB", "Descripcion", 2, _proyecto);
        Tarea tareaC = new Tarea("TituloC", "Descripcion", 1, _proyecto);

        _proyecto.AgregarTarea(tareaA);
        _proyecto.AgregarTarea(tareaB);
        _proyecto.AgregarTarea(tareaC);

        tareaB.AgregarDependencia(tareaA);

        var esperado = new List<Tarea> { tareaA, tareaB };

        CollectionAssert.AreEqual(esperado, _proyecto.CaminoCritico().ToList());
    }

    [TestMethod]
    public void Constructor_CamposValidos_CreaProyecto()
    {
        var hoy = DateTime.Today;
        var proyecto = new Proyecto("Proyecto1", "Descripción del proyecto", hoy);
        Assert.AreEqual("Proyecto1", proyecto.Nombre);
        Assert.AreEqual("Descripción del proyecto", proyecto.Descripcion);
        Assert.AreEqual(hoy, proyecto.FechaInicioEstimada);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructr_CamposInvalidos_LanzaArgumentException()
    {
        new Proyecto("", "", DateTime.Today.AddDays(-1));
    }

    [TestMethod]
    public void PropiedadNombre_ModificaYRetornaValor()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        p.Nombre = "NuevoNombre";
        Assert.AreEqual("NuevoNombre", p.Nombre);
    }

    [TestMethod]
    public void PropiedadDescripcion_ModificaYRetornaValor()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        p.Descripcion = "NuevaDesc";
        Assert.AreEqual("NuevaDesc", p.Descripcion);
    }

    [TestMethod]
    public void PropiedadFechaInicio_ModificaYRetornaValor()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var nuevaFecha = DateTime.Today.AddDays(2);
        p.FechaInicioEstimada = nuevaFecha;
        Assert.AreEqual(nuevaFecha, p.FechaInicioEstimada);
    }

    [TestMethod]
    public void AgregarMiembroUsuarioNoExistente_SeAgrega()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var u = CrearUsuario("user@mail.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        Assert.AreEqual(1, p.ListaUsuarios.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarMiembro_UsuarioExistente_LanzaArgumentException()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var u = CrearUsuario("user@mail.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        p.AgregarMiembroAlProyecto(u);
    }

    [TestMethod]
    public void BuscarUsuarioPorMailExistente_RetornaUsuario()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var u = CrearUsuario("u@mail.com", RolUsuario.AdminSistema);
        p.AgregarMiembroAlProyecto(u);
        var resultado = p.BuscarUsuarioEnProyectoPorMail("u@mail.com");
        Assert.AreSame(u, resultado);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarUsuarioPorMailNoExistente_LanzaException()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        p.BuscarUsuarioEnProyectoPorMail("no@mail.com");
    }

    [TestMethod]
    public void AgregarTareaTareaNoExistente_SeAgrega()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var t1 = new Tarea("T1", "Desc1", 1, p);
        p.AgregarTarea(t1);
        Assert.AreEqual(1, p.ListaDeTareas.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarTarea_TareaExistente_LanzaArgumentException()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var t1 = new Tarea("T1", "Desc1", 1, p);
        p.AgregarTarea(t1);
        p.AgregarTarea(t1);
    }

    [TestMethod]
    public void BuscarTareaPorNombreExistente_RetornaTarea()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        var t1 = new Tarea("T1", "Desc1", 1, p);
        p.AgregarTarea(t1);
        var resultado = p.BuscarTareaPorNombre("T1");
        Assert.AreSame(t1, resultado);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BuscarTareaPorNombreNoExistente_LanzaArgumentException()
    {
        var p = new Proyecto("N", "D", DateTime.Today);
        p.BuscarTareaPorNombre("X");
    }

    [TestMethod]
    public void FechaFinEstimadaSinTareas_RetornaFechaInicio()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("N", "D", inicio);
        Assert.AreEqual(inicio, p.FechaFinEstimada());
    }

    [TestMethod]
    public void FechaFinEstimada_ConTareas_RetornaMayorFechaFin()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("N", "D", inicio);
        var t1 = new Tarea("T1", "Desc1", 1, p);
        var t2 = new Tarea("T2", "Desc2", 2, p);
        p.AgregarTarea(t1);
        p.AgregarTarea(t2);
        var fechaEsperada = inicio.AddDays(1);
        Assert.AreEqual(fechaEsperada, p.FechaFinEstimada());
    }

    [TestMethod]
    public void ModificarProyectoAdmin_CambiaDescripcionYFecha()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("N", "D", inicio);
        var admin = CrearUsuario("admin@mail.com", RolUsuario.AdminProyecto);
        p.AgregarMiembroAlProyecto(admin);
        var nuevaFecha = inicio.AddDays(5);
        p.ModificarProyecto(admin, "DescNueva", nuevaFecha, p);
        Assert.AreEqual("DescNueva", p.Descripcion);
        Assert.AreEqual(nuevaFecha, p.FechaInicioEstimada);
    }

    [TestMethod]
    public void CaminoCritico_ConTareasLineales_RetornaCaminoEnOrdenCorrecto()
    {
        var inicio = DateTime.Today;
        var proyecto = new Proyecto("N", "D", inicio);

        var t1 = new Tarea("T1", "Desc1", 1, proyecto);
        var t2 = new Tarea("T2", "Desc2", 1, proyecto);
        t2.AgregarDependencia(t1);

        proyecto.AgregarTarea(t1);
        proyecto.AgregarTarea(t2);

        var camino = proyecto.CaminoCritico().ToList();

        CollectionAssert.AreEqual(new List<Tarea> { t1, t2 }, camino);
    }
    
    private Usuario CrearUsuario(string mail, RolUsuario rol) => new Usuario("Nombre", "Apellido", mail, "Passw0rd1!",
        DateTime.Today.AddYears(-30), rol);

    private Proyecto CrearProyecto(string nombre) => new Proyecto(nombre, "Descripción", DateTime.Today);

    [TestMethod]
    public void Constructor_CamposValidos_CreaInstancia()
    {
        var hoy = DateTime.Today;
        var p = new Proyecto("P1", "Desc", hoy);
        Assert.AreEqual("P1", p.Nombre);
        Assert.AreEqual("Desc", p.Descripcion);
        Assert.AreEqual(hoy, p.FechaInicioEstimada);
        Assert.AreEqual(0, p.ListaDeTareas.Count);
        Assert.AreEqual(0, p.ListaUsuarios.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_CamposInvalidos_LanzaArgumentException()
    {
        new Proyecto("", "", DateTime.Today.AddDays(-1));
    }

    [TestMethod]
    public void PropiedadNombre_SetGet_CambiaValor()
    {
        var p = CrearProyecto("P1");
        p.Nombre = "ProyectoNuevo";
        Assert.AreEqual("ProyectoNuevo", p.Nombre);
    }
    

    [TestMethod]
    public void PropiedadDescripcion_SetGet_CambiaValor()
    {
        var p = CrearProyecto("P1");
        p.Descripcion = "DescNueva";
        Assert.AreEqual("DescNueva", p.Descripcion);
    }
    

    [TestMethod]
    public void PropiedadFechaInicio_SetGet_CambiaValor()
    {
        var p = CrearProyecto("P1");
        var fecha = DateTime.Today.AddDays(3);
        p.FechaInicioEstimada = fecha;
        Assert.AreEqual(fecha, p.FechaInicioEstimada);
    }

    [TestMethod]
    public void PropiedadListaUsuarios_SetGet_CambiaLista()
    {
        var p = CrearProyecto("P1");
        var lista = new List<Usuario> { CrearUsuario("a@b.com", RolUsuario.MiembroProyecto) };
        p.ListaUsuarios = lista;
        Assert.AreSame(lista, p.ListaUsuarios);
    }
    
    [TestMethod]
    public void AgregarMiembro_UsuarioNoExistente_SeAgrega()
    {
        var p = CrearProyecto("P1");
        var u = CrearUsuario("u@x.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        Assert.AreEqual(1, p.ListaUsuarios.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarMiembro_Existente_LanzaArgumentException()
    {
        var p = CrearProyecto("P1");
        var u = CrearUsuario("u@x.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        p.AgregarMiembroAlProyecto(u);
    }

    [TestMethod]
    public void BuscarUsuarioPorMail_Existente_RetornaUsuario()
    {
        var p = CrearProyecto("P1");
        var u = CrearUsuario("u@x.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        var res = p.BuscarUsuarioEnProyectoPorMail("u@x.com");
        Assert.AreSame(u, res);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarUsuarioPorMail_NoExistente_LanzaException()
    {
        var p = CrearProyecto("P1");
        p.BuscarUsuarioEnProyectoPorMail("no@x.com");
    }

    [TestMethod]
    public void AgregarTarea_TareaNoExistente_SeAgrega()
    {
        var p = CrearProyecto("P1");
        var t = new Tarea("T1", "Desc", 2, p);
        p.AgregarTarea(t);
        Assert.AreEqual(1, p.ListaDeTareas.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarTarea_Existente_LanzaArgumentException()
    {
        var p = CrearProyecto("P1");
        var t = new Tarea("T1", "Desc", 2, p);
        p.AgregarTarea(t);
        p.AgregarTarea(t);
    }

    [TestMethod]
    public void BuscarTareaPorNombre_Existente_RetornaTarea()
    {
        var p = CrearProyecto("P1");
        var t = new Tarea("T1", "Desc", 2, p);
        p.AgregarTarea(t);
        var res = p.BuscarTareaPorNombre("T1");
        Assert.AreSame(t, res);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BuscarTareaPorNombre_NoExistente_LanzaArgumentException()
    {
        var p = CrearProyecto("P1");
        p.BuscarTareaPorNombre("NINGUNA");
    }

    [TestMethod]
    public void FechaFinEstimada_SinTareas_RetornaFechaInicio()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("P1", "Desc", inicio);
        Assert.AreEqual(inicio, p.FechaFinEstimada());
    }

    [TestMethod]
    public void FechaFinEstimada_ConTareas_RetornaFechaFinMayor()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("P1", "Desc", inicio);
        var t1 = new Tarea("T1", "Desc", 1, p);
        var t2 = new Tarea("T2", "Desc", 3, p);
        p.AgregarTarea(t1);
        p.AgregarTarea(t2);
        var esperado = inicio.AddDays(2);
        Assert.AreEqual(esperado, p.FechaFinEstimada());
    }

    [TestMethod]
    public void ModificarProyecto_Admin_CambiaDescripcionYFecha()
    {
        var p = CrearProyecto("P1");
        var admin = CrearUsuario("a@x.com", RolUsuario.AdminProyecto);
        p.AgregarMiembroAlProyecto(admin);
        var nuevaDesc = "DescModificada";
        var nuevaFecha = DateTime.Today.AddDays(7);
        p.ModificarProyecto(admin, nuevaDesc, nuevaFecha, p);
        Assert.AreEqual(nuevaDesc, p.Descripcion);
        Assert.AreEqual(nuevaFecha, p.FechaInicioEstimada);
    }

    [TestMethod]
    public void ModificarProyecto_AdminSoloDescripcion_CambiaDescripcionMantieneFecha()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("P1", "Desc", inicio);
        var admin = CrearUsuario("a@x.com", RolUsuario.AdminProyecto);
        p.AgregarMiembroAlProyecto(admin);
        p.ModificarProyecto(admin, "SoloDesc", null, p);
        Assert.AreEqual("SoloDesc", p.Descripcion);
        Assert.AreEqual(inicio, p.FechaInicioEstimada);
    }

    [TestMethod]
    public void ModificarProyecto_AdminSoloFecha_CambiaFechaMantieneDescripcion()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("P1", "Desc", inicio);
        var admin = CrearUsuario("a@x.com", RolUsuario.AdminProyecto);
        p.AgregarMiembroAlProyecto(admin);
        var nuevaFecha = inicio.AddDays(2);
        p.ModificarProyecto(admin, null, nuevaFecha, p);
        Assert.AreEqual("Desc", p.Descripcion);
        Assert.AreEqual(nuevaFecha, p.FechaInicioEstimada);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ModificarProyecto_NoAdmin_LanzaArgumentException()
    {
        var p = CrearProyecto("P1");
        var u = CrearUsuario("u@x.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);
        p.ModificarProyecto(u, "X", DateTime.Today, p);
    }

    [TestMethod]
    public void CaminoCritico_ConTareasLineales_RetornaEnOrdenInicioAFin()
    {
        var inicio = DateTime.Today;
        var p = new Proyecto("P1", "Desc", inicio);

        var t1 = new Tarea("T1", "Desc", 1, p);
        var t2 = new Tarea("T2", "Desc", 2, p);
        t2.AgregarDependencia(t1);

        p.AgregarTarea(t1);
        p.AgregarTarea(t2);

        var camino = p.CaminoCritico().ToList();

        CollectionAssert.AreEqual(new List<Tarea> { t1, t2 }, camino);
    }

    [TestMethod]
    public void BuscarUsuarioEnProyectoPorMail_UsuarioExistente_RetornaInstancia()
    {
        var p = new Proyecto("P1", "Desc", DateTime.Today);
        var u = CrearUsuario("test@mail.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);

        var resultado = p.BuscarUsuarioEnProyectoPorMail("test@mail.com");

        Assert.AreSame(u, resultado);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarUsuarioEnProyectoPorMail_UsuarioNoExistente_LanzaException()
    {
        var p = new Proyecto("P1", "Desc", DateTime.Today);

        p.BuscarUsuarioEnProyectoPorMail("noexiste@mail.com");
    }
    
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarUsuarioEnProyectoPorMail_UsuarioNoExistenteEnListaVacia_LanzaException()
    {
        var p = new Proyecto("P1", "Desc", DateTime.Today);
        p.BuscarUsuarioEnProyectoPorMail("noexiste@mail.com");
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BuscarUsuarioEnProyectoPorMail_UsuarioNoExistenteEnListaConUsuarios_LanzaException()
    {
        var p = new Proyecto("P1", "Desc", DateTime.Today);
        var u = CrearUsuario("user1@mail.com", RolUsuario.MiembroProyecto);
        p.AgregarMiembroAlProyecto(u);

        p.BuscarUsuarioEnProyectoPorMail("otro@mail.com");
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_DescripcionSupera400Caracteres_LanzaExcepcion()
    {
        var nombre = "Proyecto Test";
        var descripcionLarga = new string('a', 401); 
        var fecha = DateTime.Today.AddDays(1);
        var proyecto = new Proyecto(nombre, descripcionLarga, fecha);

    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_DescripcionVacia_LanzaExcepcion()
    {
        var nombre = "Proyecto Test";
        var descripcion = ""; 
        var fecha = DateTime.Today.AddDays(1);
        var proyecto = new Proyecto(nombre, descripcion, fecha);
    }
    
    [TestMethod]
    public void TraerTareasCriticas_DevuelveSoloTareasEnCaminoCritico()
    {
        
        var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today.AddDays(1));

        var tareaCritica = new Tarea("Tarea Crítica", "desc", 2, proyecto);
        var tareaNoCritica = new Tarea("Tarea No Crítica", "desc", 1, proyecto);

        proyecto.AgregarTarea(tareaCritica);
        proyecto.AgregarTarea(tareaNoCritica);


        var tareasCriticas = proyecto.TraerTareasCriticas();

        Assert.AreEqual(1, tareasCriticas.Count());
        Assert.IsTrue(tareasCriticas.Contains(tareaCritica));
        Assert.IsFalse(tareasCriticas.Contains(tareaNoCritica));
    }
    
    [TestMethod]
    public void TraerTareasNoCriticas_DevuelveSoloTareasFueraDeCaminoCritico()
    {
        var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today.AddDays(1));

        var t1 = new Tarea("T1", "desc", 1, proyecto);
        var t2 = new Tarea("T2", "desc", 1, proyecto);
        var t3 = new Tarea("T3", "desc", 3, proyecto);

      
        proyecto.AgregarTarea(t1);
        proyecto.AgregarTarea(t2);
        proyecto.AgregarTarea(t3);

        var tareasNoCriticas = proyecto.TraerTareasNoCriticas();

        Assert.AreEqual(2, tareasNoCriticas.Count);
        Assert.IsTrue(tareasNoCriticas.Contains(t1));
        Assert.IsTrue(tareasNoCriticas.Contains(t2));
        Assert.IsFalse(tareasNoCriticas.Contains(t3));
    }

    [TestMethod]
    public void EliminarUsuarioDeProyecto_DeberiaEliminarCorrectamente()
    {
        var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today.AddDays(1));
        var usuario = new Usuario("Ana", "Tester", "ana@mail.com", "Test123@", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);

        proyecto.AgregarMiembroAlProyecto(usuario);
        Assert.IsTrue(proyecto.ListaUsuarios.Contains(usuario));
        
        proyecto.EliminarUsuarioDeProyecto(usuario);
        
        Assert.IsFalse(proyecto.ListaUsuarios.Contains(usuario));
    }
    
}
    

