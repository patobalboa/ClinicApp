using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Validators
{
    /// <summary>
    /// Validación personalizada para verificar que el paciente sea mayor de edad
    /// </summary>
    public class MayorDeEdadAttribute : ValidationAttribute
    {
        private readonly int _edadMinima;

        public MayorDeEdadAttribute(int edadMinima = 18)
        {
            _edadMinima = edadMinima;
            ErrorMessage = $"El paciente debe tener al menos {_edadMinima} años";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly fechaNacimiento)
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                var edad = hoy.Year - fechaNacimiento.Year;
                
                if (fechaNacimiento > hoy.AddYears(-edad))
                {
                    edad--;
                }

                if (edad < _edadMinima)
                {
                    return new ValidationResult(ErrorMessage ?? $"El paciente debe tener al menos {_edadMinima} años");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Fecha de nacimiento inválida");
        }
    }
}
