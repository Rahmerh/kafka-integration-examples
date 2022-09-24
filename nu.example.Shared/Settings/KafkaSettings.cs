namespace nu.example.Shared.Settings;

public class KafkaSettings
{
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public string? Topic { get; set; }
    public string? SchemaRegistryUrl { get; set; }
}
