using Backend.Dominio;
namespace Servicios.Exportacion;

public class TareaExportacion : IComparable<TareaExportacion>
{
    public string Titulo { get; }
    public string? FechaInicio { get; }
    public string Duracion { get; }
    public string Critica { get; }
    public IList<string> Recursos { get; }
    
    public TareaExportacion(Tarea tarea)
    {
        Titulo = tarea.Titulo;
        FechaInicio = tarea.FechaInicioForzada?.ToString("dd/MM/yyyy") ?? null;
        Duracion = tarea.DuracionEnDias.ToString();
        Critica = tarea.EsCritica ? "S" : "N";
        Recursos = new List<string>();
        foreach (Recurso recurso in tarea.Recursos)
        {
            Recursos.Add(recurso.Nombre);
        }
    }
    
    public int CompareTo(TareaExportacion? otro)
    {
        if (otro == null)
            return 1;

        return string.Compare(otro.Titulo, Titulo, StringComparison.OrdinalIgnoreCase);
    }
}