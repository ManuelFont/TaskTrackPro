using System.Linq.Expressions;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using InterfacesDataAccess;

namespace DataAccess;
public class RepositorioUsuarios : IRepositorio<Usuario>
{
    private readonly SqlContext _context;

    public RepositorioUsuarios(SqlContext context)
    {
        _context = context;
    }
    
    public void Agregar(Usuario nuevo)
    {
        _context.Usuarios.Add(nuevo);
        _context.SaveChanges();
    }
    
    public IList<Usuario> TraerTodos()
    {
        return _context.Usuarios.ToList();
    }
    
    public void Eliminar(Func<Usuario, bool> filtro)
    {
        Usuario recurso = _context.Usuarios.FirstOrDefault(filtro);
        _context.Usuarios.Remove(recurso);
        _context.SaveChanges();
    }

    public void Actualizar(Usuario actualizado)
    {
        Usuario usuarioDb = EncontrarElemento(u => u.Email == actualizado.Email);
        usuarioDb.Nombre = actualizado.Nombre;
        usuarioDb.Apellido = actualizado.Apellido;
        usuarioDb.Email = actualizado.Email;
        usuarioDb.Password = actualizado.Password;
        usuarioDb.FechaNacimiento = actualizado.FechaNacimiento;
        usuarioDb.Roles = actualizado.Roles;
        
        List<Proyecto> proyectosDb = new List<Proyecto>();
        foreach (Proyecto proyecto in actualizado.ListaProyectos)
        {
            Proyecto proyectoDb = _context.Proyectos.FirstOrDefault(p => p.Nombre == proyecto.Nombre);
            proyectosDb.Add(proyectoDb);
        }
        usuarioDb.ListaProyectos = proyectosDb;
        
        List<Notificacion> notificacionesDb = new List<Notificacion>();
        foreach (Notificacion notificacion in actualizado.Notificaciones)
        {
            Notificacion notificacionDb = _context.Notificaciones.FirstOrDefault(n => n.Id == notificacion.Id);
            notificacionesDb.Add(notificacionDb);
        }
        usuarioDb.Notificaciones = notificacionesDb;
        _context.SaveChanges();
    }
    public Usuario? EncontrarElemento(Expression<Func<Usuario, bool>> filtro)
    {
        return _context.Usuarios
            .Include(u => u.ListaProyectos)
            .Include(u => u.Notificaciones)
            .FirstOrDefault(filtro);
    }

    public IList<Usuario> EncontrarLista(Expression<Func<Usuario, bool>> filtro)
    {
        return _context.Usuarios
            .Where(filtro)
            .Include(u => u.ListaProyectos)
            .Include(u => u.Notificaciones)
            .ToList();
    }
    
}
    