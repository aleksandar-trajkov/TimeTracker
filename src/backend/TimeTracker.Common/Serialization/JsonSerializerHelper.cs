using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeTracker.Common.Serialization;

public static class JsonSerializerHelper
{
    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
        }
    }
}
