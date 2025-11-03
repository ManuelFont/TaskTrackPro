using System.ComponentModel.DataAnnotations;

namespace Backend.Dominio;

public class Recurso
{

    [Key] public int Id { get; set; }
    [Required] public string Nombre { get; set; }
    [Required] public string Tipo { get; set; }
    [Required] public string Descripcion { get; set; }
    public string? ProyectoNombre { get; set; }
    public Proyecto? Proyecto { get; set; }
    public string Funcionalidad { get; set; }
    public DateTime? UtilizadoHasta { get; set; }
    public List<Tarea> Tareas { get; set; } = new();
    
    public int Capacidad { get; set; } = 1;
    public List<RangoFecha> FechasDeUso { get; set; } = new();
    
    public int Usos { get; set; }


    

    public Recurso() {}

    public Recurso(string nombre, string tipo, string descripcion, string funcionalidad)
    {
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
        Funcionalidad = funcionalidad;
    }

    public Recurso(string nombre, string tipo, string descripcion, string funcionalidad, Proyecto proyecto)
    {
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
        Funcionalidad = funcionalidad;
        ProyectoNombre = proyecto.Nombre;
        Proyecto = proyecto;
    }

    public void AsignarFuncionalidad(string funcionalidad)
    {
        Funcionalidad = funcionalidad;
    }
    
    public void AsignarCapacidad(int capacidad)
    {
        if (capacidad < 1)
            throw new ArgumentException("La capacidad debe ser al menos 1.");
        Capacidad = capacidad;
    }


    public void AsignarFechaDeUso(DateTime fechaInicio, int duracion, Tarea tarea)
    {
        if (duracion <= 0)
            throw new InvalidOperationException("La duración debe ser mayor a 0.");

        if (fechaInicio < DateTime.Today)
            throw new InvalidOperationException("No se puede usar el recurso en el pasado.");

        if (!Disponible(fechaInicio, duracion))
            throw new InvalidOperationException("El recurso no está disponible en esa fecha.");

        FechasDeUso.Add(new RangoFecha
        {
            Desde = fechaInicio,
            Hasta = fechaInicio.AddDays(duracion - 1),
            Recurso = this,
            Tarea = tarea
        });
    }
    
    public bool Disponible(DateTime fechaInicio, int duracion)
    {
        var nuevoFin = fechaInicio.AddDays(duracion - 1);

        for (DateTime dia = fechaInicio; dia <= nuevoFin; dia = dia.AddDays(1))
        {
            int usosEseDia = FechasDeUso.Count(r => r.Desde <= dia && r.Hasta >= dia);
            if (usosEseDia >= Capacidad)
            {
                return false;
            }
        }

        return true;
    }

    
    public DateTime ProximaDisponibilidad(DateTime desde, int duracion)
    {
        if (duracion <= 0)
            throw new ArgumentException("La duración debe ser mayor a 0.");

        for (var fecha = desde; fecha < desde.AddYears(1); fecha = fecha.AddDays(1))
        {
            if (Disponible(fecha, duracion))
                return fecha;
        }

        throw new InvalidOperationException("No hay disponibilidad en el próximo año.");
    }

    public void DesasignarDeProyecto() 
    {
        Proyecto = null!;
    }
    
    public void Editar(string nombre, string descripcion, string funcionalidad, int capacidad)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Funcionalidad = funcionalidad;
        Capacidad = capacidad;
    }



    
    
}
