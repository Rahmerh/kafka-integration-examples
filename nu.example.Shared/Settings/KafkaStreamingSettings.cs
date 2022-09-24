namespace nu.example.Shared.Settings;

public class KafkaStreamingSettings
{
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public string? InputTopic { get; set; }
    public string? OutputTopic { get; set; }
    public string? SchemaRegistryUrl { get; set; }
}
