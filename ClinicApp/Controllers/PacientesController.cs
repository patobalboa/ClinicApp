using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicApp.Controllers
{
    public class PacientesController : Controller
    {

        private readonly ClinicAppDbContext _context;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(ClinicAppDbContext context, ILogger<PacientesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // GET: /Pacientes
        // READ - Mostrar lista de todos los pacientes
        public async Task<IActionResult> Index()
        {
            try
            {
                var pacientes = await _context.Pacientes
                    .OrderBy(p => p.Apellidos)
                    .ThenBy(p => p.Nombres)
                    .ToListAsync();

                _logger.LogInformation("Se cargaron {Count} pacientes", pacientes.Count);
                return View(pacientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de pacientes");
                TempData["Error"] = "Error al cargar la lista de pacientes";
                return View(new List<Paciente>());
            }
        }

        // GET: /Pacientes/Details/5
        // READ - Mostrar detalles de un paciente específico
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    _logger.LogWarning("Paciente con ID {Id} no encontrado", id);
                    return NotFound();
                }

                return View(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar paciente con ID {Id}", id);
                TempData["Error"] = "Error al cargar los detalles del paciente";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Pacientes/Create
        // CREATE - Mostrar formulario para crear nuevo paciente
        public IActionResult Create()
        {
            return View(new Paciente());
        }

        // POST: /Pacientes/Create
        // CREATE - Procesar la creación de un nuevo paciente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paciente paciente)
        {
            if (!ModelState.IsValid)
            {
                return View(paciente);
            }

            try
            {
                // Verificar si ya existe un paciente con la misma cédula
                var existePaciente = await _context.Pacientes
                    .AnyAsync(p => p.Cedula == paciente.Cedula);

                if (existePaciente)
                {
                    ModelState.AddModelError("Cedula", "Ya existe un paciente registrado con esta cédula");
                    return View(paciente);
                }

                // Verificar si ya existe un paciente con el mismo email
                if (!string.IsNullOrEmpty(paciente.Email))
                {
                    var existeEmail = await _context.Pacientes
                        .AnyAsync(p => p.Email == paciente.Email);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe un paciente registrado con este email");
                        return View(paciente);
                    }
                }

                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Paciente creado exitosamente: {Nombres} {Apellidos} (ID: {Id})",
                    paciente.Nombres, paciente.Apellidos, paciente.Id);

                TempData["Success"] = "Paciente creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear paciente: {Nombres} {Apellidos}",
                    paciente.Nombres, paciente.Apellidos);

                ModelState.AddModelError("", "Ocurrió un error al guardar el paciente. Por favor, intente nuevamente.");
                return View(paciente);
            }
        }

        // GET: /Pacientes/Edit/5
        // UPDATE - Mostrar formulario para editar paciente existente
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    _logger.LogWarning("Paciente con ID {Id} no encontrado para edición", id);
                    return NotFound();
                }

                return View(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar paciente para edición con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del paciente";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Pacientes/Edit/5
        // UPDATE - Procesar la actualización del paciente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Paciente paciente)
        {
            if (id != paciente.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                return View(paciente);
            }

            try
            {
                // Verificar si existe otro paciente con la misma cédula (excluyendo el actual)
                var existePaciente = await _context.Pacientes
                    .AnyAsync(p => p.Cedula == paciente.Cedula && p.Id != paciente.Id);

                if (existePaciente)
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro paciente registrado con esta cédula");
                    return View(paciente);
                }

                // Verificar email duplicado (excluyendo el actual)
                if (!string.IsNullOrEmpty(paciente.Email))
                {
                    var existeEmail = await _context.Pacientes
                        .AnyAsync(p => p.Email == paciente.Email && p.Id != paciente.Id);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe otro paciente registrado con este email");
                        return View(paciente);
                    }
                }

                _context.Update(paciente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Paciente actualizado exitosamente: {Nombres} {Apellidos} (ID: {Id})",
                    paciente.Nombres, paciente.Apellidos, paciente.Id);

                TempData["Success"] = "Paciente actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PacienteExists(paciente.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar paciente con ID {Id}", id);
                ModelState.AddModelError("", "El paciente fue modificado por otro usuario. Por favor, recargue la página.");
                return View(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar el paciente. Por favor, intente nuevamente.");
                return View(paciente);
            }
        }

        // GET: /Pacientes/Delete/5
        // DELETE - Mostrar confirmación de eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    _logger.LogWarning("Paciente con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                return View(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar paciente para eliminación con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del paciente";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Pacientes/Delete/5
        // DELETE - Ejecutar la eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);

                if (paciente == null)
                {
                    _logger.LogWarning("Paciente con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                string nombreCompleto = $"{paciente.Nombres} {paciente.Apellidos}";

                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Paciente eliminado exitosamente: {NombreCompleto} (ID: {Id})",
                    nombreCompleto, id);

                TempData["Success"] = $"Paciente {nombreCompleto} eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paciente con ID {Id}", id);
                TempData["Error"] = "Error al eliminar el paciente";
                return RedirectToAction(nameof(Index));
            }
        }
        // Método auxiliar para verificar si un paciente existe
        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(p => p.Id == id);
        }

    }
}
