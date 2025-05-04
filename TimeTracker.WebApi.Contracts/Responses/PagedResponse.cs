using TimeTracker.WebApi.Contracts.Responses;

/// <summary>
/// Wrapper for paged results
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResponse<T, TMetadata> where TMetadata : PageResponseMetadata
{
    /// <summary>
    /// The data set in the paged result
    /// </summary>
    public IList<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Contains metadata about the paged result, such as total items, page number, and page size
    /// Can be inherited from <see cref="PageResponseMetadata"/>
    /// </summary>
    public TMetadata Metadata { get; set; } = null!;
}