# Validación de RUT Chileno en ClinicApp

## ?? Descripción

Se implementó la validación de RUT chileno para el módulo de Pacientes utilizando DTOs (Data Transfer Objects) y validación personalizada mediante atributos de validación.

## ?? Cambios Realizados

### 1. Validador de RUT Chileno

**Archivo:** `ClinicApp\Validators\RutChilenoAttribute.cs`

- Validador personalizado que hereda de `ValidationAttribute`
- Acepta RUT con o sin formato (12.345.678-9 o 12345678-9)
- Valida el algoritmo módulo 11 del RUT chileno
- Soporta dígito verificador numérico (0-9) o letra K
- Validaciones implementadas:
  - Longitud correcta (7-8 dígitos + verificador)
  - Formato válido
  - Dígito verificador correcto según algoritmo módulo 11

### 2. DTOs de Pacientes

#### PacienteCreateDto
**Archivo:** `ClinicApp\DTOs\Pacientes\PacienteCreateDto.cs`
- Cambiado de `[CedulaEcuatoriana]` a `[RutChileno]`
- Todas las validaciones requeridas para crear un nuevo paciente

#### PacienteUpdateDto
**Archivo:** `ClinicApp\DTOs\Pacientes\PacienteUpdateDto.cs`
- Cambiado de `[CedulaEcuatoriana]` a `[RutChileno]`
- Incluye campos adicionales como `Id` y `FechaRegistro`

#### PacienteDto
**Archivo:** `ClinicApp\DTOs\Pacientes\PacienteDto.cs`
- DTO para lectura de datos (sin cambios estructurales)
- Incluye propiedades calculadas como `NombreCompleto` y `Edad`

### 3. Controlador de Pacientes

**Archivo:** `ClinicApp\Controllers\PacientesController.cs`

Actualizaciones:
- Mensajes de error cambiados de "cédula" a "RUT"
- Validaciones duplicadas actualizadas:
  - "Ya existe un paciente registrado con este RUT"
  - "Ya existe otro paciente registrado con este RUT"

### 4. Vistas Actualizadas

#### Create.cshtml
- Modelo cambiado a `PacienteCreateDto`
- Label cambiado de "Cédula" a "RUT"
- Placeholder actualizado: "Ej: 12.345.678-9 o 12345678-9"
- JavaScript para formateo automático del RUT mientras se escribe
- Campos de contacto de emergencia añadidos

#### Edit.cshtml
- Modelo cambiado a `PacienteUpdateDto`
- Label cambiado de "Cédula" a "RUT"
- JavaScript para formateo automático del RUT
- Campos adicionales añadidos (ContactoEmergencia, TelefonoEmergencia)

#### Index.cshtml
- Modelo cambiado a `IEnumerable<PacienteDto>`
- Display de "RUT" en lugar de "Cédula"
- Mostrar estado Activo/Inactivo del paciente
- Usar `NombreCompleto` del DTO

#### Details.cshtml
- Modelo cambiado a `PacienteDto`
- Display completo de información del paciente
- Sección de contacto de emergencia
- Mostrar estado y fecha de registro

#### Delete.cshtml
- Modelo cambiado a `PacienteDto`
- Display de "RUT" en lugar de "Cédula"
- Usar `NombreCompleto` del DTO

## ?? Formato del RUT Chileno

El RUT chileno tiene las siguientes características:

### Estructura
- **Número:** 7 u 8 dígitos
- **Dígito Verificador:** 1 carácter (0-9 o K)
- **Formato:** XX.XXX.XXX-X

### Ejemplos Válidos
- `12.345.678-9`
- `12345678-9`
- `9.876.543-K`
- `9876543-K`
- `1234567-0`

### Algoritmo de Validación (Módulo 11)

```
1. Se toma el número del RUT (sin el dígito verificador)
2. Se multiplica cada dígito por 2, 3, 4, 5, 6, 7 (de derecha a izquierda, repetitivo)
3. Se suman todos los resultados
4. Se divide la suma por 11
5. Se resta el resto de 11
6. Casos especiales:
   - Si el resultado es 11 ? dígito verificador = 0
   - Si el resultado es 10 ? dígito verificador = K
   - Si no ? dígito verificador = resultado
```

### Ejemplo de Validación

RUT: `12.345.678-9`

```
Número: 12345678
Multiplicadores: 2 3 4 5 6 7 2 3

8×2 = 16
7×3 = 21
6×4 = 24
5×5 = 25
4×6 = 24
3×7 = 21
2×2 = 4
1×3 = 3

Suma = 138
138 ÷ 11 = 12 con resto 6
11 - 6 = 5

Dígito verificador esperado: 5
Dígito verificador ingresado: 9
Resultado: ? RUT inválido
```

## ?? Funcionalidades JavaScript

### Formateo Automático
Las vistas Create y Edit incluyen JavaScript que formatea automáticamente el RUT mientras el usuario escribe:

```javascript
// Formatea: 123456789 ? 12.345.678-9
document.getElementById('Cedula').addEventListener('input', function(e) {
    let value = e.target.value.replace(/[^0-9kK]/g, '');
    if (value.length > 1) {
        let rut = value.slice(0, -1);
        let dv = value.slice(-1).toUpperCase();
        
        if (rut.length > 6) {
            rut = rut.slice(0, -6) + '.' + rut.slice(-6, -3) + '.' + rut.slice(-3);
        } else if (rut.length > 3) {
            rut = rut.slice(0, -3) + '.' + rut.slice(-3);
        }
        
        e.target.value = rut + '-' + dv;
    }
});
```

## ? Validaciones Implementadas

### En el DTO (PacienteCreateDto / PacienteUpdateDto)

1. **RUT:** `[Required]` + `[RutChileno]`
2. **Nombres:** `[Required]`, `[StringLength(100, MinimumLength = 2)]`, `[RegularExpression]` (solo letras)
3. **Apellidos:** `[Required]`, `[StringLength(100, MinimumLength = 2)]`, `[RegularExpression]` (solo letras)
4. **FechaNacimiento:** `[Required]`, `[DataType(DataType.Date)]`, `[FechaValida]`
5. **Teléfono:** `[Required]`, `[Phone]`, `[StringLength(20, MinimumLength = 10)]`, `[RegularExpression]`
6. **Email:** `[Required]`, `[EmailAddress]`, `[StringLength(150)]`
7. **TipoSangre:** `[RegularExpression]` (A+, A-, B+, B-, AB+, AB-, O+, O-)
8. **ContactoEmergencia:** `[StringLength(100)]` (opcional)
9. **TelefonoEmergencia:** `[Phone]`, `[StringLength(20)]` (opcional)

### En el Controlador

1. **Verificación de RUT duplicado** en Create
2. **Verificación de RUT duplicado** en Edit (excluyendo el registro actual)
3. **Verificación de Email duplicado** en Create y Edit

## ?? Notas Importantes

- La propiedad en el modelo y DTOs se mantiene como `Cedula` para no romper la base de datos
- El display en las vistas muestra "RUT" para el usuario final
- El validador acepta RUT con o sin formato (puntos y guión)
- La validación es tanto del lado del cliente (JavaScript) como del servidor (C#)

## ?? Dependencias

- `System.ComponentModel.DataAnnotations`
- `System.Text.RegularExpressions`
- AutoMapper (para mapeo entre entidades y DTOs)

## ?? Mejoras Futuras

1. Crear un helper de Razor para formatear RUT en las vistas
2. Agregar validación del lado del cliente con jQuery Validation
3. Crear un servicio de validación de RUT reutilizable
4. Agregar pruebas unitarias para el validador de RUT
