using CourseHub.Infrastructure.Authentication;

namespace CourseHub.UnitTests.Authentication;

public class SecureTokenGeneratorAndHasherTests
{
    [Fact]
    public void Generate_ProducesNonEmptyUrlSafeToken()
    {
        var generator = new SecureTokenGenerator();

        var token = generator.Generate();

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.DoesNotContain('+', token);
        Assert.DoesNotContain('/', token);
        Assert.DoesNotContain('=', token);
    }

    [Fact]
    public void Generate_ProducesDifferentTokensEachCall()
    {
        var generator = new SecureTokenGenerator();

        var token1 = generator.Generate();
        var token2 = generator.Generate();

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void Hash_IsDeterministic_ForSameInput()
    {
        var hasher = new Sha256TokenHasher();
        const string rawToken = "some-raw-token-value";

        var hash1 = hasher.Hash(rawToken);
        var hash2 = hasher.Hash(rawToken);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Hash_DoesNotReturnTheRawToken()
    {
        var hasher = new Sha256TokenHasher();
        const string rawToken = "some-raw-token-value";

        var hash = hasher.Hash(rawToken);

        Assert.NotEqual(rawToken, hash);
    }
}
