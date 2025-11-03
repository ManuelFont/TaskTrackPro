using System.Linq.Expressions;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using InterfacesDataAccess;

namespace DataAccess;

public class RepositorioProyectos : IRepositorio<Proyecto>
{
    private readonly SqlContext _context;
    
    public RepositorioProyectos(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Proyecto proyecto)
    {
        _context.Proyectos.Add(proyecto);
        _context.SaveChanges();
    }

    public IList<Proyecto> TraerTodos()
    {
        return _context.Proyectos.Include(p => p.ListaDeTareas).ToList();
    }
    
    public void Eliminar(Func<Proyecto, bool> filtro)
    {
        Proyecto proyecto = _context.Proyectos.Where(filtro).FirstOrDefault() 
                             ?? throw new KeyNotFoundException("Proyecto no encontrado");
        _context.Proyectos.Remove(proyecto);
        _context.SaveChanges();
    }

    public void Actualizar(Proyecto actualizado)
    {
        Proyecto proyectoDb = _context.Proyectos
                                  .Where(p => p.Nombre == actualizado.Nombre)
                                  .FirstOrDefault()
                              ?? throw new KeyNotFoundException("Proyecto no encontrado");

        proyectoDb.Nombre = actualizado.Nombre;
        proyectoDb.Descripcion = actualizado.Descripcion;
        proyectoDb.FechaInicioEstimada = actualizado.FechaInicioEstimada;

        List<Tarea> tareasDb = new List<Tarea>();
        foreach (Tarea tarea in actualizado.ListaDeTareas)
        {
            Tarea tareaDb = _context.Tareas
                                .FirstOrDefault(t => t.Proyecto.Nombre == actualizado.Nombre && t.Titulo == tarea.Titulo)
                            ?? throw new KeyNotFoundException("Tarea no encontrada");
            tareasDb.Add(tareaDb);
        }
        proyectoDb.ListaDeTareas = tareasDb;

        List<Usuario> usuariosDb = new List<Usuario>();
        foreach (Usuario usuario in actualizado.ListaUsuarios)
        {
            Usuario usuarioDb = _context.Usuarios
                                    .FirstOrDefault(u => u.Email == usuario.Email)
                                ?? throw new KeyNotFoundException("Usuario no encontrado");
            usuariosDb.Add(usuarioDb);
        }
        proyectoDb.ListaUsuarios = usuariosDb;

        proyectoDb.LiderEmail = actualizado.LiderEmail;
        Usuario liderDb = _context.Usuarios
                              .FirstOrDefault(u => u.Email == actualizado.LiderEmail)
                          ?? throw new KeyNotFoundException("Lider no encontrado");
        proyectoDb.Lider = liderDb;

        List<Recurso> recursosDb = new List<Recurso>();
        foreach (Recurso recurso in actualizado.Recursos)
        {
            Recurso recursoDb = _context.Recursos
                                    .FirstOrDefault(r => r.Id == recurso.Id)
                                ?? throw new KeyNotFoundException("Recurso no encontrado");
            recursosDb.Add(recursoDb);
        }
        proyectoDb.Recursos = recursosDb;

        _context.SaveChanges();
    }

    public Proyecto? EncontrarElemento(Expression<Func<Proyecto, bool>> filtro)
    {
        return _context.Proyectos
            .Include(p => p.ListaDeTareas)
            .ThenInclude(t => t.Dependencias)
            .Include(p => p.ListaUsuarios)
            .Include(p => p.Lider)
            .Include(p => p.Recursos)
            .FirstOrDefault(filtro);
    }

    public IList<Proyecto> EncontrarLista(Expression<Func<Proyecto, bool>> filtro)
    {
        return _context.Proyectos
            .Include(p => p.ListaDeTareas)
            .Include(p => p.ListaUsuarios)
            .Include(p => p.Lider)
            .Include(p => p.Recursos)
            .Where(filtro)
            .ToList();
    }
}