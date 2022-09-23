namespace nu.example.Shared.Settings;

public class CacheSettings
{
    public CacheSettings()
    {
        HostName = "";
        Password = "";
    }

    public string HostName { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
}
