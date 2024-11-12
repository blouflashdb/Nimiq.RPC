using System.Text.Json.Serialization;

namespace DemoApp;

public class Block
{
    [JsonPropertyName("hash")]
    public required string Hash { get; set; }

    [JsonPropertyName("number")]
    public required long Number { get; set; }

    [JsonPropertyName("bodyHash")]
    public required string BodyHash { get; set; }
};
