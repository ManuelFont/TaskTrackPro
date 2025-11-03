namespace Dtos;

public class ProyectoDto
{
    public string NombreProyecto { get; set; }
    public string DescripcionProyecto { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<UsuarioDto> MiembrosProyecto { get; set; }
    public UsuarioDto? LiderProyecto { get; set; }
    
}