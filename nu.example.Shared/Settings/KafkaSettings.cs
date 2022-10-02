namespace nu.example.Shared.Settings;

public class KafkaSettings
{
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public string? Topic { get; set; }
    public string? SchemaRegistryUrl { get; set; }
    public string? KafkaUsername {get;set;}
    public string? KafkaPassword {get;set;}
}
