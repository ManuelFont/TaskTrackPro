using System.ComponentModel.DataAnnotations;

namespace Backend.Dominio;

public class Proyecto
{
    [Key]
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaInicioEstimada { get; set; }
    public List<Tarea> ListaDeTareas { get; set; } = new();
    public List<Usuario> ListaUsuarios { get; set; } = new();
    public string LiderEmail { get; set; }
    public Usuario Lider {get; set;}
    public List<Recurso> Recursos { get; set; } = new List<Recurso>();

    public Proyecto(string nombre, string descripcion, DateTime fechaInicioEstimada)
    {
        ValidarProyecto(nombre, descripcion, fechaInicioEstimada);
        Nombre = nombre;
        Descripcion = descripcion;
        FechaInicioEstimada = fechaInicioEstimada;
        ListaDeTareas = new List<Tarea>();
        ListaUsuarios = new List<Usuario>();
    }

    private void ValidarProyecto(string nombre, string descripcion, DateTime fechaInicioEstimada)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del proyecto no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción del proyecto no puede estar vacía.");

        if (descripcion.Length > 400)
            throw new ArgumentException("La descripción no puede superar los 400 caracteres.");
    }

    public void ModificarProyecto(Usuario u, string? nuevaDescripcion, DateTime? nuevaFechaInicio, Proyecto _)
    {
        if (!EstaHabilitadoAModificarProyecto(u))
            throw new ArgumentException("Solamente un administrador de proyecto puede realizar cambios en el proyecto");

        if (nuevaDescripcion != null)
            Descripcion = nuevaDescripcion;

        if (nuevaFechaInicio != null)
            FechaInicioEstimada = nuevaFechaInicio.Value;
    }


    private bool EstaHabilitadoAModificarProyecto(Usuario u)
    {
        return ExisteUsuarioEnElProyecto(u.Email) && u.TieneRol(RolUsuario.AdminProyecto);
    }

    public void AgregarTarea(Tarea t1)
    {
        if (ExisteTarea(t1.Titulo))
            throw new ArgumentException("Ya existe una tarea con ese título");
        ListaDeTareas.Add(t1);
    }

    public void AgregarMiembroAlProyecto(Usuario u)
    {
        if (ExisteUsuarioEnElProyecto(u.Email))
            throw new ArgumentException("Ya existe el miembro en el proyecto");

        ListaUsuarios.Add(u);
    }

    public void EliminarUsuarioDeProyecto(Usuario borrar)
    {
        ListaUsuarios.Remove(borrar);
    }

    public Usuario BuscarUsuarioEnProyectoPorMail(string mail)
    {
        var usuario = ListaUsuarios.Find(u => u.Email == mail);
        return usuario ?? throw new Exception("No se encontró ningún usuario con ese mail.");
    }

    public Tarea BuscarTareaPorNombre(string titulo)
    {
        var tarea = ListaDeTareas.Find(t => t.Titulo == titulo);
        return tarea ?? throw new ArgumentException("No se encontró ninguna tarea con ese título.");
    }

    public DateTime FechaFinEstimada()
    {
        return ListaDeTareas.Count == 0 ? FechaInicioEstimada : ListaDeTareas.Max(t => t.FechaFinTemprano);
    }

    public IEnumerable<Tarea> CaminoCritico()
    {
        return CaminoMasLargo(ListaDeTareas).Reverse();
    }

    public List<Tarea> TraerTareasNoCriticas()
    {
        return ListaDeTareas.Where(t => !t.EsCritica).ToList();
    }

    public IEnumerable<Tarea> TraerTareasCriticas()
    {
        return ListaDeTareas.Where(t => t.EsCritica).ToList();
    }

    private bool ExisteTarea(string titulo)
    {
        return ListaDeTareas.Any(t => t.Titulo == titulo);
    }

    private bool ExisteUsuarioEnElProyecto(string mail)
    {
        return ListaUsuarios.Any(u => u.Email == mail);
    }

    private IEnumerable<Tarea> CaminoMasLargo(IEnumerable<Tarea> tareas)
    {
        List<Tarea> camino = new();
        if (tareas.Any())
        {
            Tarea t = tareas.Aggregate((a, b) => a.FechaFinTemprano > b.FechaFinTemprano ? a : b);
            camino.Add(t);
            camino.AddRange(CaminoMasLargo(t.Dependencias));
        }
        return camino;
    }

    public void AsignarLider(Usuario usuarioLider)
    {
        Lider = usuarioLider;
        LiderEmail= usuarioLider.Email;
        AgregarMiembroAlProyecto(usuarioLider);
        
    }

    public void AgregarRecurso(Recurso recurso)
    {
        Recursos.Add(recurso);
    }
}