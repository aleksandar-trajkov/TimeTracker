using TimeTracker.Domain.Auth;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class UserBuilder : EntityBuilder<UserBuilder, User>
{
    private Guid _id = Guid.NewGuid();
    private string _firstName = Random.Shared.GenerateString(5, 20);
    private string _lastName = Random.Shared.GenerateString(10, 30);
    private string _email = Random.Shared.GenerateEmail(10);
    private string _passwordHash = Random.Shared.GenerateString(512);
    private bool _isActive = true;
    private IEnumerable<Permission> _permissions = new List<Permission>();

    protected override User Instance => new User
    {
        Id = _id,
        FirstName = _firstName,
        LastName = _lastName,
        Email = _email,
        PasswordHash = _passwordHash,
        IsActive = _isActive,
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

    public UserBuilder WithPermissions(IEnumerable<Permission> permissions)
    {
        _permissions = permissions;
        return this;
    }
}