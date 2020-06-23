using MSensis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Languages
{
    public class en_GR
    {
        public static List<Resource> GetList()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;        
            return JsonConvert.DeserializeObject<List<Resource>>(File.ReadAllText("Languages/en-GR.json"), jsonSerializerSettings);
        }
    }
}
