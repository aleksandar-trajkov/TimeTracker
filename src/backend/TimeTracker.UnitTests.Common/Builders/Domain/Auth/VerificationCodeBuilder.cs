using TimeTracker.Domain.Auth;
using Bogus;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class VerificationCodeBuilder : EntityBuilder<VerificationCodeBuilder, VerificationCode>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _code = Faker.Random.AlphaNumeric(Faker.Random.Int(10, 20));
    private bool _isUsed = false;
    private DateTime _expiresAt = DateTime.UtcNow.AddHours(24);
    private Guid _userId = Guid.NewGuid();

    protected override VerificationCode Instance => new VerificationCode
    {
        Id = _id,
        Code = _code,
        IsUsed = _isUsed,
        ExpiresAt = _expiresAt,
        UserId = _userId
    };

    public VerificationCodeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public VerificationCodeBuilder WithCode(string code)
    {
        _code = code;
        return this;
    }

    public VerificationCodeBuilder WithIsUsed(bool isUsed)
    {
        _isUsed = isUsed;
        return this;
    }

    public VerificationCodeBuilder WithExpiresAt(DateTime expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    public VerificationCodeBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public VerificationCodeBuilder AsUsed()
    {
        _isUsed = true;
        return this;
    }

    public VerificationCodeBuilder AsExpired()
    {
        _expiresAt = DateTime.UtcNow.AddHours(-1);
        return this;
    }

    public VerificationCodeBuilder AsValid()
    {
        _isUsed = false;
        _expiresAt = DateTime.UtcNow.AddHours(24);
        return this;
    }
}