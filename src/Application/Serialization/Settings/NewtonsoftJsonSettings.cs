
using Uni.Scan.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace Uni.Scan.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}