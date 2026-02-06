using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.ConfigurationsServices.Provider;

public class ConfigurationServiceSource : IConfigurationSource
{
    private readonly ConfigurationServiceProvider _provider;

    public ConfigurationServiceSource(ConfigurationServiceProvider provider)
    {
        _provider = provider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}
