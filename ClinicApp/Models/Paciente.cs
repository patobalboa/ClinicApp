using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Models
{
    public class Paciente
    {
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Display(Name = "Número de Cédula")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 caracteres")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [Display(Name = "Nombres")]
        [StringLength(50, ErrorMessage = "Los nombres no pueden exceder 50 caracteres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        [StringLength(50, ErrorMessage = "Los apellidos no pueden exceder 50 caracteres")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; }

        [Display(Name = "Tipo de Sangre")]
        public string? TipoSangre { get; set; }

        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }

        [Display(Name = "Teléfono de Emergencia")]
        public string? TelefonoEmergencia { get; set; }

        // Propiedades calculadas
        public string NombreCompleto => $"{Nombres} {Apellidos}";

        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }
    }
}
