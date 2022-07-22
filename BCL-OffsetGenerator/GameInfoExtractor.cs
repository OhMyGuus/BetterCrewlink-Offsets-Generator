using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator;
public class GameInfoExtractor
{
    public void FillManifests(List<MannifestInfo> manifests)
    {
        Parallel.ForEach(manifests, new ParallelOptions
        {
            MaxDegreeOfParallelism = 20
        }, manifest => { FillManifest(manifest, true); });
    }

    private void FillManifest(MannifestInfo manifest, bool alwaysGenerate = false)
    {
        var obfucated = manifest.Obfucated;
        manifest.x64 = isX64(manifest);
        manifest.BroadcastVersion = alwaysGenerate || manifest.BroadcastVersion == null || manifest.BroadcastVersion == 0
            ? GetBroadCastVersion(manifest)
            : manifest.BroadcastVersion;
        manifest.InnerNetClient = alwaysGenerate || manifest.InnerNetClient == 0
            ? GetInnerNetClient(manifest, out obfucated)
            : manifest.InnerNetClient;
        manifest.Obfucated = obfucated;
        manifest.Version = alwaysGenerate || string.IsNullOrEmpty(manifest.Version) ? GetGameVersion(manifest) : manifest.Version;
    }


    private bool isX64(MannifestInfo manifest)
    {
        var gameAssamblydll = $"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/GameAssembly.dll";
        if (!File.Exists(gameAssamblydll))
        {
            throw new Exception($"No dll founnd for {manifest.ManifestId}");
        }

        using FileStream stream = new FileStream(gameAssamblydll, FileMode.Open);
        var buffer = new byte[5];
        stream.Position = 0x3C;
        stream.Read(buffer, 0, 4);
        stream.Position = BitConverter.ToInt32(buffer) + 0x18;
        stream.Read(buffer, 0, 2);
        var magicValue = BitConverter.ToUInt16(buffer);
        return magicValue == 0x20b;
    }

    private int? GetBroadCastVersion(MannifestInfo manifest)
    {
        var gameAssamblydll = $"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/GameAssembly.dll";
        if (!File.Exists(gameAssamblydll))
        {
            throw new Exception($"No dll founnd for {manifest.ManifestId}");
        }

        var filebytes = File.ReadAllBytes(gameAssamblydll);
        var sig = FindSignature(filebytes,
            manifest.x64 ? "33 D2 B9 ? ? ? ? 48 83 C4 ? E9 ? ? ? ?" : "6A 00 68 ? ? ? ? E8 ? ? ? ? 83 C4 08 C3");

        if (sig == null)
            return null;

        var nr = BitConverter.ToInt32(filebytes, sig.Value + 3);

        return nr;
    }

    private int? FindSignature(byte[] bytes, string signature)
    {
        var nrs = signature.Split(' ').Select(o => o == "?" ? (byte?) null : byte.Parse(o, NumberStyles.HexNumber)).ToArray();
        for (int i = 0; i < bytes.Length; i++)
        {
            var tmpi = i;
            for (int k = 0; k < nrs.Length; k++)
            {
                var curbyte = bytes[tmpi++];

                if (nrs[k] == null)
                    continue;

                if (nrs[k] != curbyte)
                    goto exitloop;
            }

            return i;

            exitloop:

            continue;
        }

        return null;
    }

    private string GetGameVersion(MannifestInfo manifest)
    {
        if (!File.Exists($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/Among Us_Data/globalgamemanagers"))
        {
            throw new Exception($"No dump founnd for {manifest.ManifestId}");
        }

        var bytes = File.ReadAllBytes($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/Among Us_Data/globalgamemanagers");
        var checkBytes = bytes.Skip(0xFF0).Take(1200).ToArray();
        for (int i = 0; i < checkBytes.Length; i++)
        {
            var text = Encoding.UTF8.GetBytes("202");
            var text1 = Encoding.UTF8.GetBytes("201");

            if (checkBytes[i] == text[0] && checkBytes[i + 1] == text[1] &&
                (checkBytes[i + 2] == text[2] || checkBytes[i + 2] == text1[2]))
            {
                var lngthOffset = i - 4;
                var lgth = checkBytes[lngthOffset];

                return Encoding.UTF8.GetString(checkBytes.Skip(i).Take(lgth).ToArray());
            }
        }

        return null;
    }


    private ulong GetInnerNetClient(MannifestInfo manifest, out bool obfucated)
    {
        obfucated = false;
        if (!File.Exists($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/dump/script.json"))
        {
            Console.WriteLine($"No dump found for {manifest.ManifestId}");

            throw new Exception($"No dump found for {manifest.ManifestId}");
        }

        var manifests =
            System.Text.Json.JsonSerializer.Deserialize<ScriptsJson>(
                File.ReadAllText($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/dump/script.json"));
        var k = manifests.ScriptMetadata.Find(o => o.Name == "AmongUsClient_TypeInfo");
        if (k == null)
        {
            var test = File.ReadAllLines($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/dump/dump.cs");
            var gameModeLine = test.Select((value, index) => new { value, index = index + 1 })
                .Where(pair => pair.value.Contains(" GameMode;"))
                .Select(pair => pair.index)
                .FirstOrDefault() - 1;
            var classname = test[gameModeLine - 5].Split(" ")[2];
            var k2 = manifests.ScriptMetadata.Find(o => o.Name == $"{classname}_TypeInfo");

            if (k2 == null)
                throw new Exception($"Offset not found for {manifest.ManifestId}");
            else
            {
                obfucated = true;

                return k2.Address;
            }
        }
        else
        {
            var test = File.ReadAllLines($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/dump/dump.cs");
            var gameModeLine = test.Select((value, index) => new { value, index = index + 1 })
                .Where(pair => pair.value.Contains(" GameMode;"))
                .Select(pair => pair.index)
                .FirstOrDefault() - 1;
            obfucated = !test[gameModeLine].Contains("GameModes");
        }

        //  List<ScriptInfo>
        return k.Address;
    }

    struct ScriptsJson
    {
        public List<ScriptMetadataJson> ScriptMetadata { get; set; }

        internal class ScriptMetadataJson
        {
            public ulong Address { get; set; }
            public string Name { get; set; }
            public string Signature { get; set; }
        }
    }
}