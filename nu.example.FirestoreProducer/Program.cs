﻿using System.Text.Json;
using Confluent.Kafka;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using nu.example.FirestoreProducer.Models;
using nu.example.Shared.Settings;

// Firestore setup
FirestoreDb db = new FirestoreDbBuilder
{
    ProjectId = "kafka-in-csharp",
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

using var kafkaProducer = new ProducerBuilder<Null, string>(config).Build();

FirestoreChangeListener listener = usersCollection.Listen(snapshot =>
    {

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            User user = document.ConvertTo<User>();

            Console.WriteLine($"Received change for user with id: {user.Id}");

            nu.example.Shared.Models.User internalUser = new nu.example.Shared.Models.User();
            internalUser.FirstName = user.FirstName;
            internalUser.LastName = user.LastName;

            string userJson = JsonSerializer.Serialize(internalUser);

            kafkaProducer.Produce("kafka-csharp-example", new Message<Null, string> { Value = userJson });
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