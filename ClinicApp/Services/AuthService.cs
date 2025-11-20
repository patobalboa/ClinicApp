using ClinicApp.DTOs;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ClinicApp.Services
{
    /// <summary>
    /// Servicio para manejar autenticación y registro de usuarios
    /// </summary>
    public class AuthService
    {
        private readonly ClinicAppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ClinicAppDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Valida credenciales de usuario
        /// </summary>
        public async Task<Usuario?> ValidarCredenciales(string nombreUsuario, string password)
        {
            _logger.LogInformation("Intentando login para usuario: {Usuario}", nombreUsuario);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

            if (usuario == null)
            {
                _logger.LogWarning("Usuario no encontrado o inactivo: {Usuario}", nombreUsuario);
                return null;
            }

            _logger.LogInformation("Usuario encontrado: {Usuario}, ID: {Id}", usuario.NombreUsuario, usuario.Id);
            _logger.LogInformation("Hash almacenado (primeros 30 caracteres): {Hash}...", 
                usuario.PasswordHash.Substring(0, Math.Min(30, usuario.PasswordHash.Length)));

            // Verificar password con BCrypt
            bool passwordValido = false;
            try
            {
                passwordValido = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
                _logger.LogInformation("Verificación BCrypt: {Resultado}", passwordValido ? "EXITOSA" : "FALLIDA");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar password con BCrypt");
                return null;
            }

            if (!passwordValido)
            {
                _logger.LogWarning("Password inválido para usuario: {Usuario}", nombreUsuario);
                _logger.LogWarning("Tip: Verifique que la contraseña sea exactamente: Admin123! (con A mayúscula y ! al final)");
                return null;
            }

            // Actualizar último acceso
            usuario.UltimoAcceso = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Login exitoso para usuario: {Usuario}", nombreUsuario);
            return usuario;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        public async Task<(bool Exito, string Mensaje, Usuario Usuario)> RegistrarUsuario(RegistroDto dto)
        {
            // Verificar si el usuario ya existe
            var existeUsuario = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == dto.NombreUsuario);

            if (existeUsuario)
            {
                return (false, "El nombre de usuario ya está en uso", null);
            }

            // Verificar si el email ya existe
            var existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (existeEmail)
            {
                return (false, "El email ya está registrado", null);
            }

            // Hashear password con BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Crear usuario
            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto.Trim(),
                NombreUsuario = dto.NombreUsuario.Trim().ToLower(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                Rol = dto.Rol,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario registrado exitosamente: {Usuario} ({Rol})",
                usuario.NombreUsuario, usuario.Rol);

            return (true, "Usuario registrado exitosamente", usuario);
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        public async Task<(bool Exito, string Mensaje)> CambiarPassword(
            int usuarioId, string passwordActual, string nuevaPassword)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return (false, "Usuario no encontrado");
            }

            // Verificar password actual
            bool passwordValido = BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash);

            if (!passwordValido)
            {
                _logger.LogWarning("Intento fallido de cambio de contraseña para usuario: {Usuario}",
                    usuario.NombreUsuario);
                return (false, "La contraseña actual es incorrecta");
            }

            // Hashear nueva password
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Usuario}",
                usuario.NombreUsuario);

            return (true, "Contraseña cambiada exitosamente");
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        public async Task<Usuario> ObtenerUsuarioPorNombre(string nombreUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }
    }
}
