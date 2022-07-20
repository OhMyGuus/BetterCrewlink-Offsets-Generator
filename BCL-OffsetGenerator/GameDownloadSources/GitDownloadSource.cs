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
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        public async Task<List<MannifestInfo>> FetchManifests()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }
    }
}