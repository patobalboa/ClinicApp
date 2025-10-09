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

        // P�gina principal de la cl�nica
        public IActionResult Index()
        {
            _logger.LogInformation("Usuario visit� la p�gina principal de la cl�nica");

            // Datos de ejemplo para mostrar en la p�gina
            ViewBag.NombreClinica = "Cl�nica San Jos�";
            ViewBag.TotalPacientes = 150;
            ViewBag.MedicosDisponibles = 8;
            ViewBag.CitasHoy = 25;

            return View();
        }

        // P�gina de informaci�n de la cl�nica
        public IActionResult Privacy()
        {
            ViewBag.TituloSeccion = "Informaci�n de la Cl�nica";
            return View();
        }

        // Nueva acci�n: M�dulos del sistema
        public IActionResult Modulos()
        {
            _logger.LogInformation("Usuario accedi� a la p�gina de m�dulos");

            // Lista de m�dulos disponibles
            var modulos = new List<string>
            {
                "Gesti�n de Pacientes",
                "Directorio de M�dicos",
                "Sistema de Citas",
                "Historiales M�dicos",
                "Reportes y Estad�sticas"
            };

            ViewBag.ModulosDisponibles = modulos;
            return View();
        }

        public IActionResult Especialidades()
        {
            _logger.LogInformation("Usuario accedi� a la p�gina de Especialidades");

            // Lista de especialidades disponibles
            var especialidades = new List<string>
            {
                "Medicina General",
                "Pediatr�a",
                "Cardiolog�a",
                "Dermatolog�a",
                "Ginecolog�a"
            };

            ViewBag.EspecialidadesDisponibles = especialidades;
            return View();
        }
        public IActionResult Emergencias()
        {
            _logger.LogInformation("Usuario accedi� a la p�gina de Emergencias");
            

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
            _logger.LogInformation("Usuario accedi� a la p�gina de Horarios");


            // Muestreo de horarios por d�a

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
