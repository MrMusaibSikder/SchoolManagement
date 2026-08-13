using AutoMapper;
using SchoolERP.Application.Features.StudentAttendance.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the StudentAttendance feature.</summary>
public class StudentAttendanceProfile : Profile
{
    public StudentAttendanceProfile()
    {
        // Entity -> Response DTO
        CreateMap<StudentAttendance, StudentAttendanceDto>()
            .ForMember(
                dest => dest.StudentName,
                opt => opt.MapFrom(src =>
                    src.Student != null
                    ? src.Student.FullName
                    : string.Empty
                ));



        // Create DTO -> Entity
        CreateMap<CreateStudentAttendanceDto, StudentAttendance>();



        // Update DTO -> Existing Entity
        CreateMap<UpdateStudentAttendanceDto, StudentAttendance>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.Student,
                opt => opt.Ignore()
            );
    }
}
