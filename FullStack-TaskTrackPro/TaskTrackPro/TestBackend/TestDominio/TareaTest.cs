using Backend.Dominio;

namespace TestBackend.TestDominio;

[TestClass]
public class TareaTest
{
    private static Proyecto _proyecto;
    private Tarea _tareaA;
    private Tarea _tareaB;
    private Tarea _tareaC;
    private Tarea _tareaD;
    private Tarea _tareaE;
    
    [TestInitialize]
    public void SetUp()
    {
        _proyecto = new Proyecto("Nom", "Des", DateTime.Today);
        _tareaA = new Tarea("TareaA", "Descripcion1", 1, _proyecto);
        _tareaB = new Tarea("TareaB", "Descripcion2", 2, _proyecto);
        _tareaC = new Tarea("TareaC", "Descripcion3", 3, _proyecto);
        _tareaD = new Tarea("TareaD", "Descripcion4", 4, _proyecto);
        _tareaE = new Tarea("TareaE", "Descripcion5", 5, _proyecto);
        _proyecto.AgregarTarea(_tareaA);
        _proyecto.AgregarTarea(_tareaB);
        _proyecto.AgregarTarea(_tareaC);
        _proyecto.AgregarTarea(_tareaD);
    }

    [TestMethod]
    public void AgregarDependencia()
    {
        _tareaA.AgregarDependencia(_tareaB);
        CollectionAssert.Contains(_tareaA.Dependencias.ToList(), _tareaB);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeLanzarErrorSiTituloEsVacio()
    {
        new Tarea("", "Descripción válida",5, _proyecto,DateTime.Now.AddDays(1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeLanzarErrorSiFechaInicioEsEnElPasado()
    {
        new Tarea("Titulo válido", "Descripción válida",5, _proyecto, DateTime.Now.AddDays(-1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DebeLanzarErrorSiDuracionEsMenorOIgualACero()
    {
        new Tarea("Titulo válido", "Descripción válida", 0,_proyecto, DateTime.Now.AddDays(1));
    }
    
    [TestMethod]
    public void DebeCrearTareaConTitulo_Descripcion_Duracion_FechaInicio_Validos()
    {
        Tarea tarea = new Tarea("Titulo válido", "Descripción válida", 3, _proyecto, DateTime.Now.AddDays(1));
        Assert.IsNotNull(tarea);
        Assert.AreEqual("Titulo válido", tarea.Titulo);
        Assert.AreEqual("Descripción válida", tarea.Descripcion);
        Assert.AreEqual(3, tarea.DuracionEnDias);
    }

    [TestMethod]
    public void DebeCargarTareaEnListaDependencias()
    {
        Tarea tareaA = new Tarea("Tarea A", "primera tarea", 3, _proyecto, DateTime.Now.AddDays(1));
        Tarea tareaB = new Tarea("Tarea B", "segunda tarea", 3, _proyecto, DateTime.Now.AddDays(2));
        
        tareaB.AgregarDependencia(tareaA);
        Assert.AreEqual(tareaB.Dependencias.First(), tareaA);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ErrorAlMarcarComoRealizada_SiDependenciasNoCompletadas()
    {
        Tarea tareaA = new Tarea("Titulo", "Descripcion", 3, _proyecto);
        Tarea tareaB = new Tarea("Titulo", "Descripcion", 2, _proyecto);
        
        tareaB.AgregarDependencia(tareaA);
        tareaB.MarcarTareaRealizada(new DateTime(2100, 1, 1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ErrorAlMarcarRealizada_FechaEjecucionAnteriorAFechaInicioProyecto()
    {
        Tarea tareaA = new Tarea("Titulo", "Descripcion", 3, _proyecto);
        tareaA.MarcarTareaRealizada(new  DateTime(2020, 1, 1));
    }
    
    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConUnaTareaSola()
    {
        Proyecto proyecto = new Proyecto("Nom", "Des", DateTime.Today);
        Tarea tareaA = new Tarea("TareaA", "Descripcion1", 1, _proyecto);
     
        proyecto.AgregarTarea(tareaA);
      
        Assert.AreEqual(_tareaA.FechaInicioTemprano, _proyecto.FechaInicioEstimada);
        Assert.AreEqual(_tareaA.FechaFinTemprano, _proyecto.FechaInicioEstimada);
    }

   [TestMethod]
   public void FechaInicioTemprano_FechaFinTemprano_ConDosTareasConsecutivas()
   {
       var inicio = new DateTime(2025, 6, 20);
       var proyecto = new Proyecto("Proyecto", "Desc", inicio);
       
       var tareaA = new Tarea("A", "desc", 5, proyecto);
       var tareaB = new Tarea("B", "desc", 3, proyecto);
   
       tareaB.AgregarDependencia(tareaA);
   
       proyecto.AgregarTarea(tareaA);
       proyecto.AgregarTarea(tareaB);
   
       var esB = tareaB.FechaInicioTemprano;
       var efB = tareaB.FechaFinTemprano;
   
       Assert.AreEqual(inicio.AddDays(5), esB, "Inicio temprano de B debe ser el día siguiente a fin de A (25/6)");
       Assert.AreEqual(inicio.AddDays(7), efB, "Fin temprano de B debe ser 27/6");
   }
   
    [TestMethod]
    public void FechaInicioTemprano_ConTresTareasConsecutivas()
    {
        _tareaC.AgregarDependencia(_tareaB);
        _tareaB.AgregarDependencia(_tareaA);
        Assert.AreEqual(_proyecto.FechaInicioEstimada.AddDays(3), _tareaC.FechaInicioTemprano);
    }
    
    [TestMethod]
    public void FechaInicioTemprano_ConCuatroTareasConsecutivas()
    {
        var fechaInicioProyecto = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("Proyecto A -> B -> C -> D", "Test", fechaInicioProyecto);

        var tareaA = new Tarea("A", "Inicio", 2, proyecto);
        var tareaB = new Tarea("B", "Siguiente", 2, proyecto);
        var tareaC = new Tarea("C", "Tercera", 1, proyecto);
        var tareaD = new Tarea("D", "Final", 3, proyecto);
        
        tareaB.AgregarDependencia(tareaA);
        tareaC.AgregarDependencia(tareaB);
        tareaD.AgregarDependencia(tareaC);
        
        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        proyecto.AgregarTarea(tareaD);

        Assert.AreEqual(fechaInicioProyecto, tareaA.FechaInicioTemprano);
        Assert.AreEqual(fechaInicioProyecto.AddDays(2), tareaB.FechaInicioTemprano);
        Assert.AreEqual(fechaInicioProyecto.AddDays(4), tareaC.FechaInicioTemprano);
        Assert.AreEqual(fechaInicioProyecto.AddDays(5), tareaD.FechaInicioTemprano);
    }
    
    [TestMethod]
    public void FechaInicioTemprano_ConDosDependenciasParalelas()
    {
        _tareaC.AgregarDependencia(_tareaA);
        _tareaC.AgregarDependencia(_tareaB);
        Assert.AreEqual(_proyecto.FechaInicioEstimada.AddDays(2), _tareaC.FechaInicioTemprano);
    }
    
    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConTresDependenciasParalelas()
    {
        var inicio = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("Proyecto Paralelo", "Desc", inicio);

        var tareaA = new Tarea("A", "desc", 2, proyecto);
        var tareaB = new Tarea("B", "desc", 3, proyecto);
        var tareaC = new Tarea("C", "desc", 1, proyecto);
        var tareaD = new Tarea("D", "desc", 4, proyecto);

        tareaD.AgregarDependencia(tareaA);
        tareaD.AgregarDependencia(tareaB);
        tareaD.AgregarDependencia(tareaC);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        proyecto.AgregarTarea(tareaD);

        var esD = tareaD.FechaInicioTemprano;
        var efD = tareaD.FechaFinTemprano;

        Assert.AreEqual(inicio.AddDays(3), esD, "D debe empezar el 23/6");
        Assert.AreEqual(inicio.AddDays(6), efD, "D debe terminar el 26/6");
    }

    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConDependenciasConsecutivasYParalelas()
    {
        DateTime fechaInicioProyecto = new DateTime(2025, 6, 20);
        Proyecto proyecto = new Proyecto("Proyecto X", "Descripción", fechaInicioProyecto);

        Tarea tareaA = new Tarea("Tarea A", "Desc A", 2, proyecto);
        Tarea tareaB = new Tarea("Tarea B", "Desc B", 2, proyecto);
        Tarea tareaC = new Tarea("Tarea C", "Desc C", 2, proyecto);
        Tarea tareaD = new Tarea("Tarea D", "Desc D", 2, proyecto);

        tareaC.AgregarDependencia(tareaB);
        tareaD.AgregarDependencia(tareaA);
        tareaD.AgregarDependencia(tareaC);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        proyecto.AgregarTarea(tareaD);

        Assert.AreEqual(fechaInicioProyecto.AddDays(4), tareaD.FechaInicioTemprano);
        Assert.AreEqual(fechaInicioProyecto.AddDays(5), tareaD.FechaFinTemprano);
    }


    
    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConUnaDependencia_Planificacion()
    {
        _tareaA.DuracionEnDias = 20;
        _tareaB.DuracionEnDias = 2;
        _tareaB.AgregarDependencia(_tareaA);

        DateTime esB = _tareaB.FechaInicioTemprano;
        DateTime efB = _tareaB.FechaFinTemprano;

        Assert.AreEqual(
            _proyecto.FechaInicioEstimada.AddDays(20),
            esB,
            "El inicio temprano de B debe coincidir con el fin planificado de A (20 días)."
        );
        Assert.AreEqual(
            _proyecto.FechaInicioEstimada.AddDays(21),
            efB,
            "El fin temprano de B debe ser inicio temprano + 2 días - 1."
        );
    }
    
    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConUnaDependenciaRealizadaYUnaNo()
    {
        _tareaA.DuracionEnDias = 20;
        _tareaB.DuracionEnDias = 19;
        _tareaC.DuracionEnDias = 3;

        _tareaC.AgregarDependencia(_tareaA);
        _tareaC.AgregarDependencia(_tareaB);

        _tareaA.MarcarTareaRealizada(_proyecto.FechaInicioEstimada.AddDays(20));

        DateTime esC = _tareaC.FechaInicioTemprano;  
        DateTime efC = _tareaC.FechaFinTemprano;

        Assert.AreEqual(
            _proyecto.FechaInicioEstimada.AddDays(20),
            esC,
            "FechaInicioTemprano de C debe coincidir con el fin planificado de A (20 días)."
        );
        Assert.AreEqual(
            _proyecto.FechaInicioEstimada.AddDays(22),
            efC,
            "FechaFinTemprano de C debe ser inicio temprano + 3 días - 1."
        );
    }

    [TestMethod]
    public void FechaInicioTemprano_FechaFinTemprano_ConDosDependenciasParalelas()
    {
        DateTime inicio = new DateTime(2025, 6, 20);
        Proyecto proyecto = new Proyecto("Proyecto", "Descripción", inicio);

        Tarea tareaA = new Tarea("A", "desc", 20, proyecto);
        Tarea tareaB = new Tarea("B", "desc", 19, proyecto);
        Tarea tareaC = new Tarea("C", "desc", 3, proyecto);

        tareaC.AgregarDependencia(tareaA);
        tareaC.AgregarDependencia(tareaB);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);

        var inicioTempranoC = tareaC.FechaInicioTemprano;
        var finTempranoC = tareaC.FechaFinTemprano;

        Assert.AreEqual(new DateTime(2025, 7, 10), inicioTempranoC, "C debe iniciar el 10/07");
        Assert.AreEqual(new DateTime(2025, 7, 12), finTempranoC, "C debe finalizar el 12/07");
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarRequeridor_ErrorSiNoPertenecenAlMismoProyecto()
    {
        Proyecto proyecto2 = new Proyecto("Nombre", "Des", DateTime.Today);
        Tarea tareaX = new Tarea("nombre", "des", 3, proyecto2);
        _tareaA.AgregarRequeridor(tareaX);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarRequerido_ErrorSiNoSoyDependenciaDelRequerido()
    {
        _tareaA.AgregarRequeridor(_tareaB);
    }

    [TestMethod]
    public void AgregarRequerido_AgregaRequeridoALista()
    {
        _tareaB.AgregarDependencia(_tareaA);
        CollectionAssert.Contains(_tareaA.Requeridores.ToList(), _tareaB);
    }
    
    [TestMethod]
    public void EsCritica()
    {
        Assert.IsFalse(_tareaA.EsCritica);
        Assert.IsTrue(_tareaD.EsCritica);
    }

    [TestMethod]
    public void FechaFinTarde_SinDependencias()
    {
        Assert.AreEqual(_tareaA.FechaFinTarde, _proyecto.FechaFinEstimada());
        Assert.AreEqual(_tareaB.FechaFinTarde, _proyecto.FechaFinEstimada());
        Assert.AreEqual(_tareaC.FechaFinTarde, _proyecto.FechaFinEstimada());
        Assert.AreEqual(_tareaD.FechaFinTarde, _proyecto.FechaFinEstimada());
    }

    [TestMethod]
    public void FechaFinTarde_ConUnaDependencia_DesdeCero()
    {
        var inicio = new DateTime(2025, 7, 30);
        var proyecto = new Proyecto("Proyecto prueba", "desc", inicio);

        var tareaA = new Tarea("TareaA", "Desc", 1, proyecto);
        var tareaB = new Tarea("TareaB", "Desc", 2, proyecto); 

        tareaB.AgregarDependencia(tareaA);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        
        Assert.AreEqual(proyecto.FechaFinEstimada(), tareaB.FechaFinTarde, "B debe terminar cuando termina el proyecto");
        
        Assert.AreEqual(tareaB.FechaInicioTarde.AddDays(-1), tareaA.FechaFinTarde, "A debe terminar justo antes que empiece B");
    }

  

    [TestMethod]
    public void FechaFinTarde_ConDosRequeridores_DesdeCero()
    {
        var inicio = new DateTime(2025, 7, 30);
        var proyecto = new Proyecto("ProyectoDosRequeridores", "desc", inicio);
        
        var tareaA = new Tarea("TareaA", "desc", 2, proyecto);
        var tareaB = new Tarea("TareaB", "desc", 3, proyecto);
        var tareaC = new Tarea("TareaC", "desc", 2, proyecto);
    
        tareaB.AgregarDependencia(tareaA);
        tareaC.AgregarDependencia(tareaA);
    
        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        
        Assert.AreEqual(proyecto.FechaFinEstimada(), tareaB.FechaFinTarde, "B debería terminar al final del proyecto");
        Assert.AreEqual(proyecto.FechaFinEstimada(), tareaC.FechaFinTarde, "C debería terminar al final del proyecto");
        
        var inicioTardeMin = new[] { tareaB.FechaInicioTarde, tareaC.FechaInicioTarde }.Min();
        Assert.AreEqual(inicioTardeMin.AddDays(-1), tareaA.FechaFinTarde, "A debería terminar justo antes que el requeridor más temprano inicie");
    }

    [TestMethod]
    public void Holgura()
    {
        TimeSpan diferencia = _tareaA.FechaFinTarde - _tareaA.FechaFinTemprano;
        Assert.AreEqual(diferencia.Days, _tareaA.Holgura);
    }
    
    [TestMethod]
    public void Holgura_PruebaUnaTareaConTresDependencias()
    {
        DateTime inicio = new DateTime(2025, 7, 30);
        var proyecto = new Proyecto("Proyecto Test", "Descripción", inicio);

        var tareaA = new Tarea("A", "desc", 1, proyecto);
        var tareaB = new Tarea("B", "desc", 2, proyecto);
        var tareaC = new Tarea("C", "desc", 3, proyecto);
        var tareaD = new Tarea("D", "desc", 1, proyecto);

        tareaD.AgregarDependencia(tareaA);
        tareaD.AgregarDependencia(tareaB);
        tareaD.AgregarDependencia(tareaC);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        proyecto.AgregarTarea(tareaD);
        
        Assert.AreEqual(0, tareaD.Holgura, "D debería tener holgura 0");
        Assert.AreEqual(0, tareaC.Holgura, "C pertenece al camino critico con D");
        Assert.AreEqual(1, tareaB.Holgura, "B debería tener holgura 1");
        Assert.AreEqual(2, tareaA.Holgura, "A debería tener holgura 2");
    }
    [TestMethod]
    public void Holgura_pruebaConTareasParalelasYConsecutivas()
    {
        var fechaInicio = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("Proyecto X", "Desc", fechaInicio);

        var tareaA = new Tarea("A", "desc", 10, proyecto);
        var tareaB = new Tarea("B", "desc", 5, proyecto);
        var tareaC = new Tarea("C", "desc", 1, proyecto);
        var tareaD = new Tarea("D", "desc", 20, proyecto); 

        tareaC.AgregarDependencia(tareaA);
        tareaC.AgregarDependencia(tareaB);

        proyecto.AgregarTarea(tareaA);
        proyecto.AgregarTarea(tareaB);
        proyecto.AgregarTarea(tareaC);
        proyecto.AgregarTarea(tareaD);

        Assert.AreEqual(9, tareaA.Holgura, "A tiene la misma holgura que C");
        Assert.AreEqual(14, tareaB.Holgura, "B puede demorarse 5 días más");
        Assert.AreEqual(9, tareaC.Holgura, "C termina antes de la crítica");
        Assert.AreEqual(0, tareaD.Holgura, "D es crítica");
    }


    [TestMethod]
    public void DebeObtenerFechaFinRealCorrecta()
    {
        var proyecto = new Proyecto("TestProy", "Desc", DateTime.Today);
        var tarea = new Tarea("T1", "desc", 3, proyecto);
        tarea.MarcarTareaRealizada(DateTime.Today.AddDays(4));
    
        Assert.AreEqual(DateTime.Today.AddDays(4), tarea.FechaFinReal());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DebeLanzarExcepcionSiNoSePuedeObtenerFechaFinReal()
    {
        var proyecto = new Proyecto("TestProy", "Desc", DateTime.Today);
        var tarea = new Tarea("T1", "desc", 3, proyecto);
        var fecha = tarea.FechaFinReal();
    }

    [TestMethod]
    public void DebeCompararFechaFinRealConFechaFinTardeCorrectamente()
    {
        var proyecto = new Proyecto("TestProy", "Desc", DateTime.Today);
        var tarea = new Tarea("T1", "desc", 3, proyecto);
        proyecto.AgregarTarea(tarea);
    
        DateTime fechaTarde = tarea.FechaFinTarde;
        tarea.MarcarTareaRealizada(fechaTarde.AddDays(2));

        Assert.IsTrue(tarea.FechaFinReal() > fechaTarde);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarDependencia_TareaEsEllaMisma_LanzaExcepcion()
    {
        var proyecto = new Proyecto("Proyecto Test", "Desc", DateTime.Today.AddDays(1));
        var tarea = new Tarea("Tarea", "desc", 1, proyecto);

        tarea.AgregarDependencia(tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarDependencia_TareaDeOtroProyecto_LanzaExcepcion()
    {
        var proyecto1 = new Proyecto("P1", "desc", DateTime.Today.AddDays(1));
        var proyecto2 = new Proyecto("P2", "desc", DateTime.Today.AddDays(1));

        var tarea1 = new Tarea("Tarea 1", "desc", 2, proyecto1);
        var tareaDeOtroProyecto = new Tarea("Otra", "desc", 2, proyecto2);

        tarea1.AgregarDependencia(tareaDeOtroProyecto);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DependenciaConFechaMasTardia_SinDependencias_LanzaExcepcion()
    {
        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today.AddDays(1));
        var tarea = new Tarea("Tarea", "desc", 1, proyecto);

        tarea.DependenciaConFechaMasTardia(); 
    }

    [TestMethod]
    public void DependenciaConFechaMasTardia_DevuelveConFechaFinTempranoMasAlta()
    {
        var proyecto = new Proyecto("Proyecto", "Desc", DateTime.Today.AddDays(1));

        var dep1 = new Tarea("Dep1", "desc", 1, proyecto); 
        var dep2 = new Tarea("Dep2", "desc", 5, proyecto); 

        var tarea = new Tarea("Tarea Principal", "desc", 2, proyecto);

        tarea.AgregarDependencia(dep1);
        tarea.AgregarDependencia(dep2);

        var resultado = tarea.DependenciaConFechaMasTardia();

        Assert.AreEqual(dep2, resultado);
    }

    [TestMethod]
    public void Tarea_Realizada_SetDebeActualizarElValor()
    {
        var tarea = CrearTareaValida();
        tarea.Realizada = true;
        Assert.IsTrue(tarea.Realizada);
    }

    [TestMethod]
    public void Tarea_FechaEjecucion_SetDebeActualizarElValor()
    {
        var tarea = new Tarea("T1", "desc", 2, _proyecto);
        var fecha = DateTime.Today.AddDays(3);
        tarea.FechaEjecucion = fecha;
        Assert.AreEqual(fecha, tarea.FechaEjecucion);
    }

    [TestMethod]
    public void Tarea_DuracionFueAjustada_DetectaCambioDeDuracion()
    {
        var tarea = new Tarea("T1", "desc", 2, _proyecto);
        tarea.DuracionEnDias = 5;
        Assert.IsTrue(tarea.DuracionFueAjustada());
    }

    private Tarea CrearTareaValida()
    {
        return new Tarea("TareaTest", "desc", 3, _proyecto);
    }
    
    [TestMethod]
    public void QuitarTodasLasDependencias_EliminaTodasLasReferencias()
    {
        var proyecto = new Proyecto("Test", "Desc", DateTime.Today);
        var tareaA = new Tarea("A", "desc", 2, proyecto);
        var tareaB = new Tarea("B", "desc", 2, proyecto);
        var tareaC = new Tarea("C", "desc", 2, proyecto);

        tareaA.AgregarDependencia(tareaB);
        tareaA.AgregarDependencia(tareaC);

        Assert.AreEqual(2, tareaA.Dependencias.Count());

        tareaA.QuitarTodasLasDependencias();

        Assert.AreEqual(0, tareaA.Dependencias.Count());
    }
    
    [TestMethod]
    public void AgregarRecurso_AlmacenaRecursoCorrectamente()
    {
        var proyecto = new Proyecto("ProyectoTest", "Descripción", DateTime.Today);
        var tarea = new Tarea("Tarea A", "Descripción A", 3, proyecto);
        var recurso = new Recurso("Martillo", "Herramienta", "Recurso de prueba", "Fun");
        tarea.AgregarRecurso(recurso);
        
        Assert.AreEqual(1, tarea.Recursos.Count);
        Assert.AreEqual("Martillo", tarea.Recursos[0].Nombre);
        Assert.AreEqual("Herramienta", tarea.Recursos[0].Tipo);
        Assert.AreEqual("Recurso de prueba", tarea.Recursos[0].Descripcion);
    }
    
    [TestMethod]
    public void FechaFinTemprano_ConDuracion5Dias_EsCuatroDiasDespues()
    {
        var proyecto = new Proyecto("P", "Desc", new DateTime(2025, 6, 20));
        var tarea = new Tarea("T", "Desc", 5, proyecto);

        var fechaEsperada = new DateTime(2025, 6, 24);

        Assert.AreEqual(fechaEsperada, tarea.FechaFinTemprano);
    }
    [TestMethod]
    public void ObtenerRangoDeUso_ConFechasAsignadas_DevuelveRangoCorrecto()
    {
        var proyecto = new Proyecto("ProyectoTest", "Descripción", DateTime.Today);
        var tarea = new Tarea("Tarea A", "Descripción", 3, proyecto);
        var recurso = new Recurso("R1", "Tipo", "Desc", "Func");
    
        var fechaInicio1 = new DateTime(2025, 6, 20);
        var fechaFin1 = new DateTime(2025, 6, 21);
        var fechaInicio2 = new DateTime(2025, 6, 24);
        var fechaFin2 = new DateTime(2025, 6, 25);
    
        recurso.FechasDeUso.Add(new RangoFecha(fechaInicio1, fechaFin1) { Recurso = recurso });
        recurso.FechasDeUso.Add(new RangoFecha(fechaInicio2, fechaFin2) { Recurso = recurso });
    
        tarea.AgregarRecurso(recurso);
        tarea.RangosDeUso.AddRange(recurso.FechasDeUso);
    
        var rango = tarea.ObtenerRangoDeUso();
    
        Assert.IsNotNull(rango);
        Assert.AreEqual(fechaInicio1, rango?.desde);
        Assert.AreEqual(fechaFin2, rango?.hasta);
    }

}



