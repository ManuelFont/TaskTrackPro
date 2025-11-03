using System.Text;
using Backend.Dominio;

namespace Servicios.Exportacion;

public class ExportacionCsv : Exportacion
{
    public ExportacionCsv(IList<Proyecto> proyectosSistema, string direccion, string nombre) : base(proyectosSistema, direccion)
    {
        Direccion = direccion + "/" + nombre + ".csv";
    }

    public void CrearArchivo()
    {
        File.WriteAllText(Direccion, Serializar());
    }

    private string Serializar()
    {
        StringBuilder sb = new StringBuilder();

        foreach (ProyectoExportacion proyectoExpo in ProyectosExportacion)
        {
            sb.Append(proyectoExpo.Nombre + "," + proyectoExpo.FechaInicio);
            sb.AppendLine();
            foreach (TareaExportacion tareaExpo in proyectoExpo.Tareas)
            {
                sb.Append(tareaExpo.Titulo + "," + (tareaExpo.FechaInicio??"n/a") + "," + tareaExpo.Critica + "," 
                             + tareaExpo.Duracion);
                sb.AppendLine();
                foreach (string recurso in tareaExpo.Recursos)
                {
                    sb.Append(recurso);
                    sb.AppendLine();
                }
            }
        }
        
        return sb.ToString();
    }
}