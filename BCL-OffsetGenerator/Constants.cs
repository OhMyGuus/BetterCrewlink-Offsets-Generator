namespace BCL_OffsetGenerator;
public class Constants
{
    public const string WORKINGDIRECTORY_PATH = "working";
    public const string OUTPUT_PATH = $"{WORKINGDIRECTORY_PATH}/output";
    public const string AMONGUSFILES_PATH = $"{WORKINGDIRECTORY_PATH}/AmongUsFiles";
    public const int CURRENT_OFFSET_VERSION = 14;

    public static readonly string[] REQUIRED_GAMEFILES =
    {
        "GameAssembly.dll",
        "Among Us_Data/globalgamemanagers",
        "Among Us_Data/il2cpp_data/Metadata/global-metadata.dat",
        "MonoBleedingEdge/EmbedRuntime/MonoPosixHelper.dll"
    };

}