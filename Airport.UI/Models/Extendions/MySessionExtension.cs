using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Airport.UI.Models.Extendions;

public static class MySessionExtension
{
    public static void MySet(this ISession session, string mykey, object value)
    {
        var hideValue = JsonConvert.SerializeObject(value);
        session.SetString(mykey, hideValue);
    }

    public static T MyGet<T>(this ISession session, string mykey) where T : class
    {
        var sessionValue = session.GetString(mykey);
        return sessionValue == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionValue);
    }
}
