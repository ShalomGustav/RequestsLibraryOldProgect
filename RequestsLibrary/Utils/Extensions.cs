using Newtonsoft.Json;
using RestSharp.Extensions;

namespace RequestsLibrary.Utils
{
    internal static class SerializationExtensions
    {
        internal static T DeserializeTo<T>(this string self) where T : class
        {
            return !self.HasValue() ? null : JsonConvert.DeserializeObject<T>(self);
        }
    }
}
