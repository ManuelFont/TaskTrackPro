using Backend.Dominio;
using DataAccess;
using Dtos;
using InterfacesDataAccess;

namespace Servicios;

public class ServicioRecurso
{
    private IRepositorio<Recurso> _repoRecursos;
    private IRepositorio<Proyecto> _repoProyectos;
    private ServicioNotificaciones _servicioNotificaciones;
    private IRepositorioTarea<Tarea> _repositorioTareas;
    private IRepositorio<Notificacion> _repoNoti;

    public ServicioRecurso(IRepositorio<Recurso> repositorioRecursos, IRepositorio<Proyecto> repoProyectos, 
        ServicioNotificaciones servicioNotificaciones, IRepositorioTarea<Tarea> repositorioTareas, 
        IRepositorio<Notificacion> repoNotificaciones)
    {
        _repoRecursos = repositorioRecursos;
        _repoProyectos = repoProyectos;
        _servicioNotificaciones = servicioNotificaciones;
        _repositorioTareas = repositorioTareas;
        _repoNoti = repoNotificaciones;

    }

    public IList<RecursoDto> TraerTodo()
    {
        IList<RecursoDto> recursosDto = new List<RecursoDto>();
        foreach (Recurso recurso in _repoRecursos.TraerTodos())
        {
            RecursoDto recursoDto = new RecursoDto();
            recursoDto.Id = recurso.Id;
            recursoDto.Nombre = recurso.Nombre;
            recursoDto.Descripcion = recurso.Descripcion;
            recursoDto.Tipo = recurso.Tipo;
            recursoDto.Funcionalidad = recurso.Funcionalidad;
            recursoDto.ProyectoNombre = recurso.Proyecto == null ? "" : recurso.Proyecto.Nombre;
            recursoDto.Capacidad = recurso.Capacidad;
            recursoDto.Usos = recurso.Usos;


            recursosDto.Add(recursoDto);
        }
        return recursosDto;
    }

