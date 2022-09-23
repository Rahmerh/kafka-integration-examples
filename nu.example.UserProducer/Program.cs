using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using nu.medapp.Shared.Models;
using nu.medapp.Shared.Settings;

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

for (int i = 0; i < 100; ++i)
{
    User user = new User
    {
        Id = Guid.NewGuid(),
        FirstName = "John",
        LastName = "Doe " + i
    };

    string userJson = JsonSerializer.Serialize(user);

    DateTime now = DateTime.Now;

    Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message: {userJson}");

    p.Produce("kafka-csharp-example", new Message<Null, string> { Value = userJson });
    Thread.Sleep(100);
}

// wait for up to 10 seconds for any inflight messages to be delivered.
p.Flush(TimeSpan.FromSeconds(10));
