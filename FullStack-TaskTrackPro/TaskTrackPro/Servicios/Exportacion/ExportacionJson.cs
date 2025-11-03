using System.Text.Json;
using Backend.Dominio;

namespace Servicios.Exportacion;

public class ExportacionJson : Exportacion
{
    private JsonSerializerOptions _jsonOptions;
    public ExportacionJson(IList<Proyecto> proyectosSistema, string direccion, string nombre) : base(proyectosSistema, direccion)
    {
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.WriteIndented = true;
        Direccion = direccion + "/" + nombre + ".json";
    }

    public void CrearArchivo()
    {
        File.WriteAllText(Direccion, Serializar());
    }
    
    public string Serializar()
    { 
        return JsonSerializer.Serialize(ProyectosExportacion, _jsonOptions);
    }
}