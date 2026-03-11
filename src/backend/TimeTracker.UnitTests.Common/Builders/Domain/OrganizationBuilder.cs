using TimeTracker.Domain;
using Bogus;
using TimeTracker.Domain.Auth;

namespace TimeTracker.UnitTests.Common.Builders.Domain;

public class OrganizationBuilder : EntityBuilder<OrganizationBuilder, Organization>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _name = Faker.Company.CompanyName();
    private string? _description = Faker.Company.CatchPhrase();
    private IEnumerable<User> _users = new List<User>();
    private IEnumerable<Category> _categories = new List<Category>();

    protected override Organization Instance => new Organization
    {
        Id = _id,
        Name = _name,
        Description = _description,
        Users = _users.ToList(),
        Categories = _categories.ToList(),
    };

    public OrganizationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OrganizationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OrganizationBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public OrganizationBuilder WithoutDescription()
    {
        _description = null;
        return this;
    }

    public OrganizationBuilder WithUsers(params IEnumerable<User> users)
    {
        _users = users;
        return this;
    }

    public OrganizationBuilder WithCategories(params IEnumerable<Category> categories)
    {
        _categories = categories;
        return this;
    }
}