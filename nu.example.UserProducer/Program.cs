using System.Text.Json;
using Confluent.Kafka;
using nu.medapp.Shared.Models;

var config = new ProducerConfig
{
    BootstrapServers = "host.docker.internal:9094",
    SaslMechanism = SaslMechanism.Plain,
};

using (var p = new ProducerBuilder<Null, string>(config).Build())
{
    for (int i = 0; i < 100; ++i)
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe " + i
        };

        string userJson = JsonSerializer.Serialize(user);

        Console.WriteLine($"Producing message: {userJson}");

        p.Produce("kafka-csharp-example", new Message<Null, string> { Value = userJson });
        Thread.Sleep(100);
    }

    // wait for up to 10 seconds for any inflight messages to be delivered.
    p.Flush(TimeSpan.FromSeconds(10));
}
