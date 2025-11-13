using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClinicApp.Validators
{
    /// <summary>
    /// Validación personalizada para RUT chileno
    /// Formato válido: 12.345.678-9 o 12345678-9 o 12345678-K
    /// </summary>
    public class RutChilenoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("El RUT es obligatorio");
            }

            string rut = value.ToString()!.Trim();

            // Limpiar el RUT (remover puntos y guiones)
            string rutLimpio = rut.Replace(".", "").Replace("-", "").ToUpper();

            // Validar longitud (mínimo 8 dígitos + 1 verificador, máximo 9 dígitos + 1 verificador)
            if (rutLimpio.Length < 8 || rutLimpio.Length > 9)
            {
                return new ValidationResult("El RUT debe tener entre 7 y 8 dígitos más el dígito verificador");
            }

            // Validar formato (números seguidos de un dígito verificador que puede ser 0-9 o K)
            if (!Regex.IsMatch(rutLimpio, @"^\d{7,8}[0-9K]$"))
            {
                return new ValidationResult("El formato del RUT no es válido");
            }

            // Separar número del dígito verificador
            string numero = rutLimpio.Substring(0, rutLimpio.Length - 1);
            char digitoVerificadorIngresado = rutLimpio[rutLimpio.Length - 1];

            // Calcular dígito verificador usando algoritmo módulo 11
            int suma = 0;
            int multiplicador = 2;

            // Recorrer de derecha a izquierda
            for (int i = numero.Length - 1; i >= 0; i--)
            {
                suma += int.Parse(numero[i].ToString()) * multiplicador;
                multiplicador++;
                if (multiplicador > 7)
                {
                    multiplicador = 2;
                }
            }

            int resto = suma % 11;
            int resultado = 11 - resto;

            // Determinar dígito verificador esperado
            char digitoVerificadorEsperado;
            if (resultado == 11)
            {
                digitoVerificadorEsperado = '0';
            }
            else if (resultado == 10)
            {
                digitoVerificadorEsperado = 'K';
            }
            else
            {
                digitoVerificadorEsperado = resultado.ToString()[0];
            }

            // Comparar dígitos verificadores
            if (digitoVerificadorIngresado != digitoVerificadorEsperado)
            {
                return new ValidationResult("El dígito verificador del RUT es inválido");
            }

            return ValidationResult.Success;
        }
    }
}