    public void Agregar(RecursoDto recursoDto)
    {
        Recurso recurso;
        if (recursoDto.ProyectoNombre == null)
        {
            recurso = new Recurso(recursoDto.Nombre, recursoDto.Tipo, recursoDto.Descripcion, recursoDto.Funcionalidad);
            recurso.AsignarCapacidad(recursoDto.Capacidad); 
            _repoRecursos.Agregar(recurso);
        }
        else
        {
            Proyecto proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == recursoDto.ProyectoNombre)
                                ?? throw new KeyNotFoundException("Proyecto no encontrado");

            recurso = new Recurso(recursoDto.Nombre, recursoDto.Tipo, recursoDto.Descripcion, recursoDto.Funcionalidad, proyecto);
            recurso.Capacidad = recursoDto.Capacidad;

            
            _repoRecursos.Agregar(recurso);
            proyecto.Recursos.Add(recurso);
            _repoProyectos.Actualizar(proyecto);

        }
    }
    
    private void AsignarATarea(int idRecurso, Tarea tarea, int duracion, DateTime fecha_asignacion)
    {
        if (duracion > tarea.DuracionEnDias)
        {
            throw new InvalidOperationException(
                $"La tarea duración del uso del recurso no puede ser mayor al de la tarea"); 
        }

        if (tarea.EsCritica)
        {
            var fechaFinAsignacion = fecha_asignacion.AddDays(duracion - 1);
            var inicio = tarea.FechaInicioTemprano;
            var fin = tarea.FechaFinTemprano;

            if (fecha_asignacion < inicio || fechaFinAsignacion > fin)
            {
                throw new InvalidOperationException(
                    $"La tarea '{tarea.Titulo}' es crítica y el recurso debe estar disponible entre el {inicio:dd/MM/yyyy} y el {fin:dd/MM/yyyy}.");
            }
        }

        if (!tarea.EsCritica)
        {
            var inicioTarde = tarea.FechaInicioTarde;
            if (fecha_asignacion > inicioTarde)
            {
                throw new InvalidOperationException($"La tarea '{tarea.Titulo}' solo puede postergarse hasta el {inicioTarde:dd/MM/yyyy} sin afectar el proyecto.");
            }
        }
        
        Recurso recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                          ?? throw new KeyNotFoundException("Recurso no encontrado");

        if (recurso.Disponible(fecha_asignacion, duracion))
        {
            recurso.AsignarFechaDeUso(fecha_asignacion, duracion, tarea);
            tarea.AgregarRecurso(recurso);
            _repoRecursos.Actualizar(recurso);
            NotificarAsignacionExitosa(recurso, tarea);
        }
        else
        {
            throw new InvalidOperationException($"El recurso '{recurso.Nombre}' no está disponible desde el {fecha_asignacion:dd/MM/yyyy} durante {duracion} días.");
        }


        _repoProyectos.Actualizar(tarea.Proyecto);
    }

    public void AsignarRecursoATarea(string nombreProyecto, string tituloTarea, int idRecurso , int duracion, DateTime fecha_asignacion)
    {
        Proyecto proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto)
                            ?? throw new ArgumentException("Proyecto no encontrado");

        Tarea tarea = proyecto.BuscarTareaPorNombre(tituloTarea);
        if (tarea == null)
            throw new ArgumentException($"No se encontró la tarea '{tituloTarea}' en el proyecto '{nombreProyecto}'.");

        AsignarATarea(idRecurso, tarea, duracion, fecha_asignacion);
    }


    public void AgregarRecursoProyecto(RecursoDto recursoDto)
    {
        Recurso recurso;
        Proyecto proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == recursoDto.ProyectoNombre) 
                            ?? throw new KeyNotFoundException("Proyecto no encontrado");
        recurso = new Recurso(recursoDto.Nombre, recursoDto.Tipo,recursoDto.Descripcion,recursoDto.Funcionalidad,proyecto);
        recurso.AsignarCapacidad(recursoDto.Capacidad);

        
        proyecto.AgregarRecurso(recurso);
        _repoRecursos.Agregar(recurso);
        _repoProyectos.Actualizar(proyecto); 
    }
    
    private void ReagendarTareaPorConflicto(Recurso recurso, Tarea tarea, DateTime fecha_asignacion, int duracion)
    {
        if (tarea.EsCritica)
            throw new InvalidOperationException($"La tarea '{tarea.Titulo}' es crítica y no se puede reagendar.");

        DateTime nuevaFechaInicio = recurso.ProximaDisponibilidad(fecha_asignacion, duracion);
        DateTime nuevoFin = nuevaFechaInicio.AddDays(duracion - 1);

        recurso.AsignarFechaDeUso(nuevaFechaInicio, duracion, tarea);
        tarea.EstablecerInicioForzado(nuevaFechaInicio);
        tarea.AgregarRecurso(recurso);

        Tarea? tareaQueDebeSerDependencia = null;
        DateTime? finUltimaTarea = null;

        foreach (var otraTarea in recurso.Tareas.Where(t => t != tarea))
        {
            var rango = otraTarea.ObtenerRangoDeUso();
            if (rango != null)
            {
                var (inicioOtra, finOtra) = rango.Value;

                if (finOtra < nuevaFechaInicio && 
                    (!finUltimaTarea.HasValue || finOtra > finUltimaTarea))
                {
                    tareaQueDebeSerDependencia = otraTarea;
                    finUltimaTarea = finOtra;
                }
            }
        }

        if (tareaQueDebeSerDependencia != null &&
            !tarea.Dependencias.Contains(tareaQueDebeSerDependencia))
        {
            tarea.AgregarDependencia(tareaQueDebeSerDependencia);
        }

        _repositorioTareas.Actualizar(tarea);
        _repoProyectos.Actualizar(tarea.Proyecto);

        var lider = tarea.Proyecto.Lider;
        if (lider != null)
        {
            string mensaje = $"Conflicto: El recurso '{recurso.Nombre}' no estaba disponible para la tarea '{tarea.Titulo}'. La tarea fue reagendada al {nuevaFechaInicio:dd/MM/yyyy}.";
            _servicioNotificaciones.Notificar(mensaje, lider);
        }
    }


    

    public List<Recurso> BuscarRecursosEquivalentesDisponibles(Recurso recursoBase, DateTime fecha, int duracion)
    {
        var recursos = _repoRecursos.TraerTodos()
            .Where(r => r.Id != recursoBase.Id &&
                        r.Tipo == recursoBase.Tipo &&
                        r.Funcionalidad == recursoBase.Funcionalidad &&
                        r.Disponible(fecha, duracion))
            .ToList();

        return recursos;
    }
    
    private void NotificarAsignacionExitosa(Recurso recurso, Tarea tarea)
    {
        var lider = tarea.Proyecto.Lider;
        if (lider != null)
        {
            string mensaje = $"Recurso '{recurso.Nombre}' asignado correctamente a la tarea '{tarea.Titulo}'.";
            _servicioNotificaciones.Notificar(mensaje, lider);
        }
    }


    public void Actualizar(RecursoDto recurso)
    {
        Recurso recursoActualizar = _repoRecursos.EncontrarElemento(r => r.Id == recurso.Id)
                                    ?? throw new KeyNotFoundException("Recurso no encontrado");
        
        recursoActualizar.Editar(
            recurso.Nombre,
            recurso.Descripcion,
            recurso.Funcionalidad,
            recurso.Capacidad
        );

        if (!string.IsNullOrEmpty(recurso.ProyectoNombre))
        {
            Proyecto? proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == recurso.ProyectoNombre);
            if (proyecto == null)
                throw new Exception($"No se encontró el proyecto con nombre '{recurso.ProyectoNombre}'.");

            recursoActualizar.Proyecto = proyecto;
            recursoActualizar.ProyectoNombre = proyecto.Nombre;
        }
        else
        {
            recursoActualizar.Proyecto = null;
            recursoActualizar.ProyectoNombre = null;
        }

        _repoRecursos.Actualizar(recursoActualizar);
    }

    
    public bool HayEquivalentesDisponibles(int recursoSeleccionadoId, DateTime fechaAsignacion, int duracion)
    {
        Recurso recurso = _repoRecursos.EncontrarElemento(r => r.Id == recursoSeleccionadoId)
                          ?? throw new KeyNotFoundException("Recurso no encontrado");
        List<Recurso> equivalentes = BuscarRecursosEquivalentesDisponibles(recurso, fechaAsignacion, duracion).ToList();
        return equivalentes.Count() > 0;

    }
    
    public void RedistribuirRecursoEquivalente(string nombreProyecto, string tituloTarea, int idRecurso, int duracion, DateTime fecha_asignacion)
    {
        Proyecto proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto)
                            ?? throw new ArgumentException("Proyecto no encontrado");

        Tarea tarea = proyecto.BuscarTareaPorNombre(tituloTarea)
                      ?? throw new ArgumentException($"No se encontró la tarea '{tituloTarea}' en el proyecto '{nombreProyecto}'.");

       

        Recurso recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                          ?? throw new KeyNotFoundException("Recurso no encontrado");

        var equivalentes = BuscarRecursosEquivalentesDisponibles(recurso, fecha_asignacion, duracion)
            .Where(eq => !eq.Tareas.Any(t => t.Titulo == tarea.Titulo))
            .ToList();        
        if (!equivalentes.Any())
            throw new InvalidOperationException("No hay recursos equivalentes disponibles para redistribuir.");

        var reemplazo = equivalentes.First();
        reemplazo.AsignarFechaDeUso(fecha_asignacion, duracion, tarea);
        tarea.AgregarRecurso(reemplazo);

        NotificarAsignacionExitosa(reemplazo, tarea);
        _repoProyectos.Actualizar(tarea.Proyecto);
    }
    public void ReprogramarTareaPorConflicto(string nombreProyecto, string tituloTarea, int idRecurso, int duracion, DateTime fecha_asignacion)
    {
        
        Proyecto proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto)
                            ?? throw new ArgumentException("Proyecto no encontrado");

        Tarea tarea = proyecto.BuscarTareaPorNombre(tituloTarea)
                      ?? throw new ArgumentException($"No se encontró la tarea '{tituloTarea}' en el proyecto '{nombreProyecto}'.");

        Recurso recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                          ?? throw new KeyNotFoundException("Recurso no encontrado");

        ReagendarTareaPorConflicto(recurso, tarea, fecha_asignacion, duracion);
    }

    public List<RecursoDto> TraerTodosLosRecursosDelSistema()
    {
        var recursos = _repoRecursos.TraerTodos();

        return recursos.Select(r => new RecursoDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Tipo = r.Tipo,
            Descripcion = r.Descripcion,
            Funcionalidad = r.Funcionalidad,
            ProyectoNombre = r.ProyectoNombre,
            Capacidad = r.Capacidad,
            Usos = r.FechasDeUso.Count,
            FechasDeUso = r.FechasDeUso.Select(f => new RangoFechaDto
            {
                Desde = f.Desde,
                Hasta = f.Hasta
            }).ToList()
        }).ToList();
    }



    public List<RecursoDto> TraerRecursosDelProyecto(string nombreProyecto)
    {
        var recursos = _repoRecursos.EncontrarLista(r => r.ProyectoNombre == nombreProyecto);

        return recursos.Select(r => new RecursoDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Tipo = r.Tipo,
            Descripcion = r.Descripcion,
            Funcionalidad = r.Funcionalidad,
            ProyectoNombre = r.ProyectoNombre,
            Capacidad = r.Capacidad,
            Usos = r.FechasDeUso?.Count ?? 0  
        }).ToList();
    }
    
    public void EliminarRecursoPorId(int idRecurso)
    {
        var recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                      ?? throw new KeyNotFoundException("Recurso no encontrado");

        if (recurso.Proyecto != null)
            throw new InvalidOperationException("No se puede eliminar un recurso asignado a un proyecto");

        _repoRecursos.Eliminar(r => r.Id == idRecurso);
    }
    
    public RecursoDto TraerPorId(int id)
    {
        var recurso = _repoRecursos.EncontrarElemento(r => r.Id == id)
                      ?? throw new KeyNotFoundException("Recurso no encontrado");

        return new RecursoDto
        {
            Id = recurso.Id,
            Nombre = recurso.Nombre,
            Tipo = recurso.Tipo,
            Descripcion = recurso.Descripcion,
            Funcionalidad = recurso.Funcionalidad,
            ProyectoNombre = recurso.ProyectoNombre,
            Capacidad = recurso.Capacidad,
            Usos = recurso.FechasDeUso.Count,
            FechasDeUso = recurso.FechasDeUso.Select(f => new RangoFechaDto
            {
                Desde = f.Desde,
                Hasta = f.Hasta
            }).ToList()
        };
    }
    
    public bool RecursoDisponible(int idRecurso, DateTime fechaInicio, int duracion)
    {
        var recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                      ?? throw new KeyNotFoundException("Recurso no encontrado");

        return recurso.Disponible(fechaInicio, duracion);
    }

    public void AsignarRecursoIgnorandoDisponibilidad(string nombreProyecto, string tituloTarea, int idRecurso, int duracion, DateTime fechaInicio)
    {
        var proyecto = _repoProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto)
                       ?? throw new Exception("Proyecto no encontrado");

        var recurso = _repoRecursos.EncontrarElemento(r => r.Id == idRecurso)
                      ?? throw new Exception("Recurso no encontrado");

        var tarea = _repositorioTareas.EncontrarElemento(t => t.Titulo == tituloTarea && t.Proyecto.Nombre == nombreProyecto)
                    ?? throw new Exception("Tarea no encontrada");
        
        var rango = new RangoFecha
        {
            Desde = fechaInicio,
            Hasta = fechaInicio.AddDays(duracion - 1),
            Recurso = recurso,
            RecursoId = recurso.Id,
            Tarea = tarea,
            TareaTitulo = tarea.Titulo
        };
        
        recurso.FechasDeUso.Add(rango);
        
        if (!recurso.Tareas.Contains(tarea))
            recurso.Tareas.Add(tarea);
        if (!tarea.Recursos.Contains(recurso))
            tarea.Recursos.Add(recurso);
        
        tarea.EstablecerFechaEjecucion(fechaInicio);
        
        
        _repoRecursos.Actualizar(recurso);
        _repositorioTareas.Actualizar(tarea);
    }



}