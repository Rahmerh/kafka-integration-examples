namespace nu.example.DuplicateMessageFilter.Services;

using StackExchange.Redis;

public class RedisService
{
    public RedisService(string hostName, int port, string password)
    {
        var options = new ConfigurationOptions()
        {
            KeepAlive = 0,
            AllowAdmin = true,
            EndPoints = { { hostName, port } },
            ConnectTimeout = 5000,
            ConnectRetry = 5,
            SyncTimeout = 5000,
            AbortOnConnectFail = false,
            Password = password
        };

        lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(options);
        });
    }

    private Lazy<ConnectionMultiplexer> lazyConnection;

    public ConnectionMultiplexer Connection
    {
        get
        {
            return lazyConnection.Value;
        }
    }

    public string? getStringByValue(string key)
    {
        var database = Connection.GetDatabase();

        return database.StringGet(key);
    }

    public void putStringByValue(string key, string value)
    {
        var database = Connection.GetDatabase();

        database.StringSet(key, value);
    }
}
