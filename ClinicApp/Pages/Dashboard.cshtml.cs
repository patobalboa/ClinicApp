using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicApp.Models;


namespace ClinicApp.Pages
{
    public class DashboardModel : PageModel
    {
        public DashboardViewModel Dashboard { get; set; } = new();

        public void OnGet()
        {
            // Simular datos del dashboard
            Dashboard = new DashboardViewModel
            {
                TotalPacientes = 120,
                CitasHoy = 15,
                MedicosDisponibles = 8,
                SalasOcupadas = 5,
                IngresosMensuales = 4500.75m,
                CitasRecientes = DiaList,
                PacientesRecientes = PacienteList,
                MedicosActivos = MedicoList,
                EspecialidadesPopulares = EspecialidadList

            };
        }

        private List<CitaDelDia> DiaList { get; set; } = new()
        {
            new CitaDelDia
            {
                Id = 1,
                FechaHora = DateTime.Now.AddHours(1),
                PacienteNombre = "Juan Perez",
                MedicoNombre = "Dra. Ana Gomez",
                Especialidad = "Cardiología",
                Estado = "Programada",
                Motivo = "Chequeo anual"
            },
            new CitaDelDia
            {
                Id = 2,
                FechaHora = DateTime.Now.AddHours(2),
                PacienteNombre = "Maria Lopez",
                MedicoNombre = "Dr. Carlos Ruiz",
                Especialidad = "Dermatología",
                Estado = "En curso",
                Motivo = "Erupción cutánea"
            },
            new CitaDelDia
            {
                Id = 3,
                FechaHora = DateTime.Now.AddHours(3),
                PacienteNombre = "Luis Martinez",
                MedicoNombre = "Dra. Sofia Torres",
                Especialidad = "Pediatría",
                Estado = "Completada",
                Motivo = "Vacunación"
            }
        };

        private List<PacienteReciente> PacienteList { get; set; } = new()
        {
            new PacienteReciente
            {
                Id = 1,
                NombreCompleto = "Carlos Fernandez",
                Cedula = "12345678-9",
                FechaRegistro = DateTime.Now.AddDays(-10),
                UltimaCita = "2024-06-15",
                Estado = "Activo"
            },
            new PacienteReciente
            {
                Id = 2,
                NombreCompleto = "Ana Morales",
                Cedula = "98765432-1",
                FechaRegistro = DateTime.Now.AddDays(-20),
                UltimaCita = "2024-06-10",
                Estado = "Inactivo"
            },
            new PacienteReciente
            {
                Id = 3,
                NombreCompleto = "Jorge Ramirez",
                Cedula = "11223344-5",
                FechaRegistro = DateTime.Now.AddDays(-5),
                UltimaCita = "2024-06-18",
                Estado = "Activo"
            }
        };

        private List<MedicoStats> MedicoList { get; set; } = new()
        {
            new MedicoStats
            {
                NombreCompleto = "Dra. Ana Gomez",
                Especialidad = "Cardiología",
                CitasHoy = 5,
                Estado = "Disponible",
                ProximaCita = TimeSpan.FromHours(1)
            },
            new MedicoStats
            {
                NombreCompleto = "Dr. Carlos Ruiz",
                Especialidad = "Dermatología",
                CitasHoy = 3,
                Estado = "Ocupado",
                ProximaCita = TimeSpan.FromHours(2)
            },
            new MedicoStats
            {
                NombreCompleto = "Dra. Sofia Torres",
                Especialidad = "Pediatría",
                CitasHoy = 4,
                Estado = "No disponible",
                ProximaCita = TimeSpan.FromHours(3)
            }
        };

        private List<EspecialidadStats> EspecialidadList { get; set; } = new()
        {
            new EspecialidadStats
            {
                Especialidad = "Cardiología",
                CitasEsteMes = 20,
                Ingresos = 1500.50m,
                PacientesAtendidos = 18
            },
            new EspecialidadStats
            {
                Especialidad = "Dermatología",
                CitasEsteMes = 15,
                Ingresos = 1200.00m,
                PacientesAtendidos = 14
            },
            new EspecialidadStats
            {
                Especialidad = "Pediatría",
                CitasEsteMes = 25,
                Ingresos = 1800.75m,
                PacientesAtendidos = 22
            }
        };

    }
}
