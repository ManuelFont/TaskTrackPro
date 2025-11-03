using Backend.Dominio;
namespace Servicios.Exportacion;
public class ProyectoExportacion : IComparable<ProyectoExportacion>
{
    public string Nombre { get; }
    public string FechaInicio { get; }
    public List<TareaExportacion> Tareas { get; }

    public ProyectoExportacion(Proyecto proyecto)
    {
        Nombre = proyecto.Nombre;
        FechaInicio = proyecto.FechaInicioEstimada.ToString("dd/MM/yyyy");
        Tareas = new List<TareaExportacion>();
        foreach (Tarea tarea in proyecto.ListaDeTareas)
        {
            TareaExportacion tareaExportacion = new TareaExportacion(tarea);
            Tareas.Add(tareaExportacion);
        }

        Tareas.Sort((a,b) => b.CompareTo(a));
    }

    public int CompareTo(ProyectoExportacion? otro)
    {
        if (otro == null)
            return 1;
        
        DateTime esteFecha = DateTime.ParseExact(FechaInicio, "dd/MM/yyyy", null);
        DateTime otroFecha = DateTime.ParseExact(otro.FechaInicio, "dd/MM/yyyy", null);
        return DateTime.Compare(esteFecha, otroFecha);
    }
}