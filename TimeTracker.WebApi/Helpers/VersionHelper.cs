using System.Globalization;
using System.Reflection;

namespace RealEstate.Web.Infrastructure.Helpers;

public static class VersionHelper
{
    public static DateTime GetLinkerTime()
    {
        const string BuildVersionMetadataPrefix = "+build";

        var attribute = Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }
        }

        return default;
    }
}