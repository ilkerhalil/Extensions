using System.Configuration;

namespace Extensions.ConfigurationManager
{
    public static class ConfigurationManagerExtension
    {
        public static T GetSection<T>(this Configuration configuration, string sectionName)
    where T : ConfigurationSection
        {
            return (T)configuration.GetSection(sectionName);
        }
    }
}
