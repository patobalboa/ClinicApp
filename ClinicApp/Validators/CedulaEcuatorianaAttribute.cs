using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClinicApp.Validators
{
    /// <summary>
    /// Validación personalizada para cédula ecuatoriana
    /// </summary>
    public class CedulaEcuatorianaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("La cédula es obligatoria");
            }

            string cedula = value.ToString()!.Trim();

            // Validar longitud
            if (cedula.Length != 10)
            {
                return new ValidationResult("La cédula debe tener exactamente 10 dígitos");
            }

            // Validar que solo contenga números
            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
            {
                return new ValidationResult("La cédula debe contener solo números");
            }

            // Validar código de provincia (primeros 2 dígitos)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
            {
                return new ValidationResult("Los primeros dos dígitos de la cédula no corresponden a una provincia válida (01-24)");
            }

            // Validar tercer dígito (debe ser menor a 6 para personas naturales)
            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito > 5)
            {
                return new ValidationResult("El tercer dígito de la cédula debe ser menor a 6");
            }

            // Validar dígito verificador (algoritmo módulo 10)
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int valor = int.Parse(cedula.Substring(i, 1)) * coeficientes[i];
                if (valor > 9)
                {
                    valor -= 9;
                }
                suma += valor;
            }

            int residuo = suma % 10;
            int verificador = residuo == 0 ? 0 : 10 - residuo;
            int digitoVerificador = int.Parse(cedula.Substring(9, 1));

            if (verificador != digitoVerificador)
            {
                return new ValidationResult("El dígito verificador de la cédula es inválido");
            }

            return ValidationResult.Success;
        }
    }
}
