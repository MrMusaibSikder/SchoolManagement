using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Application.Features.StudentFeeConcession.Interfaces;


namespace SchoolERP.Infrastructure.Services
{
    public class StudentFeeConcessionService : IStudentFeeConcessionService
    {
        private readonly ICurrentEmployeeService _currentEmployee;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateStudentFeeConcessionDto> _createValidator;
        private readonly IValidator<UpdateStudentFeeConcessionDto> _updateValidator;
        private readonly IValidator<ApproveConcessionDto> _approveValidator;

        public StudentFeeConcessionService(
            IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser,
            IValidator<CreateStudentFeeConcessionDto> createValidator,
            IValidator<UpdateStudentFeeConcessionDto> updateValidator,
            IValidator<ApproveConcessionDto> approveValidator,
            ICurrentEmployeeService currentEmployeeService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _approveValidator = approveValidator;
            _currentEmployee = currentEmployeeService;
        }

        public async Task<IReadOnlyList<StudentFeeConcessionListDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var list = await _unitOfWork.StudentFeeConcessionRepository.GetByStudentIdAsync(studentId, cancellationToken);
            return _mapper.Map<IReadOnlyList<StudentFeeConcessionListDto>>(list);
        }

        public async Task<IReadOnlyList<StudentFeeConcessionListDto>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
        {
            var list = await _unitOfWork.StudentFeeConcessionRepository.GetPendingApprovalsAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<StudentFeeConcessionListDto>>(list);
        }

        public async Task<StudentFeeConcessionDto> CreateAsync(CreateStudentFeeConcessionDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = _mapper.Map<SchoolERP.Domain.Entities.StudentFeeConcession>(request);
            entity.IsApproved = !request.RequiresApproval; //If  approval not need then auto approved.
            entity.IsActive = true;

            await _unitOfWork.StudentFeeConcessionRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<StudentFeeConcessionDto>(entity);
        }

        public async Task<StudentFeeConcessionDto> UpdateAsync(int id, UpdateStudentFeeConcessionDto request, CancellationToken cancellationToken = default)
        {
            if (id != request.Id) throw new BadRequestException("Route id and body id do not match.");

            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var entity = await _unitOfWork.StudentFeeConcessionRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.StudentFeeConcession), id);

            entity.Type = request.Type;
            entity.Value = request.Value;
            entity.Reason = request.Reason;
            entity.ValidFrom = request.ValidFrom;
            entity.ValidTo = request.ValidTo;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<StudentFeeConcessionDto>(entity);
        }

        public async Task<StudentFeeConcessionDto> ApproveAsync(ApproveConcessionDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _approveValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var employeeId = await _currentEmployee.GetIdAsync(cancellationToken);   // async lookup, exception  handle inside
               

            var entity = await _unitOfWork.StudentFeeConcessionRepository.GetByIdTrackedAsync(request.ConcessionId, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.StudentFeeConcession), request.ConcessionId);

            if (entity.IsApproved)
                throw new BadRequestException("Concession is already approved.");

            entity.IsApproved = true;
            entity.ApprovedByEmployeeId = employeeId;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<StudentFeeConcessionDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.StudentFeeConcessionRepository.GetByIdTrackedAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.StudentFeeConcession), id);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
