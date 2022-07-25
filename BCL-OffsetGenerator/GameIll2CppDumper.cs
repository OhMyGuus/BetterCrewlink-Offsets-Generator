using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator
{
    public class GameIll2CppDumper
    {
        public void DumpGameFiles(List<MannifestInfo> manifests)
        {
            foreach (var manifest in manifests)
            {
                CallIll2CppDumper(manifest);
            }
            // Parallel.ForEach(manifests, new ParallelOptions
            // {
            //     MaxDegreeOfParallelism = 2
            // }, (manifest) => { CallIll2CppDumper(manifest); });
        }

        private bool DumpedFiles(string folder) => Directory.Exists($"{folder}/dump") && File.Exists($"{folder}/dump/dump.cs") &&
                                                   File.Exists($"{folder}/dump/script.json");

        public void CallIll2CppDumper(MannifestInfo manifest)
        {
            var folder = $"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}";

            if ((!Directory.Exists(folder)
                 && !File.Exists($"{folder}/GameAssembly.dll")
                 && !File.Exists($"{folder}/Among Us_Data/globalgamemanagers")
                 && !File.Exists($"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat")
                ) || DumpedFiles(folder))
                return;

            Directory.CreateDirectory($"{folder}/dump/");

            ill2cppdumperHelper.Dump($"{folder}/GameAssembly.dll", $"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat",
                $"{folder}/dump/");
            // var process = new Process
            // {
            //     StartInfo = new ProcessStartInfo
            //     {
            //         FileName = @"ill2cppdumper\Il2CppDumper.exe",
            //         Arguments = String.Join(" ", new string[]
            //         {
            //             $"{folder}/GameAssembly.dll",
            //             $"\"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat\"",
            //             $"{folder}/dump/"
            //         }),
            //         UseShellExecute = false,
            //         RedirectStandardOutput = true,
            //         RedirectStandardInput = true,
            //
            //         CreateNoWindow = false,
            //     }
            // };
            // process.Start();
            // while (!process.StandardOutput.EndOfStream)
            // {
            //     string line = process.StandardOutput.ReadLine();
            //     Console.WriteLine(line);
            //     // do something with line
            //     if (line.Contains("Press any key to exit"))
            //     {
            //         process.StandardInput.Write("a");
            //     }
            // }

            //process.WaitForExit();
        }
    }
}