namespace TimeTracker.Application.Helpers;

public static class ListHelper
{
    public static IEnumerable<T> CreateList<T>(params T[] items)
    {
        return items.ToList();
    }

    public static IEnumerable<T> CreateEmptyList<T>()
    {
        return new List<T>();
    }
}
