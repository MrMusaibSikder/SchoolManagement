using AutoMapper;
using SchoolERP.Application.Features.Student.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>
/// AutoMapper profile for the Student feature.
/// Edited By Musaib Sikder
/// </summary>
public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>()
    .ForMember(
        dest => dest.Guardians,
        opt => opt.MapFrom(
            src => src.StudentGuardians));

        CreateMap<CreateStudentDto, Student>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());

        CreateMap<UpdateStudentDto, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
    }
}
