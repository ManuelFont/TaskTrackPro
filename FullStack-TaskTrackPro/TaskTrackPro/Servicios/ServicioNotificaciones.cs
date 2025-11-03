using Backend.Dominio;
using Dtos;
using InterfacesDataAccess;

namespace Servicios;

public class ServicioNotificaciones
{
    private readonly IRepositorio<Notificacion> _repo;

    public ServicioNotificaciones(IRepositorio<Notificacion> repo)
    {
        _repo = repo;
    }

    public void Notificar(string mensaje, Usuario usuario)
    {
        var noti = new Notificacion(mensaje, usuario.Email);
        noti.Vista = false;
        _repo.Agregar(noti);
    }

    public List<NotificacionDto> TraerNoVistas(string usuarioEmail)
    {
        return _repo.EncontrarLista(n => n.UsuarioEmail == usuarioEmail && !n.Vista)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha
            })
            .ToList();
    }
    
    public void MarcarComoVista(int notificacionId)
    {
        Notificacion? notiActualizada = _repo.EncontrarElemento(n => n.Id == notificacionId);
        if (notiActualizada == null)
        {
            throw new KeyNotFoundException($"No se encontró la notificación con ID: {notificacionId}.");
        }
        notiActualizada.Vista = true;
        _repo.Actualizar(notiActualizada);
    }
    
    
}






