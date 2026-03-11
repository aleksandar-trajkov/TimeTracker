using TimeTracker.Domain.Auth;
using Bogus;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class UserBuilder : EntityBuilder<UserBuilder, User>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _firstName = Faker.Name.FirstName();
    private string _lastName = Faker.Name.LastName();
    private string _email = Faker.Internet.Email();
    private string _passwordHash = Faker.Random.AlphaNumeric(512);
    private bool _isActive = true;
    private Guid _organizationId = Guid.NewGuid();
    private IEnumerable<Permission> _permissions = new List<Permission>();

    protected override User Instance => new User
    {
        Id = _id,
        FirstName = _firstName,
        LastName = _lastName,
        Email = _email,
        PasswordHash = _passwordHash,
        IsActive = _isActive,
        OrganizationId = _organizationId,
        Permissions = _permissions.ToList()
    };

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public UserBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    public UserBuilder WithPermissions(IEnumerable<Permission> permissions)
    {
        _permissions = permissions;
        return this;
    }
}