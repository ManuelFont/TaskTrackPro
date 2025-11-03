using Backend.Dominio;
using Dtos;
using InterfacesDataAccess;
using Servicios.Exportacion;

namespace Servicios;

public class ServicioProyecto
{
    private IRepositorio<Proyecto> _repositorioProyectos;
    private IRepositorio<Usuario> _repositorioUsuarios;
    private ServicioUsuario  _servicioUsuario;
    private IRepositorio<Notificacion> _repoNotificaciones;
    private IRepositorio<Recurso> _repoRecursos;
    private IRepositorioTarea<Tarea> _repositorioTareas;
    private readonly ServicioNotificaciones _servicioNotificaciones;
    
    public ServicioProyecto(
        IRepositorio<Proyecto> repositorioProyectos,
        IRepositorio<Usuario> repositorioUsuarios,
        ServicioUsuario servicioUsuario,
        IRepositorio<Notificacion> repoNotificaciones,
        ServicioNotificaciones servicioNotificaciones,
        IRepositorioTarea<Tarea> repositorioTareas)
    {
        _repositorioProyectos = repositorioProyectos;
        _repositorioUsuarios  = repositorioUsuarios;
        _servicioUsuario = servicioUsuario;
        _repoNotificaciones = repoNotificaciones;
        _servicioNotificaciones = servicioNotificaciones;
        _repositorioTareas = repositorioTareas;
        

    }

    public void CrearArchivoJson(string direccion, string nombre)
    {
        ExportacionJson exportacionJson = new ExportacionJson(_repositorioProyectos.TraerTodos(), direccion, nombre);
        exportacionJson.CrearArchivo();
    }

    public void CrearArchivoCsv(string direccion, string nombre)
    {
        ExportacionCsv exportacionJson = new ExportacionCsv(_repositorioProyectos.TraerTodos(), direccion, nombre);
        exportacionJson.CrearArchivo();
    }
    
    public void CrearProyecto(ProyectoDto proyectoDto)
    {
        ValidarCamposProyecto(proyectoDto);

        Proyecto nuevoProyecto = new Proyecto(
            proyectoDto.NombreProyecto,
            proyectoDto.DescripcionProyecto,
            proyectoDto.FechaInicio
        );
        string emailLider = proyectoDto.LiderProyecto.Email;
        Usuario lider = _repositorioUsuarios.EncontrarElemento(u => u.Email == emailLider)
                        ?? throw new KeyNotFoundException("Usuario no encontrado");
        nuevoProyecto.AsignarLider(lider);
        if (_repoNotificaciones != null)
        {
            string mensaje = $"Fuiste asignado como Líder del proyecto \"{nuevoProyecto.Nombre}\".";
            var noti = new Notificacion(mensaje, lider.Email);
            _repoNotificaciones.Agregar(noti);
        }


        List<Usuario> listaUsuariosAgregar = new List<Usuario>();
        foreach (var usuario in proyectoDto.MiembrosProyecto)
        {
            Usuario user = _repositorioUsuarios.EncontrarElemento(u => u.Email == usuario.Email)
                           ?? throw new KeyNotFoundException("Usuario no encontrado");

            listaUsuariosAgregar.Add(user);
            _servicioNotificaciones.Notificar($"Fuiste agregado al proyecto \"{proyectoDto.NombreProyecto}\".", user);
        }


        nuevoProyecto.AgregarMiembroAlProyecto(_servicioUsuario.UsuarioLogueado);
        _servicioUsuario.AgregarProyectoUsuarioLogueado(nuevoProyecto);

        foreach (var u in listaUsuariosAgregar)
        {
            u.AgregarProyecto(nuevoProyecto);
            nuevoProyecto.AgregarMiembroAlProyecto(u);
        }

        _repositorioProyectos.Agregar(nuevoProyecto);
    }

