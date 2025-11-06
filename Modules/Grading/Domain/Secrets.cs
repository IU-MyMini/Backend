namespace GradingModule.Domain;

public class Secrets
{
    public string          ProxyUrl   { get; set; } = default!;
    public string          ServiceKey { get; set; } = default!;
    public DatabaseSecrets Database   { get; set; } = default!;

    public class DatabaseSecrets
    {
        public string ConnectionString { get; set; } = default!;
    }
}