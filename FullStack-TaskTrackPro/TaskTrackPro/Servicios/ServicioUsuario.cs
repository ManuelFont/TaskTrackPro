using Backend.Dominio;
using Dtos;
using InterfacesDataAccess;

namespace Servicios;

public class ServicioUsuario
{
    private IRepositorio<Usuario> _repositorioUsuarios;
    private readonly ServicioNotificaciones _servicioNotificaciones;


    public ServicioUsuario(
            IRepositorio<Usuario> repositorioUsuarios, ServicioNotificaciones servicioNotificaciones) 
    {
        _repositorioUsuarios = repositorioUsuarios;
        _servicioNotificaciones = servicioNotificaciones;
    }

    public Usuario? UsuarioLogueado { get; set; }

    public UsuarioDto
        UsuarioGestionado { get; set; }

    public void SetUsuario(LoginDto usuario)
    {
        Usuario? userActivo = _repositorioUsuarios.EncontrarElemento(u => u.Email == usuario.Email);
        UsuarioLogueado = userActivo;
    }

    public bool UsuarioActivoEsAdminSistema()
    {
        return UsuarioLogueado.TieneRol(RolUsuario.AdminSistema);
    }

    public bool UsuarioActivoEsAdminProyecto()
    {
        return UsuarioLogueado.TieneRol(RolUsuario.AdminProyecto);
    }

    public List<UsuarioDto> TraerTodosLosUsuarios()
    {
        return _repositorioUsuarios
            .TraerTodos()
            .Select(u => new UsuarioDto()
            {
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email
            })
            .ToList();
    }

    public List<ProyectoDto> ListarProyectosUsuarioLogueado()
    {
        if (UsuarioLogueado.ListaProyectos == null)
        {
            return new List<ProyectoDto>();
        }

        return UsuarioLogueado.ListaProyectos
            .Select(proyecto => MapearProyectoDto(proyecto))
            .ToList();
    }

    private ProyectoDto MapearProyectoDto(Proyecto proyecto)
    {
        return new ProyectoDto
        {
            NombreProyecto = proyecto.Nombre,
            DescripcionProyecto = proyecto.Descripcion,
            FechaInicio = proyecto.FechaInicioEstimada,
            MiembrosProyecto = MapearUsuariosDto(proyecto.ListaUsuarios)
        };
    }

