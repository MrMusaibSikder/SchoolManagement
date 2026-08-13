using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Application.Features.Authentication.DTOs;
using SchoolERP.Application.Features.Authentication.Validators;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;
using SchoolERP.Application.Features.EmployeeAttendance.Validators;
using SchoolERP.Application.Features.Exam.DTOs;
using SchoolERP.Application.Features.Exam.Validators;
using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Application.Features.ExamSchedule.Validators;
using SchoolERP.Application.Features.ExamType.DTOs;
using SchoolERP.Application.Features.ExamType.Validators;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;
using SchoolERP.Application.Features.ExamWeightSetup.Validators;
using SchoolERP.Application.Features.FeeCategory.DTOs;
using SchoolERP.Application.Features.FeeCategory.Validators;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Application.Features.FeeStructure.Validators;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Application.Features.FeeType.Validators;
using SchoolERP.Application.Features.GradeSetup.DTOs;
using SchoolERP.Application.Features.GradeSetup.Validators;
using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Application.Features.Invoice.Validators;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Application.Features.LateFineRule.Validators;
using SchoolERP.Application.Features.Notice.DTOs;
using SchoolERP.Application.Features.Notice.Validators;
using SchoolERP.Application.Features.Payment.DTOs;
using SchoolERP.Application.Features.Payment.Validators;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Permission.Validators;
using SchoolERP.Application.Features.Receipt.DTOs;
using SchoolERP.Application.Features.Receipt.Validators;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Application.Features.Result.Validators;
using SchoolERP.Application.Features.Role.DTOs;
using SchoolERP.Application.Features.Role.Validators;
using SchoolERP.Application.Features.RolePermission.DTOs;
using SchoolERP.Application.Features.RolePermission.Validators;
using SchoolERP.Application.Features.SmsLog.DTOs;
using SchoolERP.Application.Features.SmsLog.Validators;
using SchoolERP.Application.Features.SmsTemplate.DTOs;
using SchoolERP.Application.Features.SmsTemplate.Validators;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Application.Features.StudentFeeConcession.Validators;
using SchoolERP.Application.Features.User.DTOs;
using SchoolERP.Application.Features.User.Validators;
using SchoolERP.Application.Features.UserRole.DTOs;
using SchoolERP.Application.Features.UserRole.Validators;
using System.Reflection;


namespace SchoolERP.Application
{
    /// <summary>
    /// Registers Application-layer services: AutoMapper profiles, FluentValidation
    /// validators and MediatR handlers discovered in this assembly.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // AutoMapper: registers every Profile (one per feature) in this assembly.
            services.AddAutoMapper(assembly);
            // FluentValidation: registered explicitly (one line per validator) to stay
            // consistent with how repositories/services are registered elsewhere in
            // this project, rather than relying on assembly-scanning extensions.

            // Authentication
            services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
            services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
            services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();
            services.AddScoped<IValidator<ForgotPasswordDto>, ForgotPasswordDtoValidator>();
            services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();
            services.AddScoped<IValidator<RefreshTokenRequestDto>, RefreshTokenRequestDtoValidator>();
            services.AddScoped<IValidator<LogoutRequestDto>, LogoutRequestDtoValidator>();

            // Role / Permission administration
            services.AddScoped<IValidator<CreateRoleDto>, CreateRoleDtoValidator>();
            services.AddScoped<IValidator<UpdateRoleDto>, UpdateRoleDtoValidator>();
            services.AddScoped<IValidator<CreatePermissionDto>, CreatePermissionDtoValidator>();
            services.AddScoped<IValidator<UpdatePermissionDto>, UpdatePermissionDtoValidator>();
            services.AddScoped<IValidator<AssignPermissionsToRoleDto>, AssignPermissionsToRoleDtoValidator>();

            // User administration
            services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
            services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
            services.AddScoped<IValidator<AssignRoleToUserDto>, AssignRoleToUserDtoValidator>();

            // Employee Attendance
            services.AddScoped<IValidator<CreateEmployeeAttendanceDto>, CreateEmployeeAttendanceDtoValidator>();
            services.AddScoped<IValidator<UpdateEmployeeAttendanceDto>, UpdateEmployeeAttendanceDtoValidator>();
            services.AddScoped<IValidator<BulkEmployeeAttendanceDto>, BulkEmployeeAttendanceDtoValidator>();

            // SMS Template
            services.AddScoped<IValidator<CreateSmsTemplateDto>, CreateSmsTemplateDtoValidator>();
            services.AddScoped<IValidator<UpdateSmsTemplateDto>, UpdateSmsTemplateDtoValidator>();
            services.AddScoped<IValidator<SmsTemplateQueryDto>, SmsTemplateQueryDtoValidator>();

            // SMS Log
            services.AddScoped<IValidator<CreateSmsLogDto>, CreateSmsLogDtoValidator>();
            services.AddScoped<IValidator<SmsLogQueryDto>, SmsLogQueryDtoValidator>();

