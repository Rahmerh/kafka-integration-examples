using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using nu.example.Shared.Models;
using nu.example.Shared.Settings;

// Firestore setup
FirestoreDb db = new FirestoreDbBuilder
{
    ProjectId = "kafka-examples",
    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
}.Build();

CollectionReference usersCollection = db.Collection("Users");

// Kafka setup
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

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = kafkaSettings.SchemaRegistryUrl
};

var jsonSerializerConfig = new JsonSerializerConfig
{
    BufferBytes = 100
};

using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

using var kafkaProducer = new ProducerBuilder<Null, User>(config)
    .SetValueSerializer(new JsonSerializer<User>(schemaRegistry, jsonSerializerConfig).AsSyncOverAsync())
    .Build();

FirestoreChangeListener listener = usersCollection.Listen(snapshot =>
    {
        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            nu.example.FirestoreProducer.Models.User user = document.ConvertTo<nu.example.FirestoreProducer.Models.User>();

            User internalUser = new User
            {
                Id = Guid.Parse(document.Id),
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            Console.WriteLine($"Received change for user with id: {internalUser.Id}");

            kafkaProducer.Produce("users", new Message<Null, User> { Value = internalUser });
        }
    });

// Handle CTRL+C (SIGINT)
Console.CancelKeyPress += (sender, e) =>
{
    // The current process should resume when the event handler concludes
    e.Cancel = true;
    listener.StopAsync();
};
await listener.ListenerTask;
