using System.Collections.Generic;

namespace DiscordBot
{
    public interface IConfigIo
    {
        void Save(Dictionary<string, object> dictionary, string configName);
        Dictionary<string, object> Load(string configName);
    }
}