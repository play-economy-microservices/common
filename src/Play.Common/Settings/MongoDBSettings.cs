namespace Play.Common.Settings;

public class MongoDbSettings
{
    private string connectionString;

    public string Host { get; init; }

    public int Port { get; init; }

    /// <summary>
    /// CosmosDB and MongoDB connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return string.IsNullOrWhiteSpace(connectionString)
                ? $"mongodb://{Host}:{Port}" : connectionString;
        }
        init { connectionString = value; }
    }
}
