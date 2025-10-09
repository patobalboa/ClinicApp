using Microsoft.AspNetCore.Mvc;
using ClinicApp.Models;

namespace ClinicApp.Controllers
{
    public class PacienteController : Controller
    {
        private static List<Paciente> _pacientes = new List<Paciente>
        {
            new Paciente
            {
                Cedula = "17592019-5",
                Nombres = "Pato Carlos",
                Apellidos = "Fierro Calderon",
                FechaNacimiento = new DateTime(1990,5,26),
                Telefono = "999526945",
                Email = "patocarlos@gmail.com",
                Direccion = "Avenida Siempre Viva 46",
                TipoSangre = "RH+"
            },
            new Paciente
            {
                Cedula = "0987654321",
                Nombres = "María Elena",
                Apellidos = "González López",
                FechaNacimiento = new DateTime(1985, 8, 22),
                Telefono = "0912345678",
                Email = "maria.gonzalez@email.com",
                Direccion = "Calle Secundaria 456",
                TipoSangre = "A+"
            }

          
        };
        public IActionResult Index()
        {
            ViewBag.TotalPacientes = _pacientes.Count;
            return View(_pacientes);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                // Verificar si la cédula ya existe
                if (_pacientes.Any(p => p.Cedula == paciente.Cedula))
                {
                    ModelState.AddModelError("Cedula", "Ya existe un paciente con esta cédula");
                    return View(paciente);
                }

                // Agregar paciente a la lista
                _pacientes.Add(paciente);

                TempData["Mensaje"] = $"Paciente {paciente.NombreCompleto} registrado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, regresar al formulario
            return View(paciente);
        }

        // GET: Pacientes/Detalle/1234567890 - Muestra detalles del paciente
        public IActionResult Detalle(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
            {
                return NotFound();
            }

            var paciente = _pacientes.FirstOrDefault(p => p.Cedula == cedula);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Buscar - Formulario de búsqueda
        public IActionResult Buscar()
        {
            return View(_pacientes);
        }

        // POST: Pacientes/Buscar - Procesar búsqueda
        [HttpPost]
        public IActionResult Buscar(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                ViewBag.Mensaje = "Ingrese un término de búsqueda";
                return View();
            }

            var resultados = _pacientes.Where(p =>
                p.Nombres.ToLower().Contains(termino.ToLower()) ||
                p.Apellidos.ToLower().Contains(termino.ToLower()) ||
                p.Cedula.Contains(termino)
            ).ToList();

            ViewBag.TerminoBusqueda = termino;
            ViewBag.CantidadResultados = resultados.Count;

            return View("ResultadosBusqueda", resultados);
        }
    }
}
