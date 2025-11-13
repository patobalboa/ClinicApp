using ClinicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    public class EspecialidadesController : Controller
    {
        private readonly ClinicAppDbContext _context;
        private readonly ILogger<EspecialidadesController> _logger;

        public EspecialidadesController(ClinicAppDbContext context, ILogger<EspecialidadesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Especialidades
        public async Task<IActionResult> Index()
        {
            try
            {
                var especialidades = await _context.Especialidades
                    .OrderBy(e => e.Nombre)
                    .ToListAsync();

                _logger.LogInformation("Se cargaron {Count} especialidades", especialidades.Count);
                return View(especialidades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de especialidades");
                TempData["Error"] = "Error al cargar la lista de especialidades";
                return View(new List<Especialidade>());
            }
        }

        // GET: /Especialidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var especialidad = await _context.Especialidades
                    .Include(e => e.Medicos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (especialidad == null)
                {
                    _logger.LogWarning("Especialidad con ID {Id} no encontrada", id);
                    return NotFound();
                }

                return View(especialidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar especialidad con ID {Id}", id);
                TempData["Error"] = "Error al cargar los detalles de la especialidad";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Especialidades/Create
        public IActionResult Create()
        {
            return View(new Especialidade { Activa = true });
        }

        // POST: /Especialidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especialidade especialidad)
        {
            if (!ModelState.IsValid)
            {
                return View(especialidad);
            }

            try
            {
                // Verificar si ya existe una especialidad con el mismo nombre
                var existeEspecialidad = await _context.Especialidades
                    .AnyAsync(e => e.Nombre.ToLower() == especialidad.Nombre.ToLower());

                if (existeEspecialidad)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una especialidad con este nombre");
                    return View(especialidad);
                }

                _context.Especialidades.Add(especialidad);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Especialidad creada exitosamente: {Nombre} (ID: {Id})",
                    especialidad.Nombre, especialidad.Id);

                TempData["Success"] = "Especialidad creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear especialidad: {Nombre}", especialidad.Nombre);
                ModelState.AddModelError("", "Ocurrió un error al guardar la especialidad. Por favor, intente nuevamente.");
                return View(especialidad);
            }
        }

        // GET: /Especialidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var especialidad = await _context.Especialidades.FindAsync(id);

                if (especialidad == null)
                {
                    _logger.LogWarning("Especialidad con ID {Id} no encontrada para edición", id);
                    return NotFound();
                }

                return View(especialidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar especialidad para edición con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos de la especialidad";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Especialidades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especialidade especialidad)
        {
            if (id != especialidad.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                return View(especialidad);
            }

            try
            {
                // Verificar nombre duplicado (excluyendo la actual)
                var existeEspecialidad = await _context.Especialidades
                    .AnyAsync(e => e.Nombre.ToLower() == especialidad.Nombre.ToLower() && e.Id != especialidad.Id);

                if (existeEspecialidad)
                {
                    ModelState.AddModelError("Nombre", "Ya existe otra especialidad con este nombre");
                    return View(especialidad);
                }

                _context.Update(especialidad);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Especialidad actualizada exitosamente: {Nombre} (ID: {Id})",
                    especialidad.Nombre, especialidad.Id);

                TempData["Success"] = "Especialidad actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EspecialidadExists(especialidad.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar especialidad con ID {Id}", id);
                ModelState.AddModelError("", "La especialidad fue modificada por otro usuario. Por favor, recargue la página.");
                return View(especialidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar especialidad con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar la especialidad. Por favor, intente nuevamente.");
                return View(especialidad);
            }
        }

        // GET: /Especialidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("ID no proporcionado");
            }

            try
            {
                var especialidad = await _context.Especialidades
                    .Include(e => e.Medicos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (especialidad == null)
                {
                    _logger.LogWarning("Especialidad con ID {Id} no encontrada para eliminación", id);
                    return NotFound();
                }

                return View(especialidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar especialidad para eliminación con ID {Id}", id);
                TempData["Error"] = "Error al cargar los datos de la especialidad";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Especialidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var especialidad = await _context.Especialidades
                    .Include(e => e.Medicos)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (especialidad == null)
                {
                    _logger.LogWarning("Especialidad con ID {Id} no encontrada para eliminación", id);
                    return NotFound();
                }

                // Verificar si tiene médicos asociados
                if (especialidad.Medicos.Any())
                {
                    TempData["Error"] = $"No se puede eliminar la especialidad '{especialidad.Nombre}' porque tiene {especialidad.Medicos.Count} médico(s) asociado(s)";
                    return RedirectToAction(nameof(Index));
                }

                _context.Especialidades.Remove(especialidad);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Especialidad eliminada exitosamente: {Nombre} (ID: {Id})",
                    especialidad.Nombre, id);

                TempData["Success"] = $"Especialidad '{especialidad.Nombre}' eliminada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar especialidad con ID {Id}", id);
                TempData["Error"] = "Error al eliminar la especialidad";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para verificar si una especialidad existe
        private bool EspecialidadExists(int id)
        {
            return _context.Especialidades.Any(e => e.Id == id);
        }
    }
}
