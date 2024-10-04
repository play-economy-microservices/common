namespace Play.Common.Settings;

public class ServiceSettings
{
    /// <summary>
    /// The service name of the microservice and database.
    /// </summary>
    public string ServiceName { get; init; }

    /// <summary>
    /// The authority that the microservice will demand to generate access tokens from.
    /// </summary>
    public string Authority { get; init; }

    /// <summary>
    /// Used to specified current broker (i.e RabbitMQ or Azure Service Bus)
    /// </summary>
    public string MessageBroker { get; init; }

    /// <summary>
    /// The Key Vault name to retrieve secrets from. 
    /// </summary>
    public string KeyVaultName { get; init; }
}
