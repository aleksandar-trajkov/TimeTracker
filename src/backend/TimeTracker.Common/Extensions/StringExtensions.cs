namespace TimeTracker.Common.Extensions;

public static class StringExtensions
{
    public static string TrimStart(this string target, params string[] trimStrings)
    {
        string result = target;
        bool changed;
        do
        {
            changed = false;
            foreach (var trimString in trimStrings)
            {
                if (string.IsNullOrEmpty(trimString)) continue;
                while (result.StartsWith(trimString))
                {
                    result = result.Substring(trimString.Length);
                    changed = true;
                }
            }
        } while (changed);
        return result;
    }

    public static string TrimEnd(this string target, params string[] trimStrings)
    {
        string result = target;
        bool changed;
        do
        {
            changed = false;
            foreach (var trimString in trimStrings)
            {
                if (string.IsNullOrEmpty(trimString)) continue;
                while (result.EndsWith(trimString))
                {
                    result = result.Substring(0, result.Length - trimString.Length);
                    changed = true;
                }
            }
        } while (changed);
        return result;
    }
}
