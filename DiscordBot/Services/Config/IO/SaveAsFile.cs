using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class SaveAsFile : IConfigIo
    {
        public void Save(Dictionary<string, object> dictionary, string configName)
        {
            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented,
                                                      new JsonSerializerSettings
                                                      {
                                                          PreserveReferencesHandling = PreserveReferencesHandling.All,
                                                          TypeNameHandling = TypeNameHandling.All,
                                                      });
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), configName), json);
        }

        public Dictionary<string, object> Load(string configName)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), configName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json,
                                                                                 new JsonSerializerSettings
                                                                                 {
                                                                                     PreserveReferencesHandling =
                                                                                         PreserveReferencesHandling.All,
                                                                                     TypeNameHandling =
                                                                                         TypeNameHandling.All,
                                                                                 });
            }

            return new Dictionary<string, object>();
        }
    }
}