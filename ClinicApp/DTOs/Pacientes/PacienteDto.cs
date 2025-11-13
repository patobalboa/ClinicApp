namespace ClinicApp.DTOs.Pacientes
{
    /// <summary>
    /// DTO para mostrar información de un paciente
    /// </summary>
    public class PacienteDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombres} {Apellidos}";
        public DateOnly FechaNacimiento { get; set; }
        public int Edad
        {
            get
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? TipoSangre { get; set; }
        public string? ContactoEmergencia { get; set; }
        public string? TelefonoEmergencia { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
