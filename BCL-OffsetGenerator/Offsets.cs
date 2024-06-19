using Newtonsoft.Json;
using System.Collections.Generic;

namespace BCL_OffsetGenerator;

public record Offsets
{
    public List<long> meetingHud { get; set; } = new List<long>();
    public List<long> objectCachePtr { get; set; } = new List<long>();
    public List<long> meetingHudState { get; set; } = new List<long>();
    public List<long> allPlayersPtr { get; set; } = new List<long>();
    public List<long> allPlayers { get; set; } = new List<long>();
    public List<long> playerCount { get; set; }
    public long playerAddrPtr { get; set; }
    public List<long> shipStatus { get; set; } = new List<long>();
    public List<long> shipStatus_systems { get; set; } = new List<long>();
    public List<long> shipStatus_map { get; set; } = new List<long>();
    public List<long> shipstatus_allDoors { get; set; } = new List<long>();
    public long door_doorId { get; set; }
    public long door_isOpen { get; set; }
    public long mushroomDoor_isOpen { get; set; }
    public List<long> deconDoorUpperOpen { get; set; } = new List<long>();
    public List<long> deconDoorLowerOpen { get; set; } = new List<long>();
    public List<long> hqHudSystemType_CompletedConsoles { get; set; } = new List<long>();
    public List<long> HudOverrideSystemType_isActive { get; set; } = new List<long>();
    public List<long> miniGame { get; set; }
    public List<long> planetSurveillanceMinigame_currentCamera { get; set; } = new List<long>();
    public List<long> planetSurveillanceMinigame_camarasCount { get; set; } = new List<long>();
    public List<long> surveillanceMinigame_FilteredRoomsCount { get; set; } = new List<long>();
    public List<long> lightRadius { get; set; }
    public List<long> palette { get; set; }
    public List<long> palette_playercolor { get; set; } = new List<long>();
    public List<long> palette_shadowColor { get; set; } = new List<long>();
    public List<long> playerControl_GameOptions { get; set; } = new List<long>(); //remove after a bit dpe of gameoptionsdata
    public List<long> gameoptionsData { get; set; } = new List<long>();
    public List<long> gameOptions_MapId { get; set; } = new List<long>();
    public List<long> gameOptions_MaxPLayers { get; set; } = new List<long>();
    public List<long> serverManager_currentServer { get; set; } = new List<long>();
    public long connectFunc { get; set; }
    public long showModStampFunc { get; set; }
    public long modLateUpdateFunc { get; set; }
    public long fixedUpdateFunc { get; set; }
    public long pingMessageString { get; set; }
    public Innernetclient innerNetClient { get; set; }
    public Player player { get; set; }
    public Signatures signatures { get; set; }
    public bool oldMeetingHud { get; set; }
    public bool disableWriting { get; set; }
    public bool newGameOptions { get; set; }

}

public record Innernetclient
{
    [JsonProperty("base")]
    public List<long> _base { get; set; } = new List<long>();
    public long networkAddress { get; set; }
    public long networkPort { get; set; }

    public long gameMode { get; set; }
    public long gameId { get; set; }
    public long hostId { get; set; }
    public long clientId { get; set; }
    public long gameState { get; set; }
    public long onlineScene { get; set; }
    public long mainMenuScene { get; set; }
}

public record Player
{
    [JsonProperty("struct")]
    public Struct[] _struct { get; set; }
    public List<long> isDummy { get; set; }
    public List<long> isLocal { get; set; }
    public List<long> localX { get; set; }
    public List<long> localY { get; set; }
    public List<long> remoteX { get; set; }
    public List<long> remoteY { get; set; }
    public long bufferLength { get; set; }
    public List<long> offsets { get; set; }
    public List<long> inVent { get; set; }
    public List<long> clientId { get; set; }
    public List<long> currentOutfit { get; set; }
    public List<long> roleTeam { get; set; }
    public List<long> nameText { get; set; }
    public Outfit outfit { get; set; }
}

public class Outfit
{
    public List<long> colorId { get; set; }
    public List<long> hatId { get; set; }
    public List<long> skinId { get; set; }
    public List<long> visorId { get; set; }
    public List<long> playerName { get; set; }
}

public class Struct
{
    public string type { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? skip { get; set; }
    public string name { get; set; }
}

public class Signatures
{
    public Signature innerNetClient { get; set; }
    public Signature meetingHud { get; set; }
    public Signature gameData { get; set; }
    public Signature shipStatus { get; set; }
    public Signature miniGame { get; set; }
    public Signature palette { get; set; }
    public Signature playerControl { get; set; }
    public Signature showModStamp { get; set; }
    public Signature connectFunc { get; set; }
    public Signature fixedUpdateFunc { get; set; }
    public Signature pingMessageString { get; set; }
    public Signature modLateUpdate { get; set; }
    public Signature serverManager { get; set; }
    public Signature gameOptionsManager { get; set; }

}

public class Signature
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? sig { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? patternOffset { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? addressOffset { get; set; }
}


