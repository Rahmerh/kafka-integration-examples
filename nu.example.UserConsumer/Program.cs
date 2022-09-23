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

var config = new ConsumerConfig
{
    BootstrapServers = kafkaSettings.BootstrapServers,
    SaslMechanism = SaslMechanism.Plain,
    GroupId = kafkaSettings.GroupId,
};

using var c = new ConsumerBuilder<Ignore, string>(config).Build();

c.Subscribe(kafkaSettings.Topic);

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

try
{
    while (true)
    {
        try
        {
            ConsumeResult<Ignore, string> cr = c.Consume(cts.Token);

            DateTime now = DateTime.Now;

            User? user = JsonSerializer.Deserialize<User>(cr.Message.Value);

            Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Consumed user with firstname: {user?.FirstName} and lastname: {user?.LastName}");
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occured: {e.Error.Reason}");
        }
    }
}
catch (OperationCanceledException)
{
    c.Close();
}
