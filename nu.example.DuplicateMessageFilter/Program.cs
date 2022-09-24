using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using nu.example.DuplicateMessageFilter.Services;
using nu.example.Shared.Models;
using nu.example.Shared.Settings;

// Kafka setup
var configuration = new ConfigurationBuilder()
 .SetBasePath(Directory.GetCurrentDirectory())
 .AddJsonFile($"appsettings.json")
 .Build();

var kafkaSettings = configuration.GetRequiredSection("Kafka").Get<KafkaStreamingSettings>();

// Redis setup
var cacheSettings = configuration.GetRequiredSection("Cache").Get<CacheSettings>();

var redisService = new RedisService(
        cacheSettings.HostName,
        cacheSettings.Port,
        cacheSettings.Password);

// Kafka consuming setup
var consumerConfig = new ConsumerConfig
{
    BootstrapServers = kafkaSettings.BootstrapServers,
    GroupId = kafkaSettings.GroupId,
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "",
    SaslPassword = ""
};

using var c = new ConsumerBuilder<Ignore, User>(consumerConfig)
    .SetValueDeserializer(new JsonDeserializer<User>().AsSyncOverAsync())
    .Build();

c.Subscribe(kafkaSettings.InputTopic);

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

// Kafka producing setup
var producerConfig = new ProducerConfig
{
    BootstrapServers = kafkaSettings.BootstrapServers,
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "",
    SaslPassword = ""
};

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = kafkaSettings.SchemaRegistryUrl
};

var jsonSerializerConfig = new JsonSerializerConfig
{
    BufferBytes = 100
};

using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

using var producer = new ProducerBuilder<Null, User>(producerConfig)
    .SetErrorHandler((_, e) => Console.WriteLine($"Error occured: {e.Reason}"))
    .SetValueSerializer(new JsonSerializer<User>(schemaRegistry, jsonSerializerConfig).AsSyncOverAsync())
    .Build();

try
{
    while (true)
    {
        bool shouldUpdate = true;

        ConsumeResult<Ignore, User> cr = c.Consume(cts.Token);

        User user = cr.Message.Value;

        String? userHash = redisService.getStringByValue(user.Id.ToString());
        string hashedUser = sha256_hash(JsonSerializer.Serialize(user));

        if (string.IsNullOrEmpty(userHash))
        {
            Console.WriteLine($"Value for id {user.Id} was not found, putting it into redis.");
            redisService.putStringByValue(user.Id.ToString(), hashedUser);
            shouldUpdate = true;
        }
        else if (userHash == hashedUser)
        {
            Console.WriteLine($"Skipping update.");
            shouldUpdate = false;
        }
        else
        {
            Console.WriteLine($"Value for id {user.Id} was different, updating.");
            redisService.putStringByValue(user.Id.ToString(), hashedUser);
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            Console.WriteLine($"Sent message to: {kafkaSettings.OutputTopic}");
            producer.Produce(kafkaSettings.OutputTopic, new Message<Null, User> { Value = cr.Message.Value });
        }
    }
}
catch (OperationCanceledException)
{
    c.Close();
}

static String sha256_hash(String value)
{
    StringBuilder Sb = new StringBuilder();

    using (SHA256 hash = SHA256.Create())
    {
        Encoding enc = Encoding.UTF8;
        Byte[] result = hash.ComputeHash(enc.GetBytes(value));

        foreach (Byte b in result)
            Sb.Append(b.ToString("x2"));
    }

    return Sb.ToString();
}
