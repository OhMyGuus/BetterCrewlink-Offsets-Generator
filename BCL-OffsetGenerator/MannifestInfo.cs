using System;

namespace BCL_OffsetGenerator
{
    public class MannifestInfo
    {
        public DateTime Date { get; set; }
        public ulong ManifestId { get; set; }
        public ulong InnerNetClient { get; set; }
        public int? BroadcastVersion { get; set; }
        public string Version { get; set; }
        public bool Obfucated { get; set; }
        public bool x64 { get; set; }
    }
}
