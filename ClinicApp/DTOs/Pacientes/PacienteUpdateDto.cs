using System.ComponentModel.DataAnnotations;
using ClinicApp.Validators;

namespace ClinicApp.DTOs.Pacientes
{
    /// <summary>
    /// DTO para actualizar un paciente existente
    /// </summary>
    public class PacienteUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [CedulaEcuatoriana]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los nombres solo pueden contener letras")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Los apellidos solo pueden contener letras")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [FechaValida]
        public DateOnly FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 20 caracteres")]
        [RegularExpression(@"^[0-9\-\+\(\)\s]+$", ErrorMessage = "El teléfono solo puede contener números, guiones, paréntesis y espacios")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres")]
        public string? Direccion { get; set; }

        [StringLength(5, ErrorMessage = "El tipo de sangre no puede exceder los 5 caracteres")]
        [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "El tipo de sangre debe ser válido (A+, A-, B+, B-, AB+, AB-, O+, O-)")]
        [Display(Name = "Tipo de Sangre")]
        public string? TipoSangre { get; set; }

        [StringLength(100, ErrorMessage = "El contacto de emergencia no puede exceder los 100 caracteres")]
        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono de emergencia no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono de emergencia no puede exceder los 20 caracteres")]
        [Display(Name = "Teléfono de Emergencia")]
        public string? TelefonoEmergencia { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
