using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Il2CppDumper;

namespace BCL_OffsetGenerator;
public class ill2cppdumperHelper
{
    public static void Dump(string il2cppPath,string metadataPath, string outputDir )
    {
      var currentDirectory = Directory.GetCurrentDirectory();
        try
        {
            Metadata metadata;
            Il2Cpp il2Cpp;
            if (Init(il2cppPath, metadataPath, out metadata, out il2Cpp))
                Dump(metadata, il2Cpp, outputDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine((object) ex);
        }
        Directory.SetCurrentDirectory(currentDirectory);

    }
    
    
      private static bool Init(
      string il2cppPath,
      string metadataPath,
      out Metadata metadata,
      out Il2Cpp il2Cpp)
    {
      Console.WriteLine("Initializing metadata...");
      byte[] buffer1 = File.ReadAllBytes(metadataPath);
      metadata = new Metadata((Stream) new MemoryStream(buffer1));
      Console.WriteLine(string.Format("Metadata Version: {0}", (object) metadata.Version));
      Console.WriteLine("Initializing il2cpp file...");
      byte[] buffer2 = File.ReadAllBytes(il2cppPath);
      uint uint32 = BitConverter.ToUInt32(buffer2, 0);
      MemoryStream memoryStream = new MemoryStream(buffer2);
      switch (uint32)
      {
        case 9460301:
          il2Cpp = (Il2Cpp) new PE((Stream) memoryStream);
          break;
        case 810505038:
          NSO nso = new NSO((Stream) memoryStream);
          il2Cpp = (Il2Cpp) nso.UnCompress();
          break;
        case 1179403647:
          il2Cpp = buffer2[4] != (byte) 2 ? (Il2Cpp) new Elf((Stream) memoryStream) : (Il2Cpp) new Elf64((Stream) memoryStream);
          break;
        case 1836278016:
          WebAssembly webAssembly = new WebAssembly((Stream) memoryStream);
          il2Cpp = (Il2Cpp) webAssembly.CreateMemory();
          break;
        case 3199925962:
        case 3405691582:
          MachoFat machoFat = new MachoFat((Stream) new MemoryStream(buffer2));
          Console.Write("Select Platform: ");
          for (int index = 0; index < machoFat.fats.Length; ++index)
            Console.Write(machoFat.fats[index].magic == 4277009103U ? string.Format("{0}.64bit ", (object) (index + 1)) : string.Format("{0}.32bit ", (object) (index + 1)));
          Console.WriteLine();
          int num = int.Parse(Console.ReadKey(true).KeyChar.ToString()) - 1;
          int magic = (int) machoFat.fats[num % 2].magic;
          memoryStream = new MemoryStream(machoFat.GetMacho(num % 2));
          if (magic != -17958193)
            goto case 4277009102;
          else
            goto case 4277009103;
        case 4277009102:
          il2Cpp = (Il2Cpp) new Macho((Stream) memoryStream);
          break;
        case 4277009103:
          il2Cpp = (Il2Cpp) new Macho64((Stream) memoryStream);
          break;
        default:
          throw new NotSupportedException("ERROR: il2cpp file not supported.");
      }
      double version = config.ForceIl2CppVersion ? config.ForceVersion : metadata.Version;
      il2Cpp.SetProperties(version, metadata.metadataUsagesCount);
      Console.WriteLine(string.Format("Il2Cpp Version: {0}", (object) il2Cpp.Version));
      if (config.ForceDump || il2Cpp.CheckDump())
      {
        if (il2Cpp is ElfBase elfBase)
        {
          Console.WriteLine("Detected this may be a dump file.");
          Console.WriteLine("Input il2cpp dump address or input 0 to force continue:");
          ulong uint64 = Convert.ToUInt64(Console.ReadLine(), 16);
          if (uint64 != 0UL)
          {
            il2Cpp.ImageBase = uint64;
            il2Cpp.IsDumped = true;
            if (!config.NoRedirectedPointer)
              elfBase.Reload();
          }
        }
        else
          il2Cpp.IsDumped = true;
      }
      Console.WriteLine("Searching...");
      try
      {
        bool flag = il2Cpp.PlusSearch(((IEnumerable<Il2CppMethodDefinition>) metadata.methodDefs).Count<Il2CppMethodDefinition>((Func<Il2CppMethodDefinition, bool>) (x => x.methodIndex >= 0)), metadata.typeDefs.Length, metadata.imageDefs.Length);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !flag && il2Cpp is PE)
        {
          Console.WriteLine("Use custom PE loader");
          il2Cpp = (Il2Cpp) PELoader.Load(il2cppPath);
          il2Cpp.SetProperties(version, metadata.metadataUsagesCount);
          flag = il2Cpp.PlusSearch(((IEnumerable<Il2CppMethodDefinition>) metadata.methodDefs).Count<Il2CppMethodDefinition>((Func<Il2CppMethodDefinition, bool>) (x => x.methodIndex >= 0)), metadata.typeDefs.Length, metadata.imageDefs.Length);
        }
        if (!flag)
          flag = il2Cpp.Search();
        if (!flag)
          flag = il2Cpp.SymbolSearch();
        if (!flag)
        {
          Console.WriteLine("ERROR: Can't use auto mode to process file, try manual mode.");
          Console.Write("Input CodeRegistration: ");
          ulong uint64_1 = Convert.ToUInt64(Console.ReadLine(), 16);
          Console.Write("Input MetadataRegistration: ");
          ulong uint64_2 = Convert.ToUInt64(Console.ReadLine(), 16);
          il2Cpp.Init(uint64_1, uint64_2);
        }
        if (il2Cpp.Version >= 27.0)
        {
          if (il2Cpp.IsDumped)
          {
            Il2CppTypeDefinition typeDef = metadata.typeDefs[0];
            Il2CppType type = il2Cpp.types[typeDef.byvalTypeIndex];
            metadata.ImageBase = type.data.typeHandle - (ulong) metadata.header.typeDefinitionsOffset;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        Console.WriteLine("ERROR: An error occurred while processing.");
        return false;
      }
      return true;
    }
      
      private static void Dump(Metadata metadata, Il2Cpp il2Cpp, string outputDir)
      {
        Console.WriteLine("Dumping...");
        Il2CppExecutor il2CppExecutor = new Il2CppExecutor(metadata, il2Cpp);
        new Il2CppDecompiler(il2CppExecutor).Decompile(config, outputDir);
        Console.WriteLine("Done!");
        if (config.GenerateStruct)
        {
          Console.WriteLine("Generate struct...");
          new StructGenerator(il2CppExecutor).WriteScript(outputDir);
          Console.WriteLine("Done!");
        }
        if (!config.GenerateDummyDll)
          return;
        Console.WriteLine("Generate dummy dll...");
        DummyAssemblyExporter.Export(il2CppExecutor, outputDir, config.DummyDllAddToken);
        Console.WriteLine("Done!");
      }

      private static Il2CppDumper.Config config = new Il2CppDumper.Config()
      {
        DumpMethod = true,
        DumpField = true,
        DumpProperty = true,
        DumpAttribute = true,
        DumpFieldOffset = true,
        DumpMethodOffset = true,
        DumpTypeDefIndex = true,
        GenerateDummyDll = false,
        GenerateStruct = true,
        DummyDllAddToken = true,
        RequireAnyKey = true,
        ForceIl2CppVersion = false,
        ForceVersion = 16,
        ForceDump = false,
        NoRedirectedPointer = false
      };
}