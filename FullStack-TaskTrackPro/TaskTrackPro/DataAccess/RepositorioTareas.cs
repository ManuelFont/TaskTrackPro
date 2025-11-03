using System.Linq.Expressions;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using InterfacesDataAccess;

namespace DataAccess;

public class RepositorioTareas : IRepositorioTarea<Tarea>
{
    private readonly SqlContext _context;
    
    public RepositorioTareas(SqlContext context)
    {
        _context = context;
    }
    
    public void Agregar(Tarea tarea)
    {
        _context.Tareas.Add(tarea);
        _context.SaveChanges();
    }
        
    public Tarea? EncontrarElemento(Expression<Func<Tarea, bool>> filtro)
    {
        return _context.Tareas
            .Include(t => t.Proyecto)
            .Include(t => t.Dependencias)
            .Include(t => t.Requeridores)
            .Include(t => t.Recursos)
            .FirstOrDefault(filtro);
    }

    public IList<Tarea> TraerTodos()
    {
        return _context.Tareas
            .Include(t => t.Dependencias)
            .Include(t => t.Requeridores)
            .Include(t => t.Recursos)
            .ToList();
    }
    
    public void Actualizar(Tarea actualizado)
    {
        _context.Tareas.Update(actualizado);
        _context.SaveChanges();
    }
}