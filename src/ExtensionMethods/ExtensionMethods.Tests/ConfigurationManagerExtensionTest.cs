using System.Configuration;
using Extensions.ConfigurationManager;
using Shouldly;
using Xunit;

namespace ExtensionMethods.Tests
{
    public class ConfigurationManagerExtensionTest {

        [Fact]
        public void GetSection_Test()
        {
            
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //act
            var samplePortConfigurationSection = configuration.GetSection<SamplePortConfigurationSection>("SampleConfigSection");
            //assert
            samplePortConfigurationSection.ShouldNotBeNull();
            samplePortConfigurationSection.Name.ShouldBe("name");
            samplePortConfigurationSection.Port.ShouldBe(1);
        }
    }

    public class SamplePortConfigurationSection:ConfigurationSection
    {
        [ConfigurationProperty("name",DefaultValue = "name")]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("port",DefaultValue = 1)]
        public int Port
        {
            get { return (int) this["port"]; }
            set { this["port"] = value; }
        }
    }
}