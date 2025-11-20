using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Models;

public partial class ClinicAppDbContext : DbContext
{
    public ClinicAppDbContext()
    {
    }

    public ClinicAppDbContext(DbContextOptions<ClinicAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CitasMedica> CitasMedicas { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<HistorialesMedico> HistorialesMedicos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CitasMedica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CitasMed__3214EC0702581A23");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Programada");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Motivo).HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(1000);

            entity.HasOne(d => d.Medico).WithMany(p => p.CitasMedicas)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CitasMedi__Medic__4CA06362");

            entity.HasOne(d => d.Paciente).WithMany(p => p.CitasMedicas)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CitasMedi__Pacie__4BAC3F29");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Especial__3214EC0773C9E93D");

            entity.HasIndex(e => e.Nombre, "UQ__Especial__75E3EFCFFDFABCF1").IsUnique();

            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<HistorialesMedico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC071FC3A480");

            entity.Property(e => e.Diagnostico).HasMaxLength(2000);
            entity.Property(e => e.Medicamentos).HasMaxLength(500);
            entity.Property(e => e.MotivoConsulta).HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.Tratamiento).HasMaxLength(1000);

            entity.HasOne(d => d.Medico).WithMany(p => p.HistorialesMedicos)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Medic__5070F446");

            entity.HasOne(d => d.Paciente).WithMany(p => p.HistorialesMedicos)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Pacie__4F7CD00D");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicos__3214EC07D5E59272");

            entity.HasIndex(e => e.NumeroLicencia, "UQ__Medicos__8DD65A0611B1F9E1").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__Medicos__B4ADFE38D803B7C8").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Cedula)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HorarioFin).HasDefaultValue(new TimeOnly(17, 0, 0));
            entity.Property(e => e.HorarioInicio).HasDefaultValue(new TimeOnly(8, 0, 0));
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroLicencia).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(20);

            entity.HasOne(d => d.Especialidad).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.EspecialidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicos__Especia__46E78A0C");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Paciente__3214EC07C7E06AFF");

            entity.HasIndex(e => e.Email, "UQ__Paciente__A9D10534055FB4FB").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__Paciente__B4ADFE38A2E3415D").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Cedula)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ContactoEmergencia).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TelefonoEmergencia).HasMaxLength(20);
            entity.Property(e => e.TipoSangre).HasMaxLength(5);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0763375B84");

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__6B0F5AE07467BF15").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D1053421E725AD").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Rol).HasMaxLength(20);
            entity.Property(e => e.UltimoAcceso).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
