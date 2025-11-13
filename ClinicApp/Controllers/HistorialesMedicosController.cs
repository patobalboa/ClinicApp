using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    public class HistorialesMedicosController : Controller
    {
        private readonly ClinicAppDbContext _context;
        private readonly ILogger<HistorialesMedicosController> _logger;

        public HistorialesMedicosController(ClinicAppDbContext context, ILogger<HistorialesMedicosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /HistorialesMedicos
        public async Task<IActionResult> Index(int? pacienteId, int? medicoId)
        {
            try
            {
                var query = _context.HistorialesMedicos
                    .Include(h => h.Paciente)
                    .Include(h => h.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .AsQueryable();

                // Filtrar por paciente si se proporciona
                if (pacienteId.HasValue)
                {
                    query = query.Where(h => h.PacienteId == pacienteId.Value);
                }

                // Filtrar por médico si se proporciona
                if (medicoId.HasValue)
                {
                    query = query.Where(h => h.MedicoId == medicoId.Value);
                }

                var historiales = await query
                    .OrderByDescending(h => h.FechaConsulta)
                    .ToListAsync();

                ViewBag.PacienteId = pacienteId;
                ViewBag.MedicoId = medicoId;

                await CargarListasFiltros();

                _logger.LogInformation("Se cargaron {Count} historiales médicos", historiales.Count);
                return View(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de historiales médicos");
                TempData["Error"] = "Error al cargar la lista de historiales médicos";
                return View(new List<HistorialesMedico>());
            }
        }

        // GET: /HistorialesMedicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var historial = await _context.HistorialesMedicos
                    .Include(h => h.Paciente)
                    .Include(h => h.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (historial == null)
                {
                    _logger.LogWarning("Historial médico con ID {Id} no encontrado", id);
                    return NotFound();
                }

                return View(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar historial médico con ID {Id}", id);
                TempData["Error"] = "Error al cargar los detalles del historial médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /HistorialesMedicos/Create
        public async Task<IActionResult> Create(int? pacienteId)
        {
            await CargarListasDesplegables();
            
            var historial = new HistorialesMedico
            {
                FechaConsulta = DateTime.Now
            };

            if (pacienteId.HasValue)
            {
                historial.PacienteId = pacienteId.Value;
            }

            return View(historial);
        }

        // POST: /HistorialesMedicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HistorialesMedico historial)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return View(historial);
            }

            try
            {
                // Validar que la fecha de consulta no sea futura
                if (historial.FechaConsulta > DateTime.Now)
                {
                    ModelState.AddModelError("FechaConsulta", "La fecha de consulta no puede ser posterior a la fecha actual");
                    await CargarListasDesplegables();
                    return View(historial);
                }

                // Validar que la próxima cita (si existe) sea futura
                if (historial.ProximaCita.HasValue && historial.ProximaCita.Value <= DateTime.Now)
                {
                    ModelState.AddModelError("ProximaCita", "La próxima cita debe ser una fecha futura");
                    await CargarListasDesplegables();
                    return View(historial);
                }

                _context.HistorialesMedicos.Add(historial);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Historial médico creado exitosamente (ID: {Id})", historial.Id);

                TempData["Success"] = "Historial médico creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear historial médico");
                ModelState.AddModelError("", "Ocurrió un error al guardar el historial médico. Por favor, intente nuevamente.");
                await CargarListasDesplegables();
                return View(historial);
            }
        }

        // GET: /HistorialesMedicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var historial = await _context.HistorialesMedicos.FindAsync(id);

                if (historial == null)
                {
                    _logger.LogWarning("Historial médico con ID {Id} no encontrado para edición", id);
                    return NotFound();
                }

                await CargarListasDesplegables();
                return View(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar historial médico para edición con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del historial médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /HistorialesMedicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HistorialesMedico historial)
        {
            if (id != historial.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return View(historial);
            }

            try
            {
                // Validar que la fecha de consulta no sea futura
                if (historial.FechaConsulta > DateTime.Now)
                {
                    ModelState.AddModelError("FechaConsulta", "La fecha de consulta no puede ser posterior a la fecha actual");
                    await CargarListasDesplegables();
                    return View(historial);
                }

                // Validar que la próxima cita (si existe) sea futura
                if (historial.ProximaCita.HasValue && historial.ProximaCita.Value <= DateTime.Now)
                {
                    ModelState.AddModelError("ProximaCita", "La próxima cita debe ser una fecha futura");
                    await CargarListasDesplegables();
                    return View(historial);
                }

                _context.Update(historial);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Historial médico actualizado exitosamente (ID: {Id})", historial.Id);

                TempData["Success"] = "Historial médico actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!HistorialMedicoExists(historial.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar historial médico con ID {Id}", id);
                ModelState.AddModelError("", "El historial fue modificado por otro usuario. Por favor, recargue la página.");
                await CargarListasDesplegables();
                return View(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar historial médico con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar el historial médico. Por favor, intente nuevamente.");
                await CargarListasDesplegables();
                return View(historial);
            }
        }

        // GET: /HistorialesMedicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var historial = await _context.HistorialesMedicos
                    .Include(h => h.Paciente)
                    .Include(h => h.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (historial == null)
                {
                    _logger.LogWarning("Historial médico con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                return View(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar historial médico para eliminación con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos del historial médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /HistorialesMedicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var historial = await _context.HistorialesMedicos.FindAsync(id);

                if (historial == null)
                {
                    _logger.LogWarning("Historial médico con ID {Id} no encontrado para eliminación", id);
                    return NotFound();
                }

                _context.HistorialesMedicos.Remove(historial);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Historial médico eliminado exitosamente (ID: {Id})", id);

                TempData["Success"] = "Historial médico eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar historial médico con ID {Id}", id);
                TempData["Error"] = "Error al eliminar el historial médico";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para cargar listas desplegables
        private async Task CargarListasDesplegables()
        {
            ViewBag.Pacientes = new SelectList(
                await _context.Pacientes
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Apellidos)
                    .ThenBy(p => p.Nombres)
                    .Select(p => new { 
                        p.Id, 
                        NombreCompleto = p.Apellidos + " " + p.Nombres + " (" + p.Cedula + ")"
                    })
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );

            ViewBag.Medicos = new SelectList(
                await _context.Medicos
                    .Include(m => m.Especialidad)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Apellidos)
                    .ThenBy(m => m.Nombres)
                    .Select(m => new { 
                        m.Id, 
                        NombreCompleto = "Dr(a). " + m.Apellidos + " " + m.Nombres + " - " + m.Especialidad.Nombre
                    })
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );
        }

        // Método auxiliar para cargar listas de filtros
        private async Task CargarListasFiltros()
        {
            ViewBag.PacientesFiltro = new SelectList(
                await _context.Pacientes
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Apellidos)
                    .ThenBy(p => p.Nombres)
                    .Select(p => new { 
                        p.Id, 
                        NombreCompleto = p.Apellidos + " " + p.Nombres
                    })
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );

            ViewBag.MedicosFiltro = new SelectList(
                await _context.Medicos
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Apellidos)
                    .ThenBy(m => m.Nombres)
                    .Select(m => new { 
                        m.Id, 
                        NombreCompleto = "Dr(a). " + m.Apellidos + " " + m.Nombres
                    })
                    .ToListAsync(),
                "Id",
                "NombreCompleto"
            );
        }

        // Método auxiliar para verificar si un historial médico existe
        private bool HistorialMedicoExists(int id)
        {
            return _context.HistorialesMedicos.Any(h => h.Id == id);
        }
    }
}
