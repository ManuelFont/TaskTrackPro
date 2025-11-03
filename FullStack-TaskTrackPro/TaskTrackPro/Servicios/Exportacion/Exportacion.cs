using Backend.Dominio;

namespace Servicios.Exportacion;

public class Exportacion
{
    protected List<ProyectoExportacion> ProyectosExportacion;
    protected string Direccion;
    public Exportacion(IList<Proyecto> proyectosSistema, string direccion)
    {
        ProyectosExportacion = new List<ProyectoExportacion>();
        foreach (Proyecto proyecto in proyectosSistema)
        {
            ProyectoExportacion proyectoExportacion =
                new ProyectoExportacion(proyecto);
            ProyectosExportacion.Add(proyectoExportacion);
        }

        Direccion = direccion;

        ProyectosExportacion.Sort();
    }
}