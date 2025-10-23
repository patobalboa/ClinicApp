using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class CitasMedica
{
    public int Id { get; set; }

    public int PacienteId { get; set; }

    public int MedicoId { get; set; }

    public DateTime FechaHora { get; set; }

    public string? Motivo { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Medico Medico { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}
