namespace Dtos;

public class RecursoDto
{
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public bool Seleccionado { get; set; }
    public string? ProyectoNombre { get; set; }
    public string Funcionalidad{get;set;}
    public DateTime? FechaAsignacion { get; set; }
    public int Id { get; set; }
    
    public int Capacidad { get; set; }
    public int Usos { get; set; }
    public List<RangoFechaDto> FechasDeUso { get; set; } = new();


}