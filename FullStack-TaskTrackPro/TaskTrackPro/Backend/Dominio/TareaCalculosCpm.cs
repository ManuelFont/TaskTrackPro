namespace Backend.Dominio;

public class TareaCalculosCpm
{
    private Tarea _tarea;

    public TareaCalculosCpm(Tarea tarea)
    {
        _tarea = tarea;
    }

    public bool EsCritica()
    {
        return Holgura() == 0;
    }

    public int Holgura()
    {
        TimeSpan diferencia = _tarea.FechaFinTarde - _tarea.FechaFinTemprano;
        return diferencia.Days;
    }

    public DateTime FechaInicioTemprano()
    {
        var fechaPorDependencias = !_tarea.Dependencias.Any()
            ? _tarea.Proyecto.FechaInicioEstimada
            : DependenciaConFechaFinMasTardia().FechaFinTemprano.AddDays(1);

        if (_tarea.FechaInicioForzada.HasValue)
            return (_tarea.FechaInicioForzada.Value > fechaPorDependencias)
                ? _tarea.FechaInicioForzada.Value
                : fechaPorDependencias;

        return fechaPorDependencias;
    }


    public DateTime FechaFinTemprano()
    {
        return FechaInicioTemprano().AddDays(_tarea.DuracionEnDias - 1);
    }


    public DateTime FechaInicioTarde()
    {
        return FechaFinTarde().AddDays(-(_tarea.DuracionEnDias - 1));
    }

    public DateTime FechaFinTarde()
    {
        if (!_tarea.Requeridores.Any())
        {
            var maxFin = _tarea.Proyecto.ListaDeTareas
                .Max(t => t.FechaFinTemprano);

            return maxFin;
        }

        
        var minInicioTarde = _tarea.Requeridores
            .Select(t => t.FechaInicioTarde)
            .Min();

        return minInicioTarde.AddDays(-1); 
    }
    

    
    private Tarea DependenciaConFechaFinMasTardia()
    {
        IEnumerable<Tarea> dependencias = _tarea.Dependencias;
        Tarea dConFechaMasTarde = dependencias.First();

        foreach (Tarea d in dependencias)
        {
            if (d.FechaFinTemprano > dConFechaMasTarde.FechaFinTemprano)
            {
                dConFechaMasTarde = d;
            }
        }

        return dConFechaMasTarde;
    }
    
}