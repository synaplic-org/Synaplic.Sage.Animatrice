using System.Text.Json;
using Uni.Scan.Application.Interfaces.Serialization.Options;

namespace Uni.Scan.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}