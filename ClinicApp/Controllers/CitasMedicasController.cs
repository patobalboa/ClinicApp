using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    public class CitasMedicasController : Controller
    {
        private readonly ClinicAppDbContext _context;
        private readonly ILogger<CitasMedicasController> _logger;

        public CitasMedicasController(ClinicAppDbContext context, ILogger<CitasMedicasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /CitasMedicas
        public async Task<IActionResult> Index(string estado = "")
        {
            try
            {
                var query = _context.CitasMedicas
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .AsQueryable();

                // Filtrar por estado si se proporciona
                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(c => c.Estado == estado);
                }

                var citas = await query
                    .OrderByDescending(c => c.FechaHora)
                    .ToListAsync();

                ViewBag.EstadoFiltro = estado;
                ViewBag.Estados = new List<string> { "Programada", "Confirmada", "Completada", "Cancelada" };

                _logger.LogInformation("Se cargaron {Count} citas médicas", citas.Count);
                return View(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de citas médicas");
                TempData["Error"] = "Error al cargar la lista de citas médicas";
                return View(new List<CitasMedica>());
            }
        }

        // GET: /CitasMedicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var cita = await _context.CitasMedicas
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cita == null)
                {
                    _logger.LogWarning("Cita médica con ID {Id} no encontrada", id);
                    return NotFound();
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cita médica con ID {Id}", id);
                TempData["Error"] = "Error al cargar los detalles de la cita médica";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /CitasMedicas/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasDesplegables();
            return View(new CitasMedica 
            { 
                Estado = "Programada",
                FechaHora = DateTime.Now.AddDays(1)
            });
        }

        // POST: /CitasMedicas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitasMedica cita)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                return View(cita);
            }

            try
            {
                // Validar que la fecha de la cita sea futura
                if (cita.FechaHora <= DateTime.Now)
                {
                    ModelState.AddModelError("FechaHora", "La fecha de la cita debe ser posterior a la fecha actual");
                    await CargarListasDesplegables();
                    return View(cita);
                }

                // Verificar disponibilidad del médico
                var medico = await _context.Medicos.FindAsync(cita.MedicoId);
                if (medico != null)
                {
                    var horaCita = TimeOnly.FromDateTime(cita.FechaHora);
                    if (horaCita < medico.HorarioInicio || horaCita > medico.HorarioFin)
                    {
                        ModelState.AddModelError("FechaHora", 
                            $"El médico solo atiende de {medico.HorarioInicio} a {medico.HorarioFin}");
                        await CargarListasDesplegables();
                        return View(cita);
                    }
                }

                // Verificar si el médico ya tiene una cita en ese horario
                var citaExistente = await _context.CitasMedicas
                    .AnyAsync(c => c.MedicoId == cita.MedicoId && 
                                   c.FechaHora == cita.FechaHora &&
                                   c.Estado != "Cancelada");

                if (citaExistente)
                {
                    ModelState.AddModelError("FechaHora", "El médico ya tiene una cita programada en este horario");
                    await CargarListasDesplegables();
                    return View(cita);
                }

                _context.CitasMedicas.Add(cita);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cita médica creada exitosamente (ID: {Id})", cita.Id);

                TempData["Success"] = "Cita médica creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cita médica");
                ModelState.AddModelError("", "Ocurrió un error al guardar la cita médica. Por favor, intente nuevamente.");
                await CargarListasDesplegables();
                return View(cita);
            }
        }

        // GET: /CitasMedicas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var cita = await _context.CitasMedicas.FindAsync(id);

                if (cita == null)
                {
                    _logger.LogWarning("Cita médica con ID {Id} no encontrada para edición", id);
                    return NotFound();
                }

                await CargarListasDesplegables();
                ViewBag.Estados = new SelectList(new[] { "Programada", "Confirmada", "Completada", "Cancelada" });
                return View(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cita médica para edición con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos de la cita médica";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /CitasMedicas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CitasMedica cita)
        {
            if (id != cita.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables();
                ViewBag.Estados = new SelectList(new[] { "Programada", "Confirmada", "Completada", "Cancelada" });
                return View(cita);
            }

            try
            {
                // Verificar disponibilidad del médico (si cambió fecha u hora)
                var citaExistente = await _context.CitasMedicas
                    .AnyAsync(c => c.MedicoId == cita.MedicoId && 
                                   c.FechaHora == cita.FechaHora &&
                                   c.Estado != "Cancelada" &&
                                   c.Id != cita.Id);

                if (citaExistente)
                {
                    ModelState.AddModelError("FechaHora", "El médico ya tiene una cita programada en este horario");
                    await CargarListasDesplegables();
                    ViewBag.Estados = new SelectList(new[] { "Programada", "Confirmada", "Completada", "Cancelada" });
                    return View(cita);
                }

                _context.Update(cita);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cita médica actualizada exitosamente (ID: {Id})", cita.Id);

                TempData["Success"] = "Cita médica actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CitaMedicaExists(cita.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar cita médica con ID {Id}", id);
                ModelState.AddModelError("", "La cita fue modificada por otro usuario. Por favor, recargue la página.");
                await CargarListasDesplegables();
                ViewBag.Estados = new SelectList(new[] { "Programada", "Confirmada", "Completada", "Cancelada" });
                return View(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cita médica con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar la cita médica. Por favor, intente nuevamente.");
                await CargarListasDesplegables();
                ViewBag.Estados = new SelectList(new[] { "Programada", "Confirmada", "Completada", "Cancelada" });
                return View(cita);
            }
        }

        // GET: /CitasMedicas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var cita = await _context.CitasMedicas
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cita == null)
                {
                    _logger.LogWarning("Cita médica con ID {Id} no encontrada para eliminación", id);
                    return NotFound();
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cita médica para eliminación con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos de la cita médica";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /CitasMedicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cita = await _context.CitasMedicas.FindAsync(id);

                if (cita == null)
                {
                    _logger.LogWarning("Cita médica con ID {Id} no encontrada para eliminación", id);
                    return NotFound();
                }

                _context.CitasMedicas.Remove(cita);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cita médica eliminada exitosamente (ID: {Id})", id);

                TempData["Success"] = "Cita médica eliminada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cita médica con ID {Id}", id);
                TempData["Error"] = "Error al eliminar la cita médica";
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

        // Método auxiliar para verificar si una cita médica existe
        private bool CitaMedicaExists(int id)
        {
            return _context.CitasMedicas.Any(c => c.Id == id);
        }
    }
}
