using System.Linq.Expressions;
using Backend.Dominio;
using Microsoft.EntityFrameworkCore;
using InterfacesDataAccess;

namespace DataAccess;
public class RepositorioNotificaciones : IRepositorio<Notificacion>
{
    private readonly SqlContext _context;
    public RepositorioNotificaciones(SqlContext context)
    {
        _context = context;
    }
    public void Agregar(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        _context.SaveChanges();
    }

    public IList<Notificacion> TraerTodos()
    {
        return _context.Notificaciones.OrderByDescending(n => n.Fecha).ToList();
    }

    public void Actualizar(Notificacion actualizado)
    {
        Notificacion? notiDb = _context.Notificaciones.FirstOrDefault(x => x.Id == actualizado.Id);
        if (notiDb == null)
            throw new KeyNotFoundException($"Notificacion con ID: {actualizado.Id} no ecnontrada.");
            
        notiDb.Mensaje = actualizado.Mensaje;
        notiDb.Fecha = actualizado.Fecha;
        notiDb.Vista = actualizado.Vista;
        notiDb.UsuarioEmail = actualizado.UsuarioEmail;

        var usuarioDb = _context.Usuarios.FirstOrDefault(u => u.Email == actualizado.Usuario.Email);
        if (usuarioDb != null)
        {
            notiDb.Usuario = usuarioDb;
        }

        _context.SaveChanges();
    }

    public void Eliminar(Func<Notificacion, bool> filtro)
    {
        Notificacion notificiacion = _context.Notificaciones.Where(filtro).FirstOrDefault() 
                             ?? throw new KeyNotFoundException("Proyecto no encontrado");
        _context.Notificaciones.Remove(notificiacion);
        _context.SaveChanges();
    }
    
    public Notificacion? EncontrarElemento(Expression<Func<Notificacion, bool>> filtro)
    {
        return _context.Notificaciones
            .Include(n => n.Usuario)
            .FirstOrDefault(filtro);
    }

    public IList<Notificacion> EncontrarLista(Expression<Func<Notificacion, bool>> filtro)
    {
        return _context.Notificaciones
            .Include(n => n.Usuario)
            .Where(filtro)
            .OrderByDescending(n => n.Fecha)
            .ToList();
    }
}