    private void ValidarCamposProyecto(ProyectoDto proyectoDto)
    {
        if (proyectoDto == null)
            throw new ArgumentException("proyectoDto es null");

        if (string.IsNullOrWhiteSpace(proyectoDto.NombreProyecto))
            throw new ArgumentException("El nombre del proyecto no puede estar vacío");

        if (string.IsNullOrWhiteSpace(proyectoDto.DescripcionProyecto))
            throw new ArgumentException("La descripción del proyecto no puede estar vacía");

        if (proyectoDto.MiembrosProyecto == null)
            throw new ArgumentException("MiembrosProyecto es null");

        if (_servicioUsuario.UsuarioLogueado == null)
            throw new ArgumentException("No hay usuario logueado");

        foreach (var usuario in proyectoDto.MiembrosProyecto)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Email))
                throw new ArgumentException("Hay un miembro con Email nulo o vacío");

            Usuario user = _repositorioUsuarios.EncontrarElemento(u => u.Email == usuario.Email);
            if (user == null)
                throw new ArgumentException($"No existe el usuario con email: {usuario.Email}");
        }
    }


    public ProyectoDto TraerProyectoPorNombre(string nombreProyecto)
    {
        Proyecto proyecto = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                            ?? throw new KeyNotFoundException("Proyecto no encontrado");
        ProyectoDto pDto = new ProyectoDto();
        pDto.NombreProyecto = proyecto.Nombre;
        pDto.DescripcionProyecto = proyecto.Descripcion;
        pDto.FechaInicio = proyecto.FechaInicioEstimada;
        pDto.FechaFin = proyecto.FechaFinEstimada();
        pDto.MiembrosProyecto = proyecto.ListaUsuarios
            .Select(u => new UsuarioDto
            {
                Nombre   = u.Nombre,
                Apellido = u.Apellido,
                Email    = u.Email
            })
            .ToList();
        return pDto;
    }
    
    public void AgregarMiembroAProyecto(string nombreProyecto, string uEmail)
    {
        Proyecto p = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                            ?? throw new KeyNotFoundException("Proyecto no encontrado");
        Usuario agregar = _repositorioUsuarios.EncontrarElemento(u => u.Email == uEmail);
        p.AgregarMiembroAlProyecto(agregar);
        agregar.AgregarProyecto(p);

        _servicioNotificaciones.Notificar($"Fuiste agregado al proyecto \"{p.Nombre}\".", agregar);

    }
    public void CrearYAgregarTareaAProyecto(string nombreProyecto, TareaDto tareaDto)
    {
        try
        {
            Proyecto p = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                                ?? throw new KeyNotFoundException("Proyecto no encontrado");
            
            Tarea tarea = new Tarea(tareaDto.Titulo, tareaDto.Descripcion, tareaDto.Duracion, p);
            foreach (string dNombre in tareaDto.DependenciasNombres)
            {
                tarea.AgregarDependencia(p.BuscarTareaPorNombre(dNombre));
            }
            p.AgregarTarea(tarea);
            _repositorioTareas.Agregar(tarea);
            _repositorioProyectos.Actualizar(p);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al crear la tarea: {ex.Message}");
        }
    }
    
    public List<UsuarioDto> TraerUsuariosSinElProyecto(ProyectoDto proyectoDto)
    {
        if (proyectoDto == null) 
            throw new ArgumentNullException(nameof(proyectoDto));

        var resultado = new List<UsuarioDto>();

        foreach (var u in _repositorioUsuarios.TraerTodos())
        {
            bool yaTiene = u.ListaProyectos.Any(p => p.Nombre.Equals(proyectoDto.NombreProyecto, StringComparison.OrdinalIgnoreCase));
            if (!yaTiene)
            {
                resultado.Add(new UsuarioDto {Nombre = u.Nombre, Apellido = u.Apellido, Email = u.Email});
            }
        }

        return resultado;
    }

    public DateTime TraerFechaFinProyecto(string nombreProyecto)
    {
        Proyecto p = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                     ?? throw new KeyNotFoundException("Proyecto no encontrado");
        return p.FechaFinEstimada();
    }
    
    public List<List<TareaDto>> CaminoCritico(string nombreProyecto)
    {
        
        Proyecto proyecto = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                     ?? throw new KeyNotFoundException("Proyecto no encontrado");

        var todas = proyecto.ListaDeTareas
            .ToList();
        var todasDto = todas.Select(t => MapearTareaDto(t)).ToList();
        
        var criticas = todasDto.Where(t => t.Holgura == 0).ToList();
        
        var iniciales = criticas
            .Where(t => !criticas.Any(o => t.DependenciasNombres.Contains(o.Titulo)))
            .ToList();

        var resultados = new List<List<TareaDto>>();
        
        foreach (var inicio in iniciales)
        {
            ConstruirRutas(criticas, inicio, new List<TareaDto>(), resultados);
        }

        return resultados;
    }



    private TareaDto MapearTareaDto(Tarea tarea)
    {
        return new TareaDto()
        {
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            DependenciasNombres = tarea.Dependencias.Select(t => t.Titulo).ToList(),
            Duracion = tarea.DuracionEnDias,
            FechaEjecucion = tarea.FechaEjecucion,
            FechaFinTemprano = tarea.FechaFinTemprano,
            FechaInicioTemprano = tarea.FechaInicioTemprano,
            Holgura = tarea.Holgura,
            Realizada = tarea.Realizada,
            Recursos = MapearRecursosDto(tarea.Recursos)
        };
    }

    List<RecursoDto> MapearRecursosDto(List<Recurso> tareaRecursos)
    {
        return tareaRecursos.Select(r => new RecursoDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion,
            Tipo = r.Tipo
        }).ToList();
    }
    
    private void ConstruirRutas(
        List<TareaDto> criticas,
        TareaDto actual,
        List<TareaDto> rutaParcial,
        List<List<TareaDto>> resultados)
    {
        var nuevaRuta = new List<TareaDto>(rutaParcial) { actual };

        var siguientes = criticas
            .Where(t => t.DependenciasNombres.Contains(actual.Titulo))
            .ToList();

        if (!siguientes.Any())
        {
            resultados.Add(nuevaRuta);
        }
        else
        {
            foreach (var sig in siguientes)
            {
                ConstruirRutas(criticas, sig, nuevaRuta, resultados);
            }
        }
    }
    
    
    public void MarcarTareaDeProyectoComoRealizada(TareaDto tarea, string nombreProyecto, DateTime fechaEjecucion)
    {
        Proyecto p = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                     ?? throw new KeyNotFoundException("Proyecto no encontrado");
        Tarea t = p.BuscarTareaPorNombre(tarea.Titulo);
        t.MarcarTareaRealizada(fechaEjecucion);
        _repositorioProyectos.Actualizar(p);
    } 

    public List<TareaDto> TraerTareasDeProyecto(string nombreProyecto)
    {
        List<Tarea> tareas = _repositorioTareas.TraerTodos()
                .Where(t => t.ProyectoNombre == nombreProyecto)
                .ToList();
      
        return AtributosDeTareaDtos(tareas);
    }

    private List<TareaDto> AtributosDeTareaDtos(List<Tarea> tareas)
    {
        return tareas.Select(t => new TareaDto { Titulo = t.Titulo, Descripcion = t.Descripcion,  Duracion = t.DuracionEnDias, Realizada = t.Realizada,
                FechaFinTemprano = t.FechaFinTemprano, FechaInicioTemprano = t.FechaInicioTemprano, Holgura = t.Holgura, 
                DependenciasNombres = t.Dependencias.Select(d => d.Titulo).ToList(), Recursos = t.Recursos.Select(r => new RecursoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Tipo = r.Tipo,
                    Descripcion = r.Descripcion,
                    Funcionalidad = r.Funcionalidad
                }).ToList()
            })
            .ToList();
    }

    public void ActualizarTarea(string nombreProyecto, TareaDto tareaEditada)
    {
        Proyecto p = _repositorioProyectos.EncontrarElemento(p => p.Nombre == nombreProyecto) 
                     ?? throw new KeyNotFoundException("Proyecto no encontrado");
        Tarea t = p.BuscarTareaPorNombre(tareaEditada.Titulo);
        t.Editar(tareaEditada.Descripcion, tareaEditada.Duracion);
        
        var nuevasDependencias = tareaEditada.DependenciasNombres
            .Select(nombre => p.BuscarTareaPorNombre(nombre))
            .ToList();

      
        t.QuitarTodasLasDependencias(); 

        foreach (var dependencia in nuevasDependencias)
        {
            t.AgregarDependencia(dependencia);
        }
        
        _repositorioProyectos.Actualizar(p); 
    }
}