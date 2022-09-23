using Confluent.Kafka;

public class Program
{
    private static void Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "host.docker.internal:9094",
            SaslMechanism = SaslMechanism.Plain,
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var c = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            c.Subscribe("");

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
                    try
                    {
                        var cr = c.Consume(cts.Token);
                        Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Close and Release all the resources held by this consumer  
                c.Close();
            }
        }
    }
}
