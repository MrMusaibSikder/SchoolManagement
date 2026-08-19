using CourseHub.Domain.Entities;

namespace CourseHub.Application.Common.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(PasswordResetToken resetToken, CancellationToken cancellationToken = default);
}
