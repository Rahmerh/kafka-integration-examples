using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using nu.example.Shared.Models;
using Confluent.Kafka.SyncOverAsync;
using nu.example.Shared.Settings;

// Firestore setup
FirestoreDb db = new FirestoreDbBuilder
{
    ProjectId = "kafka-examples",
    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
}.Build();

CollectionReference bankAccountCollection = db.Collection("BankAccounts");

// Kafka setup
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

using var c = new ConsumerBuilder<Ignore, BankAccount>(config)
    .SetErrorHandler((_, e) => Console.WriteLine($"Error occured: {e.Reason}"))
    .SetValueDeserializer(new JsonDeserializer<BankAccount>().AsSyncOverAsync())
    .Build();

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
        ConsumeResult<Ignore, BankAccount> cr = c.Consume(cts.Token);

        BankAccount bankAccount = cr.Message.Value;

        Console.WriteLine($"Processing update for bankaccount with id: {bankAccount.Id.ToString()}");

        nu.example.FirestoreConsumer.Models.BankAccount externalBankAccount = new nu.example.FirestoreConsumer.Models.BankAccount
        {
            UserId = bankAccount.UserId.ToString(),
            AccountNumber = bankAccount.AccountNumber
        };

        await bankAccountCollection
                .Document(bankAccount.Id.ToString())
                .SetAsync(externalBankAccount, SetOptions.MergeAll);
    }
}
catch (OperationCanceledException)
{
    c.Close();
}
