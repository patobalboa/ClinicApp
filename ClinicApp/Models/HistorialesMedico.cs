using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class HistorialesMedico
{
    public int Id { get; set; }

    public int PacienteId { get; set; }

    public int MedicoId { get; set; }

    public DateTime FechaConsulta { get; set; }

    public string? MotivoConsulta { get; set; }

    public string? Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public string? Medicamentos { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? ProximaCita { get; set; }

    public virtual Medico Medico { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}
