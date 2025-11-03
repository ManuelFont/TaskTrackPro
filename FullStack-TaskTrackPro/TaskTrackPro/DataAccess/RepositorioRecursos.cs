using System.Linq.Expressions;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using InterfacesDataAccess;

namespace DataAccess;

public class RepositorioRecursos : IRepositorio<Recurso>
{
    private readonly SqlContext _context;

    public RepositorioRecursos(SqlContext context)
    {
        _context = context;
    }
    
    public void Agregar(Recurso recurso)
    {
        _context.Recursos.Add(recurso);
        _context.SaveChanges();
    }
    
    public IList<Recurso> TraerTodos()
    {
        return _context.Recursos.ToList();
    }

    public void Eliminar(Func<Recurso, bool> filtro)
    {
        Recurso recurso = _context.Recursos.Where(filtro).FirstOrDefault() 
                            ?? throw new KeyNotFoundException("Recurso no encontrado");
        _context.Recursos.Remove(recurso);
        _context.SaveChanges();
    }
    
    public void Actualizar(Recurso actualizado)
    {
        Recurso recursoDb = EncontrarElemento(r => actualizado.Id == r.Id) 
                             ?? throw new KeyNotFoundException("Recurso no encontrado");
        recursoDb.Nombre = actualizado.Nombre;
        recursoDb.Tipo = actualizado.Tipo;
        recursoDb.Descripcion = actualizado.Descripcion;
        recursoDb.ProyectoNombre = actualizado.ProyectoNombre;
        recursoDb.Proyecto = actualizado.Proyecto;
        recursoDb.Funcionalidad = actualizado.Funcionalidad;
        recursoDb.UtilizadoHasta = actualizado.UtilizadoHasta;
        
        List<Tarea> tareasDb = new List<Tarea>();
        foreach (Tarea tarea in actualizado.Tareas)
        {
            Tarea tareaDb = _context.Tareas.FirstOrDefault(t => t.Titulo == tarea.Titulo && t.ProyectoNombre == tarea.ProyectoNombre)
                            ?? throw new KeyNotFoundException("Tarea no encontrada");
            tareasDb.Add(tareaDb);
        }

        recursoDb.Tareas = tareasDb;
        _context.SaveChanges();

    }
    public Recurso? EncontrarElemento(Expression<Func<Recurso, bool>> filtro)
    {
        return _context.Recursos
            .Include(r => r.Proyecto)
            .Include(r => r.Tareas)
            .Include(r => r.FechasDeUso).FirstOrDefault(filtro);
    }

    public IList<Recurso> EncontrarLista(Expression<Func<Recurso, bool>> filtro)
    {
        return _context.Recursos
            .Include(r => r.Proyecto)
            .Include(r => r.Tareas)
            .Where(filtro)
            .ToList();
    }
}