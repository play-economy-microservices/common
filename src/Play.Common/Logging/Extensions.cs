using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Play.Common.Settings;

namespace Play.Common.Logging;

public static class Extensions
{
    /// <summary>
    /// This adds Seq logging for your system locally. 
    /// </summary>
    public static IServiceCollection AddSeqLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(loggingBuilder =>
        {
            var seqSettings = configuration.GetSection(nameof(SeqSettings)).Get<SeqSettings>();
            loggingBuilder.AddSeq(serverUrl: seqSettings.ServerUrl);
        });
        
        return services;
    }
}