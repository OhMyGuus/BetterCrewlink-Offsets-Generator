using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BCL_OffsetGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var gameSource =
                   new SteamGameSource(JsonConvert.DeserializeObject<SteamAccount>(File.ReadAllText("SteamAccount.json"))))
            {
                await gameSource.DownloadManifests(await gameSource.FetchManifests());
            }

            Console.WriteLine("Done");
        }
    }
}