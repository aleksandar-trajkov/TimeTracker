namespace TimeTracker.UnitTests.Common.Utilities;

public static class RandomStringGenerator
{
    public static string GenerateString(int length)
    {
        return GenerateString(length, length);
    }

    public static string GenerateString(int minLength, int maxLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, random.Next(minLength, maxLength))
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}