using System.Text.Json;

namespace RERPAPI.Middleware
{
    public static class SessionHelper
    {
        public static void SetObject<T>(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session,string key)
        {
            var value = session.GetString(key);
            return value == null ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(value);
        }
    }
}
