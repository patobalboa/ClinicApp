using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }
}

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Doctor = "Doctor";
    public const string Recepcionista = "Recepcionista";
}
