using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator.GameDownloadSources
{
    internal class GitDownloadSource : IGameDownloadSource
    {
        public async Task DownloadManifests(List<MannifestInfo> manifests, bool skipDownload = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<MannifestInfo>> FetchManifests()
        {
            throw new NotImplementedException();
        }
    }
}
