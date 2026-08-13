using AutoMapper;
using SchoolERP.Application.Features.StudentGuardian.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>
/// AutoMapper profile for the StudentGuardian join-entity feature.
/// Edited by Musaib Sikder For getting data directly form gurdian table to student table
/// </summary>
public class StudentGuardianProfile : Profile
{
    public StudentGuardianProfile()
    {
        CreateMap<StudentGuardian, StudentGuardianDto>()
     .ForMember(
         dest => dest.GuardianId,
         opt => opt.MapFrom(src => src.GuardianId))

     .ForMember(
         dest => dest.GuardianName,
         opt => opt.MapFrom(src => src.Guardian!.FullName))

     .ForMember(
         dest => dest.PhoneNumber,
         opt => opt.MapFrom(src => src.Guardian!.PhoneNumber))

     .ForMember(
         dest => dest.Relation,
         opt => opt.MapFrom(src => src.Relation));
        // Request DTO -> Entity
        CreateMap<CreateStudentGuardianDto, StudentGuardian>();
    }
}
