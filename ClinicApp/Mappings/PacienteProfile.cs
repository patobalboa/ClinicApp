using AutoMapper;
using ClinicApp.DTOs.Pacientes;
using ClinicApp.Models;

namespace ClinicApp.Mappings
{
    /// <summary>
    /// Perfil de AutoMapper para mapeo de Pacientes
    /// </summary>
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            // Mapeo de Entidad a DTO (para lectura)
            CreateMap<Paciente, PacienteDto>();

            // Mapeo de CreateDTO a Entidad (para creación)
            CreateMap<PacienteCreateDto, Paciente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.CitasMedicas, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialesMedicos, opt => opt.Ignore());

            // Mapeo de UpdateDTO a Entidad (para actualización)
            CreateMap<PacienteUpdateDto, Paciente>()
                .ForMember(dest => dest.CitasMedicas, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialesMedicos, opt => opt.Ignore());

            // Mapeo de Entidad a UpdateDTO (para edición)
            CreateMap<Paciente, PacienteUpdateDto>();

            // Mapeo de Entidad a CreateDTO (si se necesita)
            CreateMap<Paciente, PacienteCreateDto>();
        }
    }
}
