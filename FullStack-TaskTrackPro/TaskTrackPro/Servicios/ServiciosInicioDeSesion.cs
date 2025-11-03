using Backend.Dominio;
using Dtos;
using InterfacesDataAccess;

namespace Servicios;

public class ServicioInicioDeSesion
{
    private IRepositorio<Usuario> _repositorio;

    public ServicioInicioDeSesion(IRepositorio<Usuario> repositorio)
    {
        _repositorio = repositorio;
    }

    public bool Login(LoginDto loginDto)
    {
        var usuario = _repositorio.EncontrarElemento(u => u.Email == loginDto.Email);
        if (usuario == null)
            return false;

        string passwordHasheada = Usuario.HashSHA256(loginDto.Password);
        
        return usuario.Password == passwordHasheada;
    }
}