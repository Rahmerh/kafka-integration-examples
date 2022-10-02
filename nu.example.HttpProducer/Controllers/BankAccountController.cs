using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using nu.example.Shared.Models;
using nu.example.Shared.Settings;

namespace nu.example.HttpProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountController : ControllerBase, IDisposable
{
    private readonly IOptions<KafkaSettings> _settings;
    private readonly IProducer<Null, BankAccount> _producer;

    public BankAccountController(IOptions<KafkaSettings> settings, IProducer<Null, BankAccount> producer)
    {
        _settings = settings;
        _producer = producer;
    }

    [HttpPost]
    public void PostBankAccountMessage(BankAccount bankAccount)
    {
        DateTime now = DateTime.Now;

        Console.WriteLine($"{now.ToString("dd-MM-yyyy HH:mm:ss")} - Producing message to topic: '{_settings.Value.Topic}'");

        _producer.Produce(_settings.Value.Topic, new Message<Null, BankAccount> { Value = bankAccount });

        _producer.Flush();
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
