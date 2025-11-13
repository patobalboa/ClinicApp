using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    public class MedicosController : Controller
    {
        private readonly ClinicAppDbContext _context;
        private readonly ILogger<MedicosController> _logger;

        public MedicosController(ClinicAppDbContext context, ILogger<MedicosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Medicos
        public async Task<IActionResult> Index()
        {
            try
            {
                var medicos = await _context.Medicos
                    .Include(m => m.Especialidad)
                    .OrderBy(m => m.Apellidos)
                    .ThenBy(m => m.Nombres)
                    .ToListAsync();

                _logger.LogInformation("Se cargaron {Count} médicos", medicos.Count);
                return View(medicos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de médicos");
                TempData["Error"] = "Error al cargar la lista de médicos";
                return View(new List<Medico>());
            }
        }

        // GET: /Medicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var medico = await _context.Medicos
                    .Include(m => m.Especialidad)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (medico == null)
                {
                    _logger.LogWarning("Médico con ID {Id} no encontrado", id);
                    return NotFound();
                }

                return View(medico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar médico con ID {Id}", id);
                TempData["Error"] = "Error al cargar los detalles del médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Medicos/Create
        public async Task<IActionResult> Create()
        {
            await CargarEspecialidades();
            return View(new Medico { 
                HorarioInicio = new TimeOnly(8, 0),
                HorarioFin = new TimeOnly(17, 0),
                Activo = true
            });
        }

        // POST: /Medicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medico medico)
        {
            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medico);
            }

            try
            {
                // Verificar cédula duplicada
                var existeCedula = await _context.Medicos
                    .AnyAsync(m => m.Cedula == medico.Cedula);

                if (existeCedula)
                {
                    ModelState.AddModelError("Cedula", "Ya existe un médico registrado con esta cédula");
                    await CargarEspecialidades();
                    return View(medico);
                }

                // Verificar licencia duplicada
                var existeLicencia = await _context.Medicos
                    .AnyAsync(m => m.NumeroLicencia == medico.NumeroLicencia);

                if (existeLicencia)
                {
                    ModelState.AddModelError("NumeroLicencia", "Ya existe un médico con este número de licencia");
                    await CargarEspecialidades();
                    return View(medico);
                }

                // Verificar email duplicado
                if (!string.IsNullOrEmpty(medico.Email))
                {
                    var existeEmail = await _context.Medicos
                        .AnyAsync(m => m.Email == medico.Email);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe un médico registrado con este email");
                        await CargarEspecialidades();
                        return View(medico);
                    }
                }

                _context.Medicos.Add(medico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Médico creado exitosamente: {Nombres} {Apellidos} (ID: {Id})",
                    medico.Nombres, medico.Apellidos, medico.Id);

                TempData["Success"] = "Médico creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear médico: {Nombres} {Apellidos}",
                    medico.Nombres, medico.Apellidos);

                ModelState.AddModelError("", "Ocurrió un error al guardar el médico. Por favor, intente nuevamente.");
                await CargarEspecialidades();
                return View(medico);
            }
        }

        // GET: /Medicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var medico = await _context.Medicos.FindAsync(id);

                if (medico == null)
                {
                    _logger.LogWarning("Médico con ID {Id} no encontrado para edición", id);
                    return NotFound();
                }

                await CargarEspecialidades();
                return View(medico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar médico para edición con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Medicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medico medico)
        {
            if (id != medico.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(medico);
            }

            try
            {
                // Verificar cédula duplicada
                var existeCedula = await _context.Medicos
                    .AnyAsync(m => m.Cedula == medico.Cedula && m.Id != medico.Id);

                if (existeCedula)
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro médico con esta cédula");
                    await CargarEspecialidades();
                    return View(medico);
                }

                // Verificar licencia duplicada
                var existeLicencia = await _context.Medicos
                    .AnyAsync(m => m.NumeroLicencia == medico.NumeroLicencia && m.Id != medico.Id);

                if (existeLicencia)
                {
                    ModelState.AddModelError("NumeroLicencia", "Ya existe otro médico con este número de licencia");
                    await CargarEspecialidades();
                    return View(medico);
                }

                // Verificar email duplicado
                if (!string.IsNullOrEmpty(medico.Email))
                {
                    var existeEmail = await _context.Medicos
                        .AnyAsync(m => m.Email == medico.Email && m.Id != medico.Id);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe otro médico con este email");
                        await CargarEspecialidades();
                        return View(medico);
                    }
                }

                _context.Update(medico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Médico actualizado exitosamente: {Nombres} {Apellidos} (ID: {Id})",
                    medico.Nombres, medico.Apellidos, medico.Id);

                TempData["Success"] = "Médico actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MedicoExists(medico.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar médico con ID {Id}", id);
                ModelState.AddModelError("", "El médico fue modificado por otro usuario. Por favor, recargue la página.");
                await CargarEspecialidades();
                return View(medico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar médico con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar el médico. Por favor, intente nuevamente.");
                await CargarEspecialidades();
                return View(medico);
            }
        }

        // GET: /Medicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var medico = await _context.Medicos
                    .Include(m => m.Especialidad)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (medico == null)
                {
                    _logger.LogWarning("Médico con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                return View(medico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar médico para eliminación con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Medicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var medico = await _context.Medicos.FindAsync(id);

                if (medico == null)
                {
                    _logger.LogWarning("Médico con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                string nombreCompleto = $"{medico.Nombres} {medico.Apellidos}";

                _context.Medicos.Remove(medico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Médico eliminado exitosamente: {NombreCompleto} (ID: {Id})",
                    nombreCompleto, id);

                TempData["Success"] = $"Médico {nombreCompleto} eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al eliminar médico con ID {Id} - Posibles registros relacionados", id);
                TempData["Error"] = "No se puede eliminar el médico porque tiene citas o historiales asociados";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar médico con ID {Id}", id);
                TempData["Error"] = "Error al eliminar el médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para cargar especialidades en el ViewBag
        private async Task CargarEspecialidades()
        {
            ViewBag.Especialidades = new SelectList(
                await _context.Especialidades
                    .Where(e => e.Activa)
                    .OrderBy(e => e.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre"
            );
        }

        // Método auxiliar para verificar si un médico existe
        private bool MedicoExists(int id)
        {
            return _context.Medicos.Any(m => m.Id == id);
        }
    }
}
