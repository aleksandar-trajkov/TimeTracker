namespace TimeTracker.UnitTests.Common.Extensions;

public static class RandomExtensions
{
    public static string GenerateString(this Random random, int length = 10)
    {
        return random.GenerateString(length, length);
    }

    public static string GenerateString(this Random random, int minLength, int maxLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, random.Next(minLength, maxLength))
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateEmail(this Random random, int length = 10)
    {
        return $"{random.GenerateString(length)}@{random.GenerateString(10)}.{random.GenerateString(3)}";
    }
}
