using LiteBus.Queries.Abstractions;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.Categories.Handlers;

public class GetAllCategoriesHandler : IQueryHandler<GetAllCategoriesHandler.Query, List<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<List<Category>> HandleAsync(Query request, CancellationToken cancellationToken)
    {
        return (await _categoryRepository.GetAllAsync(request.OrganizationId, cancellationToken)).ToList();
    }
    public record Query(Guid OrganizationId) : IQuery<List<Category>>, ICacheable
    {
        public string CachePrefix { get; set; } = CachingHelper.CacheKeyPrefixes.Categories;

        public string GetCacheKey()
        {
            return $"{CachePrefix}:{nameof(OrganizationId)}-{OrganizationId}";
        }
    };
}