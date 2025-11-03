using System.ComponentModel.DataAnnotations;

namespace Dtos;


public class CrearUsuarioDto
{

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Password { get; set; }
        [Required(ErrorMessage = "Debes seleccionar un rol")]
        public int? RolId { get; set; }
    
}