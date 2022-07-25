using Newtonsoft.Json;
using System;

namespace BCL_OffsetGenerator
{
    public class MannifestInfo
    {
        public DateTime? Date { get; set; }
        public ulong ManifestId { get; set; }
        public ulong InnerNetClient { get; set; }
        public int? BroadcastVersion { get; set; }
        public string Version { get; set; }
        public bool Obfucated { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool x64 { get; set; }

        [JsonIgnore]
        public string VersionWithoutGame
        {
            get
            {
                return (Version.EndsWith('s') || Version.EndsWith('m')) ? Version.Substring(0, Version.Length - 1) : Version;
            }
        }
    }
}
