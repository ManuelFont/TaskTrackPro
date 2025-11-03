namespace Dtos;

public class TareaDto
{
    public TareaDto() { }

    public TareaDto(string titulo, string descripcion, int duracion, DateTime fechaInicio, DateTime fechaFinTemprano, bool realizada, int holgura)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Duracion = duracion;
        FechaInicioTemprano = fechaInicio;
        FechaFinTemprano = fechaFinTemprano;
        Realizada = realizada;
        Holgura = holgura;
    }

    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public DateTime FechaInicioTemprano { get; set; }
    public DateTime FechaFinTemprano {get; set; }
    public bool Realizada;
    public int Holgura { get; set; } = 0;
    public bool Seleccionada { get; set; }
    public List<string> DependenciasNombres { get; set; } = new List<string>();
    public List<RecursoDto> Recursos { get; set; } = new List<RecursoDto>();
    public List<string> DependenciasTransitivas { get; set; } = new List<string>();
    public DateTime? FechaEjecucion { get; set; }   



}