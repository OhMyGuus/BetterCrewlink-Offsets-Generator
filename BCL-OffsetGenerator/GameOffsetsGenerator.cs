using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator
{
    public class GameOffsetsGenerator
    {
        private static Offsets _baseOffsetsx64;
        private static Offsets _baseOffsetsx86;

        private Offsets _baseOffsets;

        private Offsets _offsets;

        private string[] _classfile;

        private MannifestInfo _manifest;

        public GameOffsetsGenerator(MannifestInfo manifest)
        {
            _manifest = manifest;
            _baseOffsets = GetbaseOffsets();
            _classfile = File.ReadAllLines($"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}/dump/dump.cs");
            _offsets = _baseOffsets with { };
        }

        private void Cache()
        {

        }

        private Offsets GetbaseOffsets()
        {
            var getOffsets = () => JsonConvert.DeserializeObject<Offsets>(File.ReadAllText($"basefiles/baseoffsets_{(_manifest.x64 ? "x64" : "x86")}.json"));
            return _manifest.x64 ? _baseOffsetsx64 ?? (_baseOffsetsx64 = getOffsets()) : _baseOffsetsx86 ?? (_baseOffsetsx86 = getOffsets());
        }

        public Offsets Generate()
        {
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine("Generating offsets for: {0}", _manifest.Version);
            stopwatch.Start();
            HandleClassProps(_offsets);
            stopwatch.Stop();
            Console.WriteLine("Stopwatch: {0}ms", stopwatch.ElapsedMilliseconds);
            return _offsets;
            //1061ms
            //3186ms
        }

        private void HandleOffset(string propertyname)
        {
            switch (propertyname)
            {
                case "meetingHudState":
                    _offsets.meetingHudState[0] = GetOffsetFromClass("MeetingHud", "state");
                    break;
                case "allPlayersPtr":
                    _offsets.allPlayersPtr[0] = GetOffsetFromClass("GameData", "AllPlayers");
                    break;
                case "shipStatus_systems":
                    _offsets.shipStatus_systems[0] = GetOffsetFromClass("ShipStatus", "Systems");
                    break;
                case "shipStatus_map":
                    _offsets.shipStatus_map[0] = GetOffsetFromClass("ShipStatus", "Type");
                    break;
                case "shipstatus_allDoors":
                    _offsets.shipstatus_allDoors[0] = GetOffsetFromClass("ShipStatus", "AllDoors");
                    break;
                case "door_doorId":
                    break;
                case "door_isOpen":
                    _offsets.door_isOpen = GetOffsetFromClass("PlainDoor", "Open");
                    break;
                case "deconDoorUpperOpen":
                    _offsets.deconDoorLowerOpen = new List<long> { GetOffsetFromClass("DeconSystem", "UpperDoor"), _offsets.door_isOpen };
                    break;
                case "deconDoorLowerOpen":
                    _offsets.deconDoorLowerOpen = new List<long> { GetOffsetFromClass("DeconSystem", "LowerDoor"), _offsets.door_isOpen };
                    break;
                case "hqHudSystemType_CompletedConsoles":
                    _offsets.hqHudSystemType_CompletedConsoles[0] = GetOffsetFromClass("HqHudSystemType", "CompletedConsoles");
                    break;
                case "HudOverrideSystemType_isActive":
                    _offsets.HudOverrideSystemType_isActive[0] = GetOffsetFromClass("HudOverrideSystemType", "<IsActive>k__BackingField");
                    break;
                case "planetSurveillanceMinigame_currentCamera":
                    _offsets.planetSurveillanceMinigame_currentCamera[0] = GetOffsetFromClass("PlanetSurveillanceMinigame", "currentCamera");
                    break;
                case "planetSurveillanceMinigame_camarasCount":
                    _offsets.planetSurveillanceMinigame_camarasCount[0] = GetOffsetFromClass("PlanetSurveillanceMinigame", "survCameras");
                    break;
                case "surveillanceMinigame_FilteredRoomsCount":
                    _offsets.surveillanceMinigame_FilteredRoomsCount[0] = GetOffsetFromClass("SurveillanceMinigame", "FilteredRooms");
                    break;
                case "palette_playercolor":
                    _offsets.palette_playercolor[0] = GetOffsetFromClass("Palette", "PlayerColors");
                    break;
                case "palette_shadowColor":
                    _offsets.palette_shadowColor[0] = GetOffsetFromClass("Palette", "ShadowColors");
                    break;
                case "lightRadius":
                    _offsets.lightRadius = new List<long> { GetOffsetFromClass("PlayerControl", "myLight"), GetOffsetFromClass("LightSource", "LightRadius") };
                    break;
                case "gameOptions_MapId":
                    _offsets.gameOptions_MapId[0] = GetOffsetFromClass("GameOptionsData", "MapId");
                    break;
                case "gameOptions_MaxPLayers":
                    _offsets.gameOptions_MaxPLayers[0] = GetOffsetFromClass("GameOptionsData", "MaxPlayers");
                    break;
                case "serverManager_currentServer": // still needs work and double check
                                                    //  offsets.serverManager_currentServer[0] = GetOffsetFromClass("GameOptionsData", "<CurrentRegion>k__BackingField");
                    break;

                case "innerNetClient.networkAddress":
                    _offsets.innerNetClient.networkAddress = GetOffsetFromClass("InnerNetClient", "networkAddress");
                    break;
                case "innerNetClient.networkPort":
                    _offsets.innerNetClient.networkPort = GetOffsetFromClass("InnerNetClient", "networkPort");
                    break;
                case "innerNetClient.gameMode":
                    _offsets.innerNetClient.gameMode = GetOffsetFromClass("InnerNetClient", "GameMode");
                    break;
                case "innerNetClient.gameId":
                    var off1 = GetOffsetFromClass("InnerNetClient", "GameId");
                    _offsets.innerNetClient.gameId = off1 != -1 ? off1 : GetOffsetFromClass("AmongUsClient", "GameId");
                    break;
                case "innerNetClient.hostId":
                    _offsets.innerNetClient.hostId = GetOffsetFromClass("InnerNetClient", "HostId");
                    break;
                case "innerNetClient.clientId":
                    _offsets.innerNetClient.clientId = GetOffsetFromClass("InnerNetClient", "ClientId");
                    break;
                case "innerNetClient.gameState":
                    _offsets.innerNetClient.gameState = GetOffsetFromClass("InnerNetClient", "GameState");
                    break;
                case "innerNetClient.onlineScene":
                    _offsets.innerNetClient.onlineScene = GetOffsetFromClass("AmongUsClient", "OnlineScene");
                    break;
                case "innerNetClient.mainMenuScene":
                    _offsets.innerNetClient.mainMenuScene = GetOffsetFromClass("AmongUsClient", "MainMenuScene");
                    break;
                case "player.isLocal":
                    _offsets.player.isLocal[0] = GetOffsetFromClass("PlayerControl", "myLight");
                    break;
                case "player.isDummy":
                    _offsets.player.isDummy[0] = GetOffsetFromClass("PlayerControl", "isDummy");
                    break;
                case "player.localX":
                    _offsets.player.localX[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.localY":
                    _offsets.player.localY[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.remoteX":
                    _offsets.player.remoteX[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.remoteY":
                    _offsets.player.remoteY[0] = GetOffsetFromClass("PlayerControl", "NetTransform");
                    break;
                case "player.inVent":
                    _offsets.player.inVent[0] = GetOffsetFromClass("PlayerControl", "inVent");
                    break;
                case "player.clientId":
                    _offsets.player.clientId[0] = GetOffsetFromClass("InnerNetObject", "OwnerId");
                    break;
                case "player.currentOutfit":
                    _offsets.player.currentOutfit[0] = GetOffsetFromClass("PlayerControl", "<CurrentOutfitType>k__BackingField");
                    break;
                case "player.roleTeam":
                    _offsets.player.roleTeam[0] = GetOffsetFromClass("RoleBehaviour", "TeamType");
                    break;
                case "player.nameText":
                    _offsets.player.nameText[0] = GetOffsetFromClass("PlayerControl", "nameText");
                    if (_offsets.player.nameText[0] == -1)
                    {
                        var txtMeshProOffset = _offsets.player.nameText[1];
                        _offsets.player.nameText = new List<long>()
                        {
                            GetOffsetFromClass("PlayerControl", "cosmetics"), GetOffsetFromClass("CosmeticsLayer", "nameText"),
                            txtMeshProOffset
                        };
                    }
                    break;
                case "player.outfit.colorId":
                    _offsets.player.outfit.colorId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "ColorId");
                    break;
                case "player.outfit.hatId":
                    _offsets.player.outfit.hatId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "HatId");
                    break;
                case "player.outfit.skinId":
                    _offsets.player.outfit.skinId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "SkinId");
                    break;
                case "player.outfit.visorId":
                    _offsets.player.outfit.visorId[0] = GetOffsetFromClass("GameData.PlayerOutfit", "VisorId");
                    break;
                case "player.outfit.playerName":
                    {
                        _offsets.player.outfit.playerName[0] = GetOffsetFromClass("GameData.PlayerOutfit", "_playerName");
                        if (_offsets.player.outfit.playerName[0] == -1)
                        {
                            _offsets.player.outfit.playerName[0] = GetOffsetFromClass("GameData.PlayerOutfit", "postCensorName");
                        }
                        break;
                    }

                case "player.outfit":
                    HandleClassProps(_offsets.player.outfit, "player.outfit.");
                    break;
                case "player._struct":
                    {
                        _offsets.player._struct = GeneratePlayerStruct();

                        // needs works
                        break;
                    }
                case "innerNetClient":
                    HandleClassProps(_offsets.innerNetClient, "innerNetClient.");
                    break;
                case "player":
                    HandleClassProps(_offsets.player, "player.");
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
                AddVal(structs, "UINT", "name", lookup_old._playerNamePtr == -1 ? lookup_old._playerNamePtr : GetOffsetFromClass("GameData.PlayerInfo", "PlayerName"), ref curOffset);
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
            Parallel.ForEach(obj.GetType().GetProperties(), (propertyInfo) =>
            {
                HandleOffset(prefix + propertyInfo.Name);

            });
            //foreach (PropertyInfo propertyInfo in )
            //{
            //HandleOffset(prefix + propertyInfo.Name);
            //}
        }
        private long GetOffsetFromClass(string classname, string offsetname)
        {
            var classIndex = _classfile.GetLineIndex(o => o.Contains("class " + classname + " "));
            if (classIndex == -1)
            {
                Console.WriteLine("Offset->class not found {0}->{1}", classname, offsetname);
                return -1;
            }
            for (int i = classIndex; i < classIndex + MAX_CLASSLENGTH; i++)
            {
                var line = _classfile[i].Replace(", ", ",").Replace(" static", "").Replace(" readonly", "").Split(" ");
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
