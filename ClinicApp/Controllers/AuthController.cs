using ClinicApp.DTOs;
using ClinicApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: /Auth/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                // Validar credenciales
                var usuario = await _authService.ValidarCredenciales(dto.NombreUsuario, dto.Password);

                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                    return View(dto);
                }

                // Crear claims (información del usuario en la sesión)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("NombreCompleto", usuario.NombreCompleto)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = dto.Recordarme, // Mantener sesión si marcó "Recordarme"
                    ExpiresUtc = dto.Recordarme
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                // Iniciar sesión
                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, authProperties);

                _logger.LogInformation("Usuario {Usuario} inició sesión exitosamente", usuario.NombreUsuario);

                // Redirigir a la página solicitada o al dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login");
                ModelState.AddModelError("", "Ocurrió un error al iniciar sesión");
                return View(dto);
            }
        }

        // GET: /Auth/Registro
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Auth/Registro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var (exito, mensaje, usuario) = await _authService.RegistrarUsuario(dto);

                if (!exito)
                {
                    ModelState.AddModelError("", mensaje);
                    return View(dto);
                }

                TempData["Success"] = "Registro exitoso. Por favor, inicie sesión.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro");
                ModelState.AddModelError("", "Ocurrió un error al registrar el usuario");
                return View(dto);
            }
        }

        // POST: /Auth/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var nombreUsuario = User.Identity.Name;

            await HttpContext.SignOutAsync("CookieAuth");

            _logger.LogInformation("Usuario {Usuario} cerró sesión", nombreUsuario);

            TempData["Info"] = "Sesión cerrada exitosamente";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Auth/AccesoDenegado
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // GET: /Auth/Perfil
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _authService.ObtenerUsuarioPorId(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: /Auth/CambiarPassword
        [Authorize]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        // POST: /Auth/CambiarPassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var (exito, mensaje) = await _authService.CambiarPassword(
                    usuarioId, dto.PasswordActual, dto.NuevaPassword);

                if (!exito)
                {
                    ModelState.AddModelError("", mensaje);
                    return View(dto);
                }

                TempData["Success"] = mensaje;
                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                ModelState.AddModelError("", "Ocurrió un error al cambiar la contraseña");
                return View(dto);
            }
        }
    }
}
