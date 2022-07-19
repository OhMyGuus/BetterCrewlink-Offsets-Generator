using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator;

public class LookupJson
{
    [JsonProperty("patterns")]
    public LookupPatterns Patterns { get; set; }

    [JsonProperty("versions")]
    public Dictionary<string, LookupVersions> Versions { get; set; }

    public class LookupPatterns
    {
        public BasePatterns x64 { get; set; }
        public BasePatterns x86 { get; set; }
    }

    public class BasePatterns
    {
        public MemoryPattern broadcastVersion { get; set; }
    }

    public class MemoryPattern
    {
        public string sig { get; set; }
        public int patternOffset { get; set; }
        public int addressOffset { get; set; }
    }


    public class Signature
    {
        public string sig { get; set; }
        public int patternOffset { get; set; }
        public int addressOffset { get; set; }
    }

    public class LookupVersions
    {
        public string version { get; set; }
        public string file { get; set; }
        public int offsetsVersion { get; set; }
    }


    public static async Task<LookupJson> GetFrombase()
    {

        var assembly = typeof(Program).GetTypeInfo().Assembly;
        using (Stream stream = assembly.GetManifestResourceStream("BCL_OffsetGenerator.BaseFiles.baselookup.json"))
        using (StreamReader reader = new StreamReader(stream))
            return JsonConvert.DeserializeObject<LookupJson>(await reader.ReadToEndAsync());
    }
}
