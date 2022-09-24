using System.Text.Json;
using Confluent.Kafka;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using nu.example.FirestoreConsumer.Models;
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

using var c = new ConsumerBuilder<Ignore, string>(config)
    .SetErrorHandler((_, e) => Console.WriteLine($"Error occured: {e.Reason}"))
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
        ConsumeResult<Ignore, string> cr = c.Consume(cts.Token);

        nu.example.Shared.Models.BankAccount? bankAccount = JsonSerializer.Deserialize<nu.example.Shared.Models.BankAccount>(cr.Message.Value);

        if (bankAccount == null)
        {
            continue;
        }

        Console.WriteLine($"Processing update for bankaccount with id: {bankAccount.Id.ToString()}");

        BankAccount internalBankAccount = new BankAccount
        {
            UserId = bankAccount.UserId.ToString(),
            AccountNumber = bankAccount.AccountNumber
        };

        await bankAccountCollection
                .Document(bankAccount.Id.ToString())
                .SetAsync(internalBankAccount, SetOptions.MergeAll);
    }
}
catch (OperationCanceledException)
{
    c.Close();
}