            // Notice
            services.AddScoped<IValidator<CreateNoticeDto>, CreateNoticeDtoValidator>();
            services.AddScoped<IValidator<UpdateNoticeDto>, UpdateNoticeDtoValidator>();
            services.AddScoped<IValidator<NoticeQueryDto>, NoticeQueryDtoValidator>();

            // Exam Type
            services.AddScoped<IValidator<CreateExamTypeDto>, CreateExamTypeDtoValidator>();
            services.AddScoped<IValidator<UpdateExamTypeDto>, UpdateExamTypeDtoValidator>();

            // Exam
            services.AddScoped<IValidator<CreateExamDto>, CreateExamDtoValidator>();
            services.AddScoped<IValidator<UpdateExamDto>, UpdateExamDtoValidator>();

            // Exam Schedule
            services.AddScoped<IValidator<CreateExamScheduleDto>, CreateExamScheduleDtoValidator>();
            services.AddScoped<IValidator<UpdateExamScheduleDto>, UpdateExamScheduleDtoValidator>();

            // Result (Mark Entry)
            services.AddScoped<IValidator<CreateResultDto>, CreateResultDtoValidator>();
            services.AddScoped<IValidator<UpdateResultDto>, UpdateResultDtoValidator>();
            services.AddScoped<IValidator<BulkMarkEntryDto>, BulkMarkEntryDtoValidator>();

            // Exam Weight Setup
            services.AddScoped<IValidator<CreateExamWeightSetupDto>, CreateExamWeightSetupDtoValidator>();
            services.AddScoped<IValidator<UpdateExamWeightSetupDto>, UpdateExamWeightSetupDtoValidator>();
            services.AddScoped<IValidator<AddExamWeightItemDto>, AddExamWeightItemDtoValidator>();
            services.AddScoped<IValidator<UpdateExamWeightItemDto>, UpdateExamWeightItemDtoValidator>();

            // Grade Setup
            services.AddScoped<IValidator<CreateGradeSetupDto>, CreateGradeSetupDtoValidator>();
            services.AddScoped<IValidator<UpdateGradeSetupDto>, UpdateGradeSetupDtoValidator>();
            // ... (existing validators remain unchanged) ...

            // ========== ⭐ FEE MANAGEMENT VALIDATORS ==========

            // FeeCategory
            services.AddScoped<IValidator<CreateFeeCategoryDto>, CreateFeeCategoryDtoValidator>();
            services.AddScoped<IValidator<UpdateFeeCategoryDto>, UpdateFeeCategoryDtoValidator>();

            // FeeType
            services.AddScoped<IValidator<CreateFeeTypeDto>, CreateFeeTypeDtoValidator>();
            services.AddScoped<IValidator<UpdateFeeTypeDto>, UpdateFeeTypeDtoValidator>();

            // FeeStructure
            services.AddScoped<IValidator<CreateFeeStructureDto>, CreateFeeStructureDtoValidator>();
            services.AddScoped<IValidator<CreateFeeStructureItemDto>, CreateFeeStructureItemDtoValidator>();
            services.AddScoped<IValidator<UpdateFeeStructureDto>, UpdateFeeStructureDtoValidator>();
            services.AddScoped<IValidator<UpdateFeeStructureItemDto>, UpdateFeeStructureItemDtoValidator>();

            // StudentFeeConcession
            services.AddScoped<IValidator<CreateStudentFeeConcessionDto>, CreateStudentFeeConcessionDtoValidator>();
            services.AddScoped<IValidator<UpdateStudentFeeConcessionDto>, UpdateStudentFeeConcessionDtoValidator>();
            services.AddScoped<IValidator<ApproveConcessionDto>, ApproveConcessionDtoValidator>();

            // Invoice
            services.AddScoped<IValidator<CreateInvoiceDto>, CreateInvoiceDtoValidator>();
            services.AddScoped<IValidator<CreateInvoiceItemDto>, CreateInvoiceItemDtoValidator>();
            services.AddScoped<IValidator<CancelInvoiceDto>, CancelInvoiceDtoValidator>();
            services.AddScoped<IValidator<GenerateMonthlyInvoicesDto>, GenerateMonthlyInvoicesDtoValidator>();
            // Payment
            services.AddScoped<IValidator<CreatePaymentDto>, CreatePaymentDtoValidator>();
            services.AddScoped<IValidator<VoidPaymentDto>, VoidPaymentDtoValidator>();

            // Receipt
            services.AddScoped<IValidator<VoidReceiptDto>, VoidReceiptDtoValidator>();

            // LateFineRule
            services.AddScoped<IValidator<CreateLateFineRuleDto>, CreateLateFineRuleDtoValidator>();
            services.AddScoped<IValidator<UpdateLateFineRuleDto>, UpdateLateFineRuleDtoValidator>();

            return services;
        }
    }
}
