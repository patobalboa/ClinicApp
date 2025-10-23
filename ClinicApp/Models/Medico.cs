using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Medico
{
    public int Id { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int EspecialidadId { get; set; }

    public string NumeroLicencia { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public TimeOnly HorarioInicio { get; set; }

    public TimeOnly HorarioFin { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<CitasMedica> CitasMedicas { get; set; } = new List<CitasMedica>();

    public virtual Especialidade Especialidad { get; set; } = null!;

    public virtual ICollection<HistorialesMedico> HistorialesMedicos { get; set; } = new List<HistorialesMedico>();
}
