using System.Text.Json;
using log4net.Appender;

namespace IntroSE.Kanban.Backend.BusinessLayer;

internal static class JsonSerializerExtention // this is a wrapper for JsonSerializer so we can change the options once and reduce double code.
{
    private static JsonSerializerOptions defaultSerializerSettings = new JsonSerializerOptions(); // will be used to access options.

    public static JsonSerializerOptions DefaultSerializerSettings
    {
        get => defaultSerializerSettings;
    }
    public static string Serialize<T>(T toSerialize) // wrapper function for JsonSerializer.Serialize
    {
        return JsonSerializer.Serialize(toSerialize, defaultSerializerSettings);
    }
}