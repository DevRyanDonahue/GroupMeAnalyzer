using System.IO;
using Newtonsoft.Json;

namespace GroupMeAnalysis.Util {
    //https://stackoverflow.com/questions/13297563/read-and-parse-a-json-file-in-c-sharp
    class JsonUtil {
        public static T LoadJson<T>(string path) {
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                T item = JsonConvert.DeserializeObject<T>(json);
                return item;
            }

        }
    }
}