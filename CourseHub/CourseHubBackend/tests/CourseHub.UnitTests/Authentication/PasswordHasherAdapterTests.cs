using CourseHub.Infrastructure.Authentication;

namespace CourseHub.UnitTests.Authentication;

public class PasswordHasherAdapterTests
{
    private readonly PasswordHasherAdapter _sut = new();

    [Fact]
    public void HashPassword_DoesNotReturnPlainText()
    {
        const string password = "Sup3rSecret!";

        var hash = _sut.HashPassword(password);

        Assert.NotEqual(password, hash);
        Assert.False(string.IsNullOrWhiteSpace(hash));
    }

    [Fact]
    public void HashPassword_ProducesDifferentHashesForSamePassword()
    {
        const string password = "Sup3rSecret!";

        var hash1 = _sut.HashPassword(password);
        var hash2 = _sut.HashPassword(password);

        // PBKDF2 with a random salt per hash — same input, different output.
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
    {
        const string password = "Sup3rSecret!";
        var hash = _sut.HashPassword(password);

        var result = _sut.VerifyPassword(hash, password);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_ForWrongPassword()
    {
        const string password = "Sup3rSecret!";
        var hash = _sut.HashPassword(password);

        var result = _sut.VerifyPassword(hash, "WrongPassword!");

        Assert.False(result);
    }
}
