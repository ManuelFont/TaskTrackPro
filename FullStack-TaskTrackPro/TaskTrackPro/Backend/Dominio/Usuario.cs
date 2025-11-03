using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Dominio;

public class Usuario
{
    public string Nombre { get; set;}
    public string Apellido { get; set; }
    public string Email{ get; set; }
    public string Password { get; set; }
    public DateTime FechaNacimiento { get; set; }
    [NotMapped]
    public List<RolUsuario> Roles { get; set; } = new();
    public List<Proyecto> ListaProyectos { get; set; } = new List<Proyecto>();
    public List<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    public Usuario()
    {
        ListaProyectos = new List<Proyecto>();
    }

    public Usuario(string nombre, string apellido, string email, string passwordTexto, DateTime fechaNacimiento, RolUsuario rol)
    {
        ValidarDatos(nombre, apellido, email, passwordTexto, fechaNacimiento, rol);
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Password = HashSHA256(passwordTexto);
        FechaNacimiento = fechaNacimiento;
        Roles = new List<RolUsuario> { rol };
        ListaProyectos = new List<Proyecto>();
    }

    private void ValidarDatos(string nombre, string apellido, string email, string password, DateTime fechaNacimiento,
        RolUsuario rol)
    {
        if (!EsMayorDeEdad(fechaNacimiento))
            throw new ArgumentException("El usuario debe ser mayor de edad.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.");

        if ((string.IsNullOrWhiteSpace(email))|| !EsEmailValido(email))
            throw new ArgumentException("El email no puede estar vacío ni tener formato inválido.");
        
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido no puede estar vacío.");
        
        if (!ContrasenaValida(password))
            throw new ArgumentException("La contraseña no cumple con los requisitos.");
      
        if (!Enum.IsDefined(typeof(RolUsuario), rol))
            throw new ArgumentException("Rol de usuario inválido.");
    }
    public string RolesSerializados
    {
        get => string.Join(",", Roles.Select(r => r.ToString()));
        set => Roles = string.IsNullOrWhiteSpace(value)
            ? new List<RolUsuario>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => Enum.Parse<RolUsuario>(r))
                .ToList();
    }

    private bool EsEmailValido(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return mail.Address == email;
        }
        catch
        {
            return false;
        }
    }
    private static bool ContrasenaValida(string password)
    {
        return password.Length >= 8
               && password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(c => !char.IsLetterOrDigit(c));
    }
    
    public void AgregarProyecto(Proyecto p)
    {
        if (!ExisteProyecto(p.Nombre))
        {
            ListaProyectos.Add(p);
        }
        else
        {
            throw new ArgumentException("Ya existe una proyecto con ese título");   
        }
    }

    private bool ExisteProyecto(string nombre)
    {
        var proyectoEncontrado = TraerProyectoONullSiNoExiste(nombre);

        if (proyectoEncontrado != null)
        {
            return true;
        }
        return false;
    }

    private Proyecto? TraerProyectoONullSiNoExiste(string nombre)
    {
        Proyecto proyectoEncontrado = ListaProyectos.Find(Proyecto => Proyecto.Nombre.Equals(nombre));
        return proyectoEncontrado;
    }

    public Proyecto BuscarProyectoPorNombre(string nombre)
    {
        var proyecto = TraerProyectoONullSiNoExiste(nombre);

        return proyecto ?? throw new Exception("No se encontró ningún proyecto con ese título.");
    }
    
    private bool EsMayorDeEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        int edad = hoy.Year - fechaNacimiento.Year;
        
        if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;

        return edad >= 18;
    }

    public bool TieneRol(RolUsuario rol)
    {
        return Roles.Contains(rol);
    }
    
    public void AgregarRol(RolUsuario rol)
    {
        if (Roles.Contains(rol))
            throw new InvalidOperationException("El usuario ya tiene ese rol.");
        Roles.Add(rol);
    }
   
    public void EliminarRol(RolUsuario rol)
    {
        if (!Roles.Contains(rol))
            throw new InvalidOperationException("El usuario no tiene este rol.");
        Roles.Remove(rol);
    }
    
    public void ActualizarDatos(string nombre, string apellido, string email, string password, DateTime fechaNacimiento)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Password = password;
        FechaNacimiento = fechaNacimiento;
    }
    
    public void RedefinirContraseña(string nuevaContrasena)
    {
        if (!ContrasenaValida(nuevaContrasena))
            throw new ArgumentException("La nueva contraseña no cumple con los requisitos.");

        Password = HashSHA256(nuevaContrasena);
    }

    public static string HashSHA256(string input)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

}