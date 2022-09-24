using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
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

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = kafkaSettings.SchemaRegistryUrl
};

var jsonSerializerConfig = new JsonSerializerConfig
{
    BufferBytes = 100
};

using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

using var bankAccountProducer = new ProducerBuilder<Null, BankAccount>(config)
    .SetValueSerializer(new JsonSerializer<BankAccount>(schemaRegistry, jsonSerializerConfig).AsSyncOverAsync())
    .Build();

using var userProducer = new ProducerBuilder<Null, User>(config)
    .SetValueSerializer(new JsonSerializer<User>(schemaRegistry, jsonSerializerConfig).AsSyncOverAsync())
    .Build();

Console.WriteLine("What message to you want to produce?\n1. User message to 'users'.\n2. BankAccount message to 'bank-accounts'.");

while (true)
{
    Console.Write("Choice: ");
    string? choice = Console.ReadLine();

    if (string.IsNullOrEmpty(choice))
    {
        Console.WriteLine("Please provice a choice.");
        continue;
    }

    if (!int.TryParse(choice, out int choiceParsed))
    {
        Console.WriteLine("Invalid input.");
        continue;
    }

    if (choiceParsed == 1)
    {
        User user = new User
        {
            Id = Guid.Parse("116e25bc-862e-4d94-961f-16d0308aeaea"),
            FirstName = "John",
            LastName = "Doe"
        };

        DateTime now = DateTime.Now;

        Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message to topic: 'users'");

        userProducer.Produce("users", new Message<Null, User> { Value = user });
    }
    else if (choiceParsed == 2)
    {
        BankAccount bankAccount = new BankAccount
        {
            Id = Guid.Parse("216e25bc-862e-4d94-961f-16d0308aeaea"),
            UserId = Guid.Parse("116e25bc-862e-4d94-961f-16d0308aeaea"),
            AccountNumber = "Account number 1"
        };

        DateTime now = DateTime.Now;

        Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message to topic: 'bank-accounts'");

        bankAccountProducer.Produce("bank-accounts", new Message<Null, BankAccount> { Value = bankAccount });
    }
    else
    {
        Console.WriteLine($"Unknown option: {choiceParsed}");
    }

    bankAccountProducer.Flush(TimeSpan.FromSeconds(10));
}
