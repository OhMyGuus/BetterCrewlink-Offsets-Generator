using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator
{
    internal interface IGameDownloadSource : IDisposable
    {
        Task DownloadManifests(List<MannifestInfo> manifests, bool skipDownload = false);
        Task<List<MannifestInfo>> FetchManifests();
        static Boolean Enabled() => false;

        public static IEnumerable<Type> DownloadSources => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IGameDownloadSource).IsAssignableFrom(p) && !p.IsInterface);
    }
}