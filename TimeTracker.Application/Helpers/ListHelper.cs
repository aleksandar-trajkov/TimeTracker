namespace TimeTracker.Application.Helpers;

public static class ListHelper
{
    public static List<T> CreateList<T>(params T[] items)
    {
        return items.ToList();
    }

    public static List<T> CreateEmptyList<T>()
    {
        return new List<T>();
    }
}
