using System.Diagnostics;
using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Página principal de la clínica
        public IActionResult Index()
        {
            _logger.LogInformation("Usuario visitó la página principal de la clínica");

            // Datos de ejemplo para mostrar en la página
            ViewBag.NombreClinica = "Clínica San José";
            ViewBag.TotalPacientes = 150;
            ViewBag.MedicosDisponibles = 8;
            ViewBag.CitasHoy = 25;

            return View();
        }

        // Página de información de la clínica
        public IActionResult Privacy()
        {
            ViewBag.TituloSeccion = "Información de la Clínica";
            return View();
        }

        // Nueva acción: Módulos del sistema
        public IActionResult Modulos()
        {
            _logger.LogInformation("Usuario accedió a la página de módulos");

            // Lista de módulos disponibles
            var modulos = new List<string>
            {
                "Gestión de Pacientes",
                "Directorio de Médicos",
                "Sistema de Citas",
                "Historiales Médicos",
                "Reportes y Estadísticas"
            };

            ViewBag.ModulosDisponibles = modulos;
            return View();
        }

        public IActionResult Especialidades()
        {
            _logger.LogInformation("Usuario accedió a la página de Especialidades");

            // Lista de especialidades disponibles
            var especialidades = new List<string>
            {
                "Medicina General",
                "Pediatría",
                "Cardiología",
                "Dermatología",
                "Ginecología"
            };

            ViewBag.EspecialidadesDisponibles = especialidades;
            return View();
        }
        public IActionResult Emergencias()
        {
            _logger.LogInformation("Usuario accedió a la página de Emergencias");
            

            // Lista de especialidades disponibles
            var emergencias = new List<string>
            {
                "Infartos",
                "Elementos Ocultos",
                "Descompensaciones",
                "Infecciones",
                "Fiebre Alta"
            };

            ViewBag.EmergenciasDisponibles = emergencias;
            return View();
        }
        public IActionResult Horarios(string dia)
        {
            _logger.LogInformation("Usuario accedió a la página de Horarios");


            // Muestreo de horarios por día

            ViewBag.DiaSeleccionado = dia ?? "lunes";
            ViewBag.HorarioAtencion = "8:00 AM - 6:00 PM";
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
