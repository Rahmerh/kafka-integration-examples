using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using nu.example.Shared.Models;
using nu.example.Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));

builder.Services.AddSingleton<ISerializer<BankAccount>>(x =>
{
    var schemaRegistry = x.GetRequiredService<CachedSchemaRegistryClient>();

    var jsonSerializerConfig = new JsonSerializerConfig
    {
        BufferBytes = 100
    };

    return new JsonSerializer<BankAccount>(schemaRegistry, jsonSerializerConfig).AsSyncOverAsync();
});

builder.Services.AddScoped<CachedSchemaRegistryClient>(x =>
{
    var settings = x.GetRequiredService<IOptions<KafkaSettings>>();

    var schemaRegistryConfig = new SchemaRegistryConfig
    {
        Url = settings.Value.SchemaRegistryUrl
    };

    return new CachedSchemaRegistryClient(schemaRegistryConfig);
});

builder.Services.AddScoped<IProducer<Null, BankAccount>>(x =>
{
    var settings = x.GetRequiredService<IOptions<KafkaSettings>>();

    var config = new ProducerConfig
    {
        BootstrapServers = settings.Value.BootstrapServers,
        SaslMechanism = SaslMechanism.Plain,
        SecurityProtocol = SecurityProtocol.SaslPlaintext,
        SaslUsername = settings.Value.KafkaUsername,
        SaslPassword = settings.Value.KafkaPassword,

    };

    ISerializer<BankAccount> jsonSerializer = x.GetRequiredService<ISerializer<BankAccount>>();

    var producer = new ProducerBuilder<Null, BankAccount>(config)
            .SetValueSerializer(jsonSerializer)
            .Build();

    return producer;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
