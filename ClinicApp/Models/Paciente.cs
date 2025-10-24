using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Paciente
{
    public int Id { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? TipoSangre { get; set; }

    public string? ContactoEmergencia { get; set; }

    public string? TelefonoEmergencia { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CitasMedica> CitasMedicas { get; set; } = new List<CitasMedica>();

    public virtual ICollection<HistorialesMedico> HistorialesMedicos { get; set; } = new List<HistorialesMedico>();
    public int Edad
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;
            
            return edad;
        }
    }
}
