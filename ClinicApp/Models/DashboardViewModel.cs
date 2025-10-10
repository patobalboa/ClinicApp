using System.Collections.Generic;

namespace ClinicApp.Models
{
    public class DashboardViewModel
    {
        public int TotalPacientes { get; set; }
        public int CitasHoy { get; set; }
        public int MedicosDisponibles { get; set; }
        public int SalasOcupadas { get; set; }
        public decimal IngresosMensuales { get; set; }
        public List<CitaDelDia> CitasRecientes { get; set; } = new();
        public List<PacienteReciente> PacientesRecientes { get; set; } = new();
        public List<MedicoStats> MedicosActivos { get; set; } = new();
        public List<EspecialidadStats> EspecialidadesPopulares { get; set; } = new();
    }

    public class CitaDelDia
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string PacienteNombre { get; set; }
        public string MedicoNombre { get; set; }
        public string Especialidad { get; set; }
        public string Estado { get; set; } // Programada, En curso, Completada, Cancelada
        public string Motivo { get; set; }
    }

    public class PacienteReciente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UltimaCita { get; set; }
        public string Estado { get; set; }
    }

    public class MedicoStats
    {
        public string NombreCompleto { get; set; }
        public string Especialidad { get; set; }
        public int CitasHoy { get; set; }
        public string Estado { get; set; } // Disponible, Ocupado, No disponible
        public TimeSpan ProximaCita { get; set; }
    }

    public class EspecialidadStats
    {
        public string Especialidad { get; set; }
        public int CitasEsteMes { get; set; }
        public decimal Ingresos { get; set; }
        public int PacientesAtendidos { get; set; }
    }

   }
