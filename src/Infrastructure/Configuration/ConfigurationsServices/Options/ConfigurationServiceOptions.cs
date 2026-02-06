namespace Infrastructure.Configuration.ConfigurationsServices.Options;

public sealed class ConfigurationServiceOptions
{
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMinutes(1);
}
