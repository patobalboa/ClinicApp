using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Validators
{
    /// <summary>
    /// Validación personalizada para verificar que la fecha de nacimiento sea válida
    /// </summary>
    public class FechaValidaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly fecha)
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                
                // No puede ser una fecha futura
                if (fecha > hoy)
                {
                    return new ValidationResult("La fecha de nacimiento no puede ser una fecha futura");
                }

                // No puede ser mayor a 120 años
                var edadMaxima = 120;
                if (fecha < hoy.AddYears(-edadMaxima))
                {
                    return new ValidationResult($"La fecha de nacimiento no puede ser mayor a {edadMaxima} años atrás");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Fecha de nacimiento inválida");
        }
    }
}
