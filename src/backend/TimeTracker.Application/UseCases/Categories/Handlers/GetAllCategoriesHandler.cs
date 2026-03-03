using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesHandler.Query, List<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<List<Category>> Handle(Query request, CancellationToken cancellationToken)
    {
        return (await _categoryRepository.GetAllAsync(request.OrganizationId, cancellationToken)).ToList();
    }
    public record Query : Cacheable, IRequest<List<Category>>
    {
        public Guid OrganizationId { get; private set; }

        public Query(Guid organizationId)
        {
            OrganizationId = organizationId;
            CacheKeyPrefix = CachingKeys.Categories;
        }

        public override string GetCacheKey()
        {
            return $"{OrganizationId}";
        }
    }
}