namespace Backend.Dominio;

public class Tarea
{
    private string _titulo;
    private string _descripcion;
    private Proyecto _proyecto;
    private bool _realizada;
    private DateTime? _fechaEjecucion;
    private int _duracionEnDias;
    private readonly int _duracionOriginal;
    private List<Tarea> _dependencias;
    private TareaCalculosCpm _tareaCalculosCpm;
    private List<Tarea> _requeridores;
    private List<Recurso> _recursos = new List<Recurso>();
    private DateTime? _fechaInicioForzada;
    public List<RangoFecha> RangosDeUso { get; set; } = new();


    public Proyecto Proyecto => _proyecto;
    public string Titulo { get => _titulo; set => _titulo = value; }
    public string Descripcion {get => _descripcion; set => _descripcion = value; }
    public DateTime? FechaEjecucion { get => _fechaEjecucion; set => _fechaEjecucion = value; }
    public int DuracionEnDias
    {
        get => _duracionEnDias;
        set => _duracionEnDias = value > 0 ? value : 
            throw new ArgumentOutOfRangeException(nameof(value), "Duración debe ser mayor a 0");
    }
    public DateTime? FechaInicioForzada
    {
        get => _fechaInicioForzada;
        set => _fechaInicioForzada = value;
    }
    public string ProyectoNombre { get; set; }
    public IEnumerable<Tarea> Dependencias => _dependencias;
    public IEnumerable<Tarea> Requeridores => _requeridores;
    public bool EsCritica => _tareaCalculosCpm.EsCritica();
    public int Holgura => _tareaCalculosCpm.Holgura();
    public DateTime FechaInicioTemprano
    {
        get
        {
            if (_fechaInicioForzada.HasValue)
                return _fechaInicioForzada.Value;

            return _tareaCalculosCpm.FechaInicioTemprano();
        }
    }
    public DateTime FechaFinTemprano => _tareaCalculosCpm.FechaFinTemprano();
    public DateTime FechaInicioTarde => _tareaCalculosCpm.FechaInicioTarde();
    public DateTime FechaFinTarde => _tareaCalculosCpm.FechaFinTarde();
    public bool Realizada
    {
        get => _realizada;
        set => _realizada = value;
    }
    public List<Recurso> Recursos {get => _recursos;}

    public Tarea() 
    {
        _tareaCalculosCpm = new TareaCalculosCpm(this);
        _dependencias = new List<Tarea>();
        _requeridores = new List<Tarea>(); 
    } 

    public Tarea(string titulo, string descripcion, int duracionEnDias, Proyecto proyecto)
    {
        if (!TareaValida(titulo, DateTime.Today, duracionEnDias))
        {
            throw new ArgumentException("Hay errores en los campos");
        }

        _dependencias = new List<Tarea>();
        _requeridores = new List<Tarea>();
        _titulo = titulo;
        _descripcion = descripcion;
        _duracionEnDias = duracionEnDias;
        _duracionOriginal = duracionEnDias;
        ProyectoNombre = proyecto.Nombre;
        _proyecto = proyecto;
        _tareaCalculosCpm = new TareaCalculosCpm(this);
        _dependencias = new List<Tarea>();
    }

    public Tarea(string titulo, string descripcion, int duracionEnDias, Proyecto proyecto, DateTime fechaInicioEstimada)
        : this(titulo, descripcion, duracionEnDias, proyecto)
    {
        if (!TareaValida(titulo, fechaInicioEstimada, duracionEnDias))
        {
            throw new ArgumentException("Hay errores en los campos de Tarea");
        } 
    }

    private static bool TareaValida(string titulo, DateTime fechaInicioEstimada, int duracionEnDias)
    {
        return !(string.IsNullOrWhiteSpace(titulo))
               && (fechaInicioEstimada.Date >= DateTime.Now.Date)
               && duracionEnDias > 0;
    }

    private bool TodasLasDependenciasRealizadas()
    {
        return _dependencias.All(t => t.Realizada);
    }
    