    private List<UsuarioDto> MapearUsuariosDto(List<Usuario> usuarios)
    {
        if (usuarios == null) return new List<UsuarioDto>();

        return usuarios
            .Select(usuario => new UsuarioDto
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email
            })
            .ToList();
    }


    public void CrearUsuario(CrearUsuarioDto u) 
    {
        RolUsuario r = new RolUsuario();
        r = AsignarRolANuevoUsuario(u);

        Usuario nuevoUsuario = new Usuario(u.Nombre, u.Apellido, u.Email, u.Password, u.FechaNacimiento, r);
        _repositorioUsuarios.Agregar(nuevoUsuario);
    }

    private static RolUsuario AsignarRolANuevoUsuario(CrearUsuarioDto u)
    {
        RolUsuario r;
        if (u.RolId == 1)
        {
            r = RolUsuario.AdminSistema;
        }
        else if (u.RolId == 2)
        {
            r = RolUsuario.AdminProyecto;
        }
        else if(u.RolId == 3)
        {
            r = RolUsuario.MiembroProyecto;
        }
        else
        {
            r = RolUsuario.LiderProyecto;
        }

        return r;
    }

    public void Limpiar() 
    {
        UsuarioLogueado = null;
    }

    public void SetUsuarioAGestionar(UsuarioDto usuarioDto) 
    {
        UsuarioGestionado = usuarioDto;
    }

    public List<RolDto> ListarRolesUsuarioGestionado()
    {
        Usuario userGestionado;
        userGestionado = _repositorioUsuarios.EncontrarElemento(u => u.Email == UsuarioGestionado.Email)
                         ?? throw new KeyNotFoundException("Usuario no encontrado");
        return userGestionado.Roles
            .Select(r => new RolDto
            {
                Id = (int)r,
                Nombre = r.ToString()
            })
            .ToList();
    }

    public UsuarioDto TraerUsuarioPorEmail(string mail)
    {
        Usuario u = _repositorioUsuarios.EncontrarElemento(u => u.Email == mail)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");
        UsuarioDto user = new UsuarioDto();
        user.Nombre = u.Nombre;
        user.Apellido = u.Apellido;
        user.Email = u.Email;
        return user;
    }

    public List<RolDto> ListarRolesDisponiblesParaGestionado()
    {
        var actuales = ListarRolesUsuarioGestionado().Select(r => r.Id).ToHashSet();
        return Enum.GetValues<RolUsuario>()
            .Cast<RolUsuario>()
            .Select(r => new RolDto { Id = (int)r, Nombre = r.ToString() })
            .Where(dto => !actuales.Contains(dto.Id))
            .ToList();
    }

    public void ReiniciarContrasena(string email)
    {
        Usuario u = _repositorioUsuarios.EncontrarElemento(u => u.Email == email)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");
        u.RedefinirContraseña("Change123.");
        
        _servicioNotificaciones.Notificar("Tu contraseña fue reiniciada por el administrador.", u);
        
    }

    public void AutogenerarContrasena(string email) 
    {
        Usuario u = _repositorioUsuarios.EncontrarElemento(u => u.Email == email)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");
        u.RedefinirContraseña(GenerarContrasenaAleatoria());
        
        _servicioNotificaciones.Notificar("Tu contraseña fue autogenerada correctamente.", u);
    }

    public string GenerarContrasenaAleatoria(int length = 8) 
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%&*";

        var rand = new Random();
        var pwdChars = new List<char>(length);

        pwdChars.Add(upper[rand.Next(upper.Length)]);
        pwdChars.Add(lower[rand.Next(lower.Length)]);
        pwdChars.Add(digits[rand.Next(digits.Length)]);
        pwdChars.Add(special[rand.Next(special.Length)]);

        string all = upper + lower + digits + special;
        for (int i = 4; i < length; i++)
        {
            pwdChars.Add(all[rand.Next(all.Length)]);
        }

        for (int i = 0; i < pwdChars.Count; i++)
        {
            int j = rand.Next(pwdChars.Count);
            (pwdChars[i], pwdChars[j]) = (pwdChars[j], pwdChars[i]);
        }

        return new string(pwdChars.ToArray());
    }

    public void CambiarRolUsuario(string usuarioGestionandoEmail, int nuevoRol) 
    {
        Usuario u = _repositorioUsuarios.EncontrarElemento(u => u.Email == usuarioGestionandoEmail)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");
        var rolEnum = (RolUsuario)nuevoRol;
        u.AgregarRol(rolEnum);
        string mensaje = $"Se te ha asignado un rol. Ahora sos '{rolEnum}'.";
        _servicioNotificaciones.Notificar(mensaje, u);
    }


    public void EliminarUsuario(string email)
    {
        _repositorioUsuarios.Eliminar(u => u.Email == email);
    }

    public List<UsuarioDto> TraerTodosLosUsuariosMiembros() 
    {
        List<Usuario> miembros = _repositorioUsuarios
            .EncontrarLista(u => true) 
            .Where(u => u.Roles.Contains(RolUsuario.MiembroProyecto))
            .ToList();        List<UsuarioDto> miembrosDto = new List<UsuarioDto>();
        foreach (Usuario miembro in miembros)
        {
            UsuarioDto miembroDto = new UsuarioDto();
            miembroDto.Nombre = miembro.Nombre;
            miembroDto.Apellido = miembro.Apellido;
            miembroDto.Email = miembro.Email;
            miembrosDto.Add(miembroDto);
        }
        return miembrosDto;
    }

    public void AgregarProyectoUsuarioLogueado(Proyecto nuevoProyecto)
    {
        UsuarioLogueado.AgregarProyecto(nuevoProyecto);
    }

    public bool UsuarioTieneRolMiembro(string uDtoEmail)
    {
        return (_repositorioUsuarios.EncontrarElemento(u => u.Email == uDtoEmail) 
                ?? throw new KeyNotFoundException("Usuario no encontrado")).Roles.Contains(RolUsuario.MiembroProyecto);
    }


    public void CambiarPasswordUsuario(CambiarPasswordDto cambiarPassDto)
    {
        string hashActual = Usuario.HashSHA256(cambiarPassDto.ContraseniaActual);
        string hashNueva = Usuario.HashSHA256(cambiarPassDto.NuevaContrasenia);

        if (hashActual != UsuarioLogueado.Password)
        {
            throw new ArgumentException("La contraseña ingresada no coincide con la contraseña actual");
        }

        if (hashNueva == UsuarioLogueado.Password)
        {
            throw new ArgumentException("La nueva contraseña es igual a la anterior, debes elegir una diferente");
        }

        UsuarioLogueado.RedefinirContraseña(cambiarPassDto.NuevaContrasenia);
        _repositorioUsuarios.Actualizar(UsuarioLogueado);
    }


    public List<ProyectoDto> ObtenerProyectosDeUsuarioLogueado()
    {
        string emailUser = UsuarioLogueado.Email;
        Usuario user = _repositorioUsuarios.EncontrarElemento(u => u.Email == emailUser)
                       ?? throw new KeyNotFoundException("Usuario no encontrado");
        List<Proyecto> proyectoUser = user.ListaProyectos;
        List<ProyectoDto> proyectoDto = new List<ProyectoDto>();
        foreach (var p in proyectoUser)
        {
            ProyectoDto pDto = new ProyectoDto();
            pDto.NombreProyecto = p.Nombre;
            proyectoDto.Add(pDto);
        }

        return proyectoDto;
    }


    public List<UsuarioDto> TraerTodosLosUsuariosLideres()
    {
        List<Usuario> lideres=  _repositorioUsuarios.EncontrarLista(u => u.RolesSerializados.Contains("LiderProyecto")).ToList();
        return MapearUsuariosDto(lideres);
    }
    public bool UsuarioActivoEsLiderProyecto()
    {
        return UsuarioLogueado.TieneRol(RolUsuario.LiderProyecto);
    }
}