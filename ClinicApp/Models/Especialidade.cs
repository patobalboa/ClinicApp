using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public partial class Especialidade
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}
