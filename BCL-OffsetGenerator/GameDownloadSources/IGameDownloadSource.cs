﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator
{
    internal interface IGameDownloadSource
    {
        Task DownloadManifests(List<MannifestInfo> manifests, bool skipDownload = false);
        Task<List<MannifestInfo>> FetchManifests();
    }
}