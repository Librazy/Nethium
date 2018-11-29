namespace Nethium.Configuration
{
    public class ConfigurationOptions
    {
        public string? Watch { get; set; }

        public string Prefix { get; set; } = "nethium";

        public string? Separator { get; set; }

        public string? ConfigurationPrefix { get; set; }

        public bool AutoReload { get; set; }
    }
}