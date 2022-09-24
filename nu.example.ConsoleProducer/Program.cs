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

        string userJson = JsonSerializer.Serialize(user);

        DateTime now = DateTime.Now;

        Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message: {userJson}, to topic: 'users'");

        p.Produce("users", new Message<Null, string> { Value = userJson });
    }
    else if (choiceParsed == 2)
    {
        BankAccount bankAccount = new BankAccount
        {
            Id = Guid.Parse("216e25bc-862e-4d94-961f-16d0308aeaea"),
            UserId = Guid.Parse("116e25bc-862e-4d94-961f-16d0308aeaea"),
            AccountNumber = "Account number 1"
        };

        string bankAccountJson = JsonSerializer.Serialize(bankAccount);

        DateTime now = DateTime.Now;

        Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message: {bankAccountJson} to topic: 'bank-accounts'");

        p.Produce("bank-accounts", new Message<Null, string> { Value = bankAccountJson });
    }
    else
    {
        Console.WriteLine($"Unknown option: {choiceParsed}");
    }

    p.Flush(TimeSpan.FromSeconds(10));
}