    public void AgregarRecurso(Recurso recurso)
    {
        _recursos.Add(recurso);
        if (!recurso.Tareas.Contains(this))
        {
            recurso.Tareas.Add(this);
        }
        
    }
    public DateTime FechaFinReal()
    {
        if (!_realizada || !_fechaEjecucion.HasValue)
            throw new InvalidOperationException("La tarea no ha sido marcada como realizada.");

        return _fechaEjecucion.Value;
    }
    
    public bool DuracionFueAjustada()
    {
        return _duracionEnDias != _duracionOriginal;
    }

    public void MarcarTareaRealizada(DateTime fechaEjecucion)
    {
        ValidarMarcarTareaRealizada(fechaEjecucion);
        _realizada = true;
        _fechaEjecucion = fechaEjecucion;
    }

    private void ValidarMarcarTareaRealizada(DateTime fechaEjecucion)
    {
        if (!TodasLasDependenciasRealizadas())
        {
            throw new ArgumentException("No todas sus tareas fueron realizadas");
        }

        if (fechaEjecucion < Proyecto.FechaInicioEstimada)
        {
            throw new ArgumentException("La Tarea no puede ser ejecutada antes de que inicie el proyecto");
        }
    }

    public void AgregarRequeridor(Tarea requeridor)
    {
        if (requeridor.Proyecto != Proyecto)
            throw new ArgumentException("El requeridor no pertenece al mismo proyecto");

        if (!requeridor.Dependencias.Contains(this))
            throw new ArgumentException("El requeridor no me tiene como dependencia");

        _requeridores.Add(requeridor);
    }

    public void AgregarDependencia(Tarea dependencia)
    {
        ValidarAgregarDependencias(dependencia);
        _dependencias.Add(dependencia);
        dependencia.AgregarRequeridor(this);
    }

    private void ValidarAgregarDependencias(Tarea dependencia)
    {
        if (dependencia.Proyecto != Proyecto)
            throw new ArgumentException("La dependencia no pertenece al mismo proyecto");

        if (dependencia == this)
            throw new ArgumentException("Una Tarea no puede ser dependencia de sí misma");
    }
    
    public Tarea DependenciaConFechaMasTardia()
    {
        if (_dependencias.Count <= 0)
            throw new InvalidOperationException("No hay tareas en el proyecto");

        Tarea dMasTardia = Dependencias.First();
        foreach (Tarea d in Dependencias)
        {
            if (d.FechaFinTemprano > dMasTardia.FechaFinTemprano)
                dMasTardia = d;
        }

        return dMasTardia;
    }
    
    public void Editar(string nuevaDescripcion, int nuevaDuracion)
    {
        if (string.IsNullOrWhiteSpace(nuevaDescripcion) || nuevaDuracion <= 0)
            throw new ArgumentException("Datos inválidos para la edición de tarea.");

        _descripcion = nuevaDescripcion;
        _duracionEnDias = nuevaDuracion;
    }
    public void QuitarTodasLasDependencias()
    {
        foreach (var d in _dependencias.ToList())
        {
            _dependencias.Remove(d);
        }
    }
    
    public void EstablecerFechaEjecucion(DateTime nuevaFecha)
    {
        if (nuevaFecha < DateTime.Today)
            throw new ArgumentException("La fecha de ejecución no puede ser en el pasado.");

        _fechaEjecucion = nuevaFecha;
    }
    
    public void EstablecerInicioForzado(DateTime fecha)
    {
        if (fecha < Proyecto.FechaInicioEstimada)
            throw new ArgumentException("La fecha no puede ser anterior al inicio del proyecto.");

        _fechaInicioForzada = fecha;
    }
    public (DateTime desde, DateTime hasta)? ObtenerRangoDeUso()
    {
        if (!RangosDeUso.Any())
            return null;

        return (RangosDeUso.Min(r => r.Desde), RangosDeUso.Max(r => r.Hasta));
    }


}
