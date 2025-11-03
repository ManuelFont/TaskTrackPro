using System.ComponentModel.DataAnnotations;

namespace Backend.Dominio;

public class Notificacion
{
    public int Id { get; set; }

    [Required]
    public string Mensaje { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public bool Vista { get; set; }

    [Required]
    public string UsuarioEmail { get; set; }

    public Usuario Usuario { get; set; }  

    public Notificacion() { }

    public Notificacion(string mensaje, string usuarioEmail)
    {
        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ArgumentException("El mensaje no puede estar vacío.");

        Mensaje = mensaje;
        Fecha = DateTime.Now;
        Vista = false;
        UsuarioEmail = usuarioEmail;
    }

    public void MarcarComoVista()
    {
        Vista = true;
    }
}