using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class MedicoController : Controller
    {
        private static List<Medico> _medicos = new List<Medico>
        {
            new Medico
            {
                Cedula = "28.283.416-2",
                Nombres = "Sofia Catalina",
                Apellidos = "Balboa Inostroza",
                FechaNacimiento = new DateTime(2023,11,10),
                Telefono = "999526945",
                Email = "sofiacatalina@gmail.com",
                Direccion = "Av Roma 26, Laja"
            },
            new Medico
            {
                Cedula = "27.342.255-2",
                Nombres = "Cristobal Eduardo",
                Apellidos = "Balboa Inostroza",
                FechaNacimiento = new DateTime(2020, 8, 28),
                Telefono = "0912345678",
                Email = "totobal08@email.com",
                Direccion = "Calle Secundaria 456"
            }


        };
        public IActionResult Index()
        {
            ViewBag.TotalMedicos = _medicos.Count;
            return View(_medicos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Medico medico)
        {
            if (ModelState.IsValid)
            {
                // Verificar si la cédula ya existe
                if (_medicos.Any(p => p.Cedula == medico.Cedula))
                {
                    ModelState.AddModelError("Cedula", "Ya existe un paciente con esta cédula");
                    return View(medico);
                }

                // Agregar paciente a la lista
                _medicos.Add(medico);

                TempData["Mensaje"] = $"Medico {medico.NombreCompleto} registrado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, regresar al formulario
            return View(medico);
        }

        // GET: Pacientes/Detalle/1234567890 - Muestra detalles del paciente
        public IActionResult Detalle(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
            {
                return NotFound();
            }

            var medico = _medicos.FirstOrDefault(p => p.Cedula == cedula);
            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        // GET: Pacientes/Buscar - Formulario de búsqueda
        public IActionResult Buscar()
        {
            return View(_medicos);
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

            var resultados = _medicos.Where(p =>
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
