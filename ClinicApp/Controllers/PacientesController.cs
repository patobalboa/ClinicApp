using AutoMapper;
using ClinicApp.DTOs.Pacientes;
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
        private readonly IMapper _mapper;

        public PacientesController(ClinicAppDbContext context, ILogger<PacientesController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
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

                // Mapear entidades a DTOs
                var pacientesDto = _mapper.Map<List<PacienteDto>>(pacientes);

                _logger.LogInformation("Se cargaron {Count} pacientes", pacientesDto.Count);
                return View(pacientesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de pacientes");
                TempData["Error"] = "Error al cargar la lista de pacientes";
                return View(new List<PacienteDto>());
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

                // Mapear entidad a DTO
                var pacienteDto = _mapper.Map<PacienteDto>(paciente);
                return View(pacienteDto);
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
            return View(new PacienteCreateDto());
        }

        // POST: /Pacientes/Create
        // CREATE - Procesar la creación de un nuevo paciente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteCreateDto pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return View(pacienteDto);
            }

            try
            {
                // Verificar si ya existe un paciente con la misma cédula
                var existePaciente = await _context.Pacientes
                    .AnyAsync(p => p.Cedula == pacienteDto.Cedula);

                if (existePaciente)
                {
                    ModelState.AddModelError("Cedula", "Ya existe un paciente registrado con esta cédula");
                    return View(pacienteDto);
                }

                // Verificar si ya existe un paciente con el mismo email
                if (!string.IsNullOrEmpty(pacienteDto.Email))
                {
                    var existeEmail = await _context.Pacientes
                        .AnyAsync(p => p.Email == pacienteDto.Email);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe un paciente registrado con este email");
                        return View(pacienteDto);
                    }
                }

                // Mapear DTO a entidad
                var paciente = _mapper.Map<Paciente>(pacienteDto);
                
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
                    pacienteDto.Nombres, pacienteDto.Apellidos);

                ModelState.AddModelError("", "Ocurrió un error al guardar el paciente. Por favor, intente nuevamente.");
                return View(pacienteDto);
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

                // Mapear entidad a UpdateDTO
                var pacienteDto = _mapper.Map<PacienteUpdateDto>(paciente);
                return View(pacienteDto);
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
        public async Task<IActionResult> Edit(int id, PacienteUpdateDto pacienteDto)
        {
            if (id != pacienteDto.Id)
            {
                return BadRequest("ID no coincide");
            }

            if (!ModelState.IsValid)
            {
                return View(pacienteDto);
            }

            try
            {
                // Verificar si existe otro paciente con la misma cédula (excluyendo el actual)
                var existePaciente = await _context.Pacientes
                    .AnyAsync(p => p.Cedula == pacienteDto.Cedula && p.Id != pacienteDto.Id);

                if (existePaciente)
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro paciente registrado con esta cédula");
                    return View(pacienteDto);
                }

                // Verificar email duplicado (excluyendo el actual)
                if (!string.IsNullOrEmpty(pacienteDto.Email))
                {
                    var existeEmail = await _context.Pacientes
                        .AnyAsync(p => p.Email == pacienteDto.Email && p.Id != pacienteDto.Id);

                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Ya existe otro paciente registrado con este email");
                        return View(pacienteDto);
                    }
                }

                // Mapear DTO a entidad
                var paciente = _mapper.Map<Paciente>(pacienteDto);
                
                _context.Update(paciente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Paciente actualizado exitosamente: {Nombres} {Apellidos} (ID: {Id})",
                    paciente.Nombres, paciente.Apellidos, paciente.Id);

                TempData["Success"] = "Paciente actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PacienteExists(pacienteDto.Id))
                {
                    return NotFound();
                }

                _logger.LogError(ex, "Error de concurrencia al actualizar paciente con ID {Id}", id);
                ModelState.AddModelError("", "El paciente fue modificado por otro usuario. Por favor, recargue la página.");
                return View(pacienteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente con ID {Id}", id);
                ModelState.AddModelError("", "Ocurrió un error al actualizar el paciente. Por favor, intente nuevamente.");
                return View(pacienteDto);
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

                // Mapear entidad a DTO
                var pacienteDto = _mapper.Map<PacienteDto>(paciente);
                return View(pacienteDto);
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
