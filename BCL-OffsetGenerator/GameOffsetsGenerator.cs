using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace BCL_OffsetGenerator
{
    partial class GameOffsetsGenerator
    {
        private Offsets baseOffsets;
        private Offsets offsets;

        private string[] classfile;
        private MannifestInfo _mannifestInfo;

        public GameOffsetsGenerator(MannifestInfo mannifestInfo, string path)
        {
            _mannifestInfo = mannifestInfo;
            baseOffsets = GetbaseOffsets();
            classfile = File.ReadAllLines($"{path}/dump/dump.cs");
            offsets = baseOffsets with { };
        }

        internal Offsets GetbaseOffsets()
        {
            return JsonConvert.DeserializeObject<Offsets>(File.ReadAllText($"baseoffsets/{(_mannifestInfo.x64 ? "x64" : "x86")}/baseoffsets.json"));
        }
        internal Offsets Generate()
        {
            HandleClassProps(offsets);
            return offsets;
        }

        internal void HandleOffset(string propertyname)
        {
            switch (propertyname)
            {
                case "meetingHudState":
                    offsets.meetingHudState[0] = GetOffsetFromClass("MeetingHud", "state");
                    break;
                case "allPlayersPtr":
                    offsets.allPlayersPtr[0] = GetOffsetFromClass("GameData", "AllPlayers");
                    break;
                case "shipStatus_systems":
                    offsets.shipStatus_systems[0] = GetOffsetFromClass("ShipStatus", "Systems");
                    break;
                case "shipStatus_map":
                    offsets.shipStatus_map[0] = GetOffsetFromClass("ShipStatus", "Type");
                    break;
                case "shipstatus_allDoors":
                    offsets.shipstatus_allDoors[0] = GetOffsetFromClass("ShipStatus", "AllDoors");
                    break;
                case "door_doorId":
                    offsets.door_doorId = GetOffsetFromClass("PlainDoor", "Id");
                    break;
                case "door_isOpen":
                    offsets.door_isOpen = GetOffsetFromClass("PlainDoor", "Open");
                    break;
                case "deconDoorUpperOpen":
                    offsets.deconDoorLowerOpen = new List<long> { GetOffsetFromClass("DeconSystem", "UpperDoor"), offsets.door_isOpen };
                    break;
                case "deconDoorLowerOpen":
                    offsets.deconDoorLowerOpen = new List<long> { GetOffsetFromClass("DeconSystem", "LowerDoor"), offsets.door_isOpen };
                    break;
                case "hqHudSystemType_CompletedConsoles":
                    offsets.hqHudSystemType_CompletedConsoles[0] = GetOffsetFromClass("HqHudSystemType", "CompletedConsoles");
                    break;
                case "HudOverrideSystemType_isActive":
                    offsets.HudOverrideSystemType_isActive[0] = GetOffsetFromClass("HudOverrideSystemType", "<IsActive>k__BackingField");
                    break;
                case "planetSurveillanceMinigame_currentCamera":
                    offsets.planetSurveillanceMinigame_currentCamera[0] = GetOffsetFromClass("PlanetSurveillanceMinigame", "currentCamera");
                    break;
                case "planetSurveillanceMinigame_camarasCount":
                    offsets.planetSurveillanceMinigame_camarasCount[0] = GetOffsetFromClass("PlanetSurveillanceMinigame", "survCameras");
                    break;
                case "surveillanceMinigame_FilteredRoomsCount":
                    offsets.surveillanceMinigame_FilteredRoomsCount[0] = GetOffsetFromClass("SurveillanceMinigame", "FilteredRooms");
                    break;
                case "palette_playercolor":
                    offsets.palette_playercolor[0] = GetOffsetFromClass("Palette", "PlayerColors");
                    break;
                case "palette_shadowColor":
                    offsets.palette_shadowColor[0] = GetOffsetFromClass("Palette", "ShadowColors");
                    break;
                case "lightRadius":
                    offsets.lightRadius = new List<long> { GetOffsetFromClass("PlayerControl", "myLight"), GetOffsetFromClass("LightSource", "LightRadius") };
                    break;
                case "gameOptions_MapId":
                    offsets.gameOptions_MapId[0] = GetOffsetFromClass("GameOptionsData", "MapId");
                    break;
                case "gameOptions_MaxPLayers":
                    offsets.gameOptions_MaxPLayers[0] = GetOffsetFromClass("GameOptionsData", "MaxPlayers");
                    break;
                case "serverManager_currentServer": // still needs work and double check
                                                    //  offsets.serverManager_currentServer[0] = GetOffsetFromClass("GameOptionsData", "<CurrentRegion>k__BackingField");
                    break;

                case "innerNetClient.networkAddress":
                    offsets.innerNetClient.networkAddress = GetOffsetFromClass("InnerNetClient", "networkAddress");
                    break;
                case "innerNetClient.networkPort":
                    offsets.innerNetClient.networkAddress = GetOffsetFromClass("InnerNetClient", "networkPort");
                    break;
                case "innerNetClient.gameMode":
                    offsets.innerNetClient.gameMode = GetOffsetFromClass("InnerNetClient", "GameMode");
                    break;
                case "innerNetClient.gameId":
                    offsets.innerNetClient.gameId = GetOffsetFromClass("InnerNetClient", "GameId");
                    break;
                case "innerNetClient.hostId":
                    offsets.innerNetClient.hostId = GetOffsetFromClass("InnerNetClient", "HostId");
                    break;
                case "innerNetClient.clientId":
                    offsets.innerNetClient.clientId = GetOffsetFromClass("InnerNetClient", "ClientId");
                    break;
                case "innerNetClient.gameState":
                    offsets.innerNetClient.gameState = GetOffsetFromClass("InnerNetClient", "GameState");
                    break;
                case "innerNetClient.onlineScene":
                    offsets.innerNetClient.onlineScene = GetOffsetFromClass("AmongUsClient", "OnlineScene");
                    break;
                case "innerNetClient.mainMenuScene":
                    offsets.innerNetClient.mainMenuScene = GetOffsetFromClass("AmongUsClient", "MainMenuScene");
                    break;
                case "player.isLocal":
                    offsets.player.isLocal[0] = GetOffsetFromClass("PlayerControl", "myLight");
                    break;
                case "player.isDummy":
                    offsets.player.isDummy[0] = GetOffsetFromClass("PlayerControl", "isDummy");
                    break;
                case "player.localX":
                    offsets.player.localX[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.localY":
                    offsets.player.localY[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.remoteX":
                    offsets.player.remoteX[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.remoteY":
                    offsets.player.remoteY[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.inVent":
                    offsets.player.inVent[0] = GetOffsetFromClass("PlayerControl", "inVent");
                    break;
                case "player.clientId":
                    offsets.player.clientId[0] = GetOffsetFromClass("InnerNetObject", "OwnerId");
                    break;
                case "player.currentOutfit":
                    offsets.player.currentOutfit[0] = GetOffsetFromClass("PlayerControl", "<CurrentOutfitType>k__BackingField");
                    break;
                case "player.roleTeam":
                    offsets.player.roleTeam[0] = GetOffsetFromClass("RoleBehaviour", "TeamType");
                    break;
                case "player.nameText":
                    offsets.player.nameText[0] = GetOffsetFromClass("PlayerControl", "nameText");
                    break;
                case "player.outfit.colorId":
                    offsets.player.outfit.colorId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "ColorId");
                    break;
                case "player.outfit.hatId":
                    offsets.player.outfit.hatId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "HatId");
                    break;
                case "player.outfit.skinId":
                    offsets.player.outfit.skinId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "SkinId");
                    break;
                case "player.outfit.visorId":
                    offsets.player.outfit.visorId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "VisorId");
                    break;
                case "player.outfit.playerName":
                    offsets.player.outfit.playerName[0] = GetOffsetFromClass("GameData.PlayerOutfit", "_playerName");
                    break;
                case "player.outfit":
                    HandleClassProps(offsets.player.outfit, "player.outfit.");
                    break;
                case "player._struct":
                    {
                        offsets.player._struct = GeneratePlayerStruct();

                        // needs works
                        break;
                    }
                case "innerNetClient":
                    HandleClassProps(offsets.innerNetClient, "innerNetClient.");
                    break;
                case "player":
                    HandleClassProps(offsets.player, "player.");
                    break;
                default:
                    // Console.WriteLine("SKIPPING: {0}", propertyname);
                    break;


            }
        }

        private Struct[] GeneratePlayerStruct()
        {
            var structs = new List<Struct>();
            var lookup = new
            {
                PlayerId = GetOffsetFromClass("GameData.PlayerInfo", "PlayerId"),
                outfitsPtr = GetOffsetFromClass("GameData.PlayerInfo", "Outfits"),
                playerLevel = GetOffsetFromClass("GameData.PlayerInfo", "PlayerLevel"),
                disconnected = GetOffsetFromClass("GameData.PlayerInfo", "Disconnected"),
                rolePtr = GetOffsetFromClass("GameData.PlayerInfo", "Role"),
                taskPtr = GetOffsetFromClass("GameData.PlayerInfo", "Tasks"),
                dead = GetOffsetFromClass("GameData.PlayerInfo", "IsDead"),
                _objectPtr = GetOffsetFromClass("GameData.PlayerInfo", "_object"),
            };
            var curOffset = 0L;
            if (lookup.playerLevel != -1) // new ver
            {
                AddVal(structs, "UINT", "id", lookup.PlayerId, ref curOffset);
                AddVal(structs, "UINT", "outfitsPtr", lookup.outfitsPtr, ref curOffset);
                AddVal(structs, "UINT", "playerLevel", lookup.playerLevel, ref curOffset);
                AddVal(structs, "UINT", "disconnected", lookup.disconnected, ref curOffset);
                AddVal(structs, "UINT", "rolePtr", lookup.rolePtr, ref curOffset);
                AddVal(structs, "UINT", "taskPtr", lookup.taskPtr, ref curOffset);
                AddVal(structs, "BYTE", "dead", lookup.dead, ref curOffset);
                AddVal(structs, "UINT", "objectPtr", lookup._objectPtr, ref curOffset);
            }
            else
            {
                var lookup_old = new
                {
                    _playerNamePtr = GetOffsetFromClass("GameData.PlayerInfo", "_playerName"),
                    colorId = GetOffsetFromClass("GameData.PlayerInfo", "ColorId"),
                    hatId = GetOffsetFromClass("GameData.PlayerInfo", "HatId"),
                    petId = GetOffsetFromClass("GameData.PlayerInfo", "PetId"),
                    skinId = GetOffsetFromClass("GameData.PlayerInfo", "SkinId"),
                    isImpostor = GetOffsetFromClass("GameData.PlayerInfo", "IsImpostor"),
                };
                AddVal(structs, "UINT", "id", lookup.PlayerId, ref curOffset);
                AddVal(structs, "UINT", "name", lookup_old._playerNamePtr == -1 ? GetOffsetFromClass("GameData.PlayerInfo", "PlayerName") : lookup_old._playerNamePtr, ref curOffset);
                AddVal(structs, "UINT", "color", lookup_old.colorId, ref curOffset);
                AddVal(structs, "UINT", "hat", lookup_old.hatId, ref curOffset);
                AddVal(structs, "UINT", "pet", lookup_old.petId, ref curOffset);
                AddVal(structs, "UINT", "skin", lookup_old.skinId, ref curOffset);
                AddVal(structs, "UINT", "disconnected", lookup.disconnected, ref curOffset);
                AddVal(structs, "UINT", "taskPtr", lookup.taskPtr, ref curOffset);
                AddVal(structs, "BYTE", "impostor", lookup_old.isImpostor, ref curOffset);
                AddVal(structs, "BYTE", "dead", lookup.dead, ref curOffset);
                AddVal(structs, "UINT", "objectPtr", lookup._objectPtr, ref curOffset);
            }

            return structs.ToArray();
        }

        private void AddVal(List<Struct> strc, string type, string name, long offset, ref long start)
        {
            AddUnused(strc, ref start, offset);
            strc.Add(new Struct() { type = type, name = name });
            switch (type)
            {
                case "UINT":
                    start += sizeof(uint);
                    break;
                case "BYTE":
                    start += sizeof(byte);
                    break;
            }

        }
        private void AddUnused(List<Struct> strc, ref long start, long stop)
        {
            var val = (stop - start);
            if (val > 0)
            {
                start += val;
                strc.Add(new Struct() { type = "SKIP", skip = val, name = "unused" });
            }
        }
        const int MAX_CLASSLENGTH = 200;

        private void HandleClassProps<T>(T obj, string prefix = "")
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                HandleOffset(prefix + propertyInfo.Name);
            }
        }
        private long GetOffsetFromClass(string classname, string offsetname)
        {
            var classIndex = classfile.GetLineIndex(o => o.Contains("class " + classname + " "));
            if (classIndex == -1)
            {
                Console.WriteLine("Offset->class not found {0}->{1}", classname, offsetname);
                return -1;
            }
            for (int i = classIndex; i < classIndex + MAX_CLASSLENGTH; i++)
            {
                var line = classfile[i].Replace(", ", ",").Replace(" static", "").Replace(" readonly", "").Split(" ");
                if (line.Length == 5 && line[2] == offsetname + ";")
                {
                    if (long.TryParse(line[4].Substring(2, line[4].Length - 2), NumberStyles.HexNumber, null, out long result))
                    {
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("Offset not found {0}->{1}", classname, offsetname);
                        return -1;
                    }
                }
            }
            Console.WriteLine("Offset not found {0}->{1}", classname, offsetname);
            return -1;
        }

    }
}
