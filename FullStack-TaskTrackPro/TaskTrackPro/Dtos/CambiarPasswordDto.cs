using System.ComponentModel.DataAnnotations;

namespace Dtos;

public class CambiarPasswordDto
{
        [Required]
        public string ContraseniaActual { get; set; }

        [Required]
        public string NuevaContrasenia { get; set; }

        [Required]
        public string ConfirmarContrasenia { get; set; }
    
}