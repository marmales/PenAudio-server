using ServerPenAudio.Code.Interfaces;

namespace ServerPenAudio.Code
{
    public class ConfigurationProvider : IConfigurationProvider
    {
    
        public string AudioFolderLocation => $"F:\\AudioFiles";
    }
}