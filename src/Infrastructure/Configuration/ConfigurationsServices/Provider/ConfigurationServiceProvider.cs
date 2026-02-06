using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.ConfigurationsServices.Provider;

public class ConfigurationServiceProvider : ConfigurationProvider
{
    public void UpdateConfiguration(IEnumerable<ConfigurationItemDto> newItems)
    {
        Data.Clear();

        foreach (ConfigurationItemDto item in newItems.Where(x => x.Key != null))
        {
            if (item.Key != null)
                Data[item.Key] = item.Value;
        }

        OnReload();
    }
}
