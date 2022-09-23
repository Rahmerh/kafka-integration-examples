using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using nu.example.Shared.Models;
using nu.example.Shared.Settings;

var configuration = new ConfigurationBuilder()
 .SetBasePath(Directory.GetCurrentDirectory())
 .AddJsonFile($"appsettings.json")
 .Build();

var kafkaSettings = configuration.GetRequiredSection("Kafka").Get<KafkaSettings>();

var config = new ProducerConfig
{
    BootstrapServers = kafkaSettings.BootstrapServers,
    SaslMechanism = SaslMechanism.Plain,
};

using var p = new ProducerBuilder<Null, string>(config).Build();

User user = new User
{
    Id = Guid.Parse("116e25bc-862e-4d94-961f-16d0308aeaea"),
    FirstName = "John",
    LastName = "Doe"
};

string userJson = JsonSerializer.Serialize(user);

DateTime now = DateTime.Now;

Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message: {userJson}");

p.Produce(kafkaSettings.Topic, new Message<Null, string> { Value = userJson });

// wait for up to 10 seconds for any inflight messages to be delivered.
p.Flush(TimeSpan.FromSeconds(10));
