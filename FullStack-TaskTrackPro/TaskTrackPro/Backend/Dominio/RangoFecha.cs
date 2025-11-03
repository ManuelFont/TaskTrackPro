namespace Backend.Dominio;


public class RangoFecha
{
    public int Id { get; set; }  
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    
    public string TareaTitulo { get; set; }
    public Tarea Tarea { get; set; } 
    public int RecursoId { get; set; }
    public Recurso Recurso { get; set; }
    public RangoFecha() {}

    public RangoFecha(DateTime desde, DateTime hasta)
    {
        if (hasta <= desde)
            throw new ArgumentException("La fecha final debe ser posterior a la inicial.");

        Desde = desde;
        Hasta = hasta;
    }

    public bool SeSuperponeCon(DateTime inicio, DateTime fin)
    {
        return inicio < Hasta && fin > Desde;
    }
}

