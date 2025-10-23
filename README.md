# ClinicApp

## Pasos previos antes de usar este repositorio

### 1.- Crear base de datos en SQL Server para realizar Database First con EF-Core SqlServer
```sql
-- Ejecutar este script en SQL Server Management Studio o SQL Server Object Explorer
CREATE DATABASE ClinicAppDB;
GO

USE ClinicAppDB;
GO

-- Tabla Especialidades
CREATE TABLE Especialidades (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Nombre nvarchar(100) NOT NULL UNIQUE,
    Descripcion nvarchar(500) NULL,
    Activa bit NOT NULL DEFAULT 1
);

-- Tabla Pacientes
CREATE TABLE Pacientes (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Cedula nchar(10) NOT NULL UNIQUE,
    Nombres nvarchar(100) NOT NULL,
    Apellidos nvarchar(100) NOT NULL,
    FechaNacimiento date NOT NULL,
    Telefono nvarchar(20) NOT NULL,
    Email nvarchar(150) NOT NULL UNIQUE,
    Direccion nvarchar(250) NULL,
    TipoSangre nvarchar(5) NULL,
    ContactoEmergencia nvarchar(100) NULL,
    TelefonoEmergencia nvarchar(20) NULL,
    FechaRegistro datetime2 NOT NULL DEFAULT GETDATE(),
    Activo bit NOT NULL DEFAULT 1
);

-- Tabla Medicos
CREATE TABLE Medicos (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Cedula nchar(10) NOT NULL UNIQUE,
    Nombres nvarchar(100) NOT NULL,
    Apellidos nvarchar(100) NOT NULL,
    EspecialidadId int NOT NULL,
    NumeroLicencia nvarchar(50) NOT NULL UNIQUE,
    Telefono nvarchar(20) NULL,
    Email nvarchar(150) NULL,
    HorarioInicio time NOT NULL DEFAULT '08:00:00',
    HorarioFin time NOT NULL DEFAULT '17:00:00',
    Activo bit NOT NULL DEFAULT 1,
    FechaRegistro datetime2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (EspecialidadId) REFERENCES Especialidades(Id)
);

-- Tabla CitasMedicas
CREATE TABLE CitasMedicas (
    Id int IDENTITY(1,1) PRIMARY KEY,
    PacienteId int NOT NULL,
    MedicoId int NOT NULL,
    FechaHora datetime2 NOT NULL,
    Motivo nvarchar(500) NULL,
    Estado nvarchar(20) NOT NULL DEFAULT 'Programada',
    Observaciones nvarchar(1000) NULL,
    FechaCreacion datetime2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id),
    FOREIGN KEY (MedicoId) REFERENCES Medicos(Id)
);

-- Tabla HistorialesMedicos
CREATE TABLE HistorialesMedicos (
    Id int IDENTITY(1,1) PRIMARY KEY,
    PacienteId int NOT NULL,
    MedicoId int NOT NULL,
    FechaConsulta datetime2 NOT NULL,
    MotivoConsulta nvarchar(500) NULL,
    Diagnostico nvarchar(2000) NULL,
    Tratamiento nvarchar(1000) NULL,
    Medicamentos nvarchar(500) NULL,
    Observaciones nvarchar(1000) NULL,
    ProximaCita datetime2 NULL,
    FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id),
    FOREIGN KEY (MedicoId) REFERENCES Medicos(Id)
);

-- Datos iniciales para Especialidades
INSERT INTO Especialidades (Nombre, Descripcion) VALUES
('Medicina General', 'Atención médica general y preventiva'),
('Cardiología', 'Especialidad en enfermedades del corazón'),
('Pediatría', 'Atención médica para niños y adolescentes'),
('Ginecología', 'Atención médica para la mujer'),
('Dermatología', 'Especialidad en enfermedades de la piel'),
('Traumatología', 'Especialidad en lesiones del sistema músculo-esquelético');

-- Datos de prueba para Medicos
INSERT INTO Medicos (Cedula, Nombres, Apellidos, EspecialidadId, NumeroLicencia, Telefono, Email) VALUES
('1234567890', 'Carlos', 'Ramírez', 2, 'LIC-001', '0987654321', 'carlos.ramirez@clinicapp.com'),
('0987654321', 'Ana', 'López', 1, 'LIC-002', '0976543210', 'ana.lopez@clinicapp.com'),
('1122334455', 'Luis', 'Morales', 3, 'LIC-003', '0965432109', 'luis.morales@clinicapp.com');

-- Datos de prueba para Pacientes
INSERT INTO Pacientes (Cedula, Nombres, Apellidos, FechaNacimiento, Telefono, Email, TipoSangre) VALUES
('1111111111', 'María', 'González', '1985-03-15', '0912345678', 'maria.gonzalez@email.com', 'O+'),
('2222222222', 'Juan', 'Pérez', '1990-07-22', '0923456789', 'juan.perez@email.com', 'A+'),
('3333333333', 'Carmen', 'Silva', '2015-12-10', '0934567890', 'carmen.silva@email.com', 'B+');
```

### 2.- Instalar NuGet correspondientes.
```bash
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design 
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

### 3.- Hacer Scaffolding

```bash
 scaffold-dbcontext "server=NOMBREPC\SQLEXPRESS; initial catalog=DATABASE; Trusted_connection=True;Encrypt=False;" microsoft.entityframeworkcore.sqlserver -outputdir Models
```

### 4.- Agregar DBContext en program.cs y verificar el appsettings.json
```csharp
// DBContext
builder.Services.AddDbContext<ClinicAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

```csharp
//Appsettings.json
"ConnectionStrings": {
    "DefaultConnection": "server=PC06LAB318; initial catalog=ClinicAppDB; Trusted_connection=True;Encrypt=False"
}
```
