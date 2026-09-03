namespace TheSocialNetwork.Infra.Persistence.Data;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; init; }
    public int MinPoolSize { get; init; } = 0;
    public int MaxPoolSize { get; init; } = 50;
    public int CommandTimeoutSeconds { get; init; } = 30;
}
