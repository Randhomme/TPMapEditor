using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using TPMapEditor.Enums;
using TPMapEditor.Exceptions;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public static class DataImport
    {
        public static void ReadMapFileAndAddData(string filePath, WorldMap map)
        {
            using var reader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            //skip comment line
            reader.ReadLine();
            try
            {
                ReadWorldInfoSection(reader, map);
                ReadGameSection(reader, map);
                ReadWorldSection(reader, map);
            }
            //TODO : handle the error, possibly with an IProgress thing
            catch { }
        }

        private static void ReadSection(string sectionName, StreamReader reader, Action action)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith(sectionName))
                {
                    reader.ReadLine(); //start of section '{'

                    action.Invoke();

                    reader.ReadLine(); //end of section '}'
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception($"Fail to read {sectionName} section."); }
        }

        private static void ReadWorldInfoSection(StreamReader reader, WorldMap map)
        {
            ReadSection("WorldInfo", reader, () =>
            {
                //IsMultiplayerMap
                map.IsMultiplayer = reader.ReadAndParseBool("IsMultiplayerMap Bool ");

                //MustAssembleFleet
                map.MustAssembleFleet = reader.ReadAndParseBool("MustAssembleFleet Bool ");

                //World Description
                map.WorldDescription = reader.ReadAndParseString("World Description String ");

                //WorldNameID
                map.WorldName = reader.ReadAndParseString("WorldNameID String ");

                //Object count, skip
                reader.ReadLine();

                //Team List - Size
                var teamListSize = reader.ReadAndParseInt("Team List - Size Int ");

                //Team List - Element
                for (int i = 0; i < teamListSize; i++)
                {
                    reader.ReadLine(); //skip line
                    reader.ReadLine(); //skip line

                    //Team Name ID
                    var teamName = reader.ReadAndParseString("Team Name ID String ");

                    //Race
                    var raceInt = reader.ReadAndParseInt("Race Int ");
                    var race = (Race)raceInt;

                    //Race Lock
                    var raceLocked = reader.ReadAndParseBool("Race Lock Bool ");

                    reader.ReadLine(); //skip line

                    map.SelectableTeams.Add(new(teamName) { Race = race, RaceLocked = raceLocked });
                }

                //Number of Players (playable)
                var numberOfPlayers = reader.ReadAndParseInt("Number of Players Int ");

                //Player loop
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    //PlayerInfo - Player Name
                    var playerName = reader.ReadAndParseString("PlayerInfo - Player Name String ");

                    //PlayerInfo - TeamIndex
                    var playerTeam = reader.ReadAndParseInt("PlayerInfo - TeamIndex Int ");

                    map.Players.Add(new(map, playerName, 0, 0, 0, 0, Colors.Red) { IsPlayable = true, SelectableTeam = playerTeam < 0 ? null : map.SelectableTeams[playerTeam] });
                }

                //IsCampaign
                map.IsCampaign = reader.ReadAndParseBool("IsCampaign Bool ");

                //Use Custom World Name
                map.UseCustomName = reader.ReadAndParseBool("Use Custom World Name Bool ");

                //Custom World Name
                map.CustomName = reader.ReadAndParseString("Custom World Name String ");

                //Use Custom World Description
                map.UseCustomDescription = reader.ReadAndParseBool("Use Custom World Description Bool ");

                //Custom World Description
                map.CustomDescription = reader.ReadAndParseString("Custom World Description String ");
            });
        }

        private static void ReadGameSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Game", reader, () =>
            {
                for (int i = 0; i < 8; i++)
                    reader.ReadLine();
            });
        }

        private static void ReadWorldSection(StreamReader reader, WorldMap map)
        {
            ReadSection("World", reader, () =>
            {
                //WorldName and Random Seed (both unused)
                reader.ReadLine();
                reader.ReadLine();

                //World Size - Min Vector3
                var worldSizeMin = reader.ReadAndParseVector3("World Size - Min Vector3");

                //World Size - Max Vector3
                var worldSizeMax = reader.ReadAndParseVector3("World Size - Max Vector3");

                var size = (int)(worldSizeMax.X - worldSizeMin.X);
                map.Size = size < 0 ? -size : size;
                var zSize = (int)(worldSizeMax.Z - worldSizeMin.Z);
                map.ZSize = zSize < 0 ? -zSize : zSize;

                //Player List Comment
                reader.ReadLine();

                //Player List Size
                var playerListSize = reader.ReadAndParseInt("PlayerList Int ");

                //Player section
                for (int i = 0; i < playerListSize; i++)
                {
                    try
                    {
                        ReadPlayerSection(reader, map, i);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Player section number {i} : {ex.Message}", ex); }
                }

                //NextID (not used)
                var line1 = reader.ReadLine();

                //#World Object List (comment line)
                reader.ReadLine();

                //WorldObject (number of WorldObject)
                var worldObjectCount = reader.ReadAndParseInt("WorldObjects Int ");

                //WorldObject Section
                for (int i = 0; i < worldObjectCount; i++)
                {
                    try
                    {
                        ReadWorldObjectSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read WorldObject section number {i} : {ex.Message}", ex); }
                }

                //GameSpecific section
                ReadGameSpecificSection(reader, map);
            });
        }

        private static void ReadPlayerSection(StreamReader reader, WorldMap map, int playerIndex)
        {
            ReadSection("Player", reader, () =>
            {
                //Name
                var playerName = reader.ReadAndParseString("Name String ");
                var player = map.Players.FirstOrDefault((p) => p.Name == playerName);
                if (player is null)
                {
                    player = new(map, playerName, 0, 0, 0, 0, Colors.White);
                    map.Players.Insert(playerIndex, player);
                }

                //Color
                player.Color = reader.ReadAndParseColor("Color Colour");

                //IsPlayable
                reader.ReadLine(); //don't read the value, it should be true if the player is in the playable list, false otherwise

                //Is Used In Game
                reader.ReadLine(); //needs testing, I don't know what this value is used for (maybe state ?)

                //Multiplayer Name
                reader.ReadLine(); //probably state again

                //StartPoint
                var startPoint = reader.ReadAndParseVector3("StartPoint Vector3");
                player.X = startPoint.X;
                player.Y = startPoint.Y;
                player.Z = startPoint.Z;

                //StartPointForwardVector
                var startPointForwardVector = reader.ReadAndParseVector3("StartPointForwardVector Vector3");
                player.Rotation = Math.Atan2(startPointForwardVector.X, startPointForwardVector.Y) * 180 / Math.PI;

                //Race
                reader.ReadLine(); //probably not used

                //Points
                reader.ReadLine(); //probably not used

                //Team Index
                player.TeamIndex = reader.ReadAndParseInt("TeamIndex Int ");

                //Formation type
                var formationTypeStart = (FormationType)reader.ReadAndParseInt("FormationType Int ");

                //FleetAI section
                ReadFleetAISection(reader, player);

                //FlagIndex
                reader.ReadLine(); //probably state thing ?
            });
        }

        private static void ReadFleetAISection(StreamReader reader, Player player)
        {
            ReadSection("FleetAI", reader, () =>
            {
                //UPDATETIMER
                //skip 4 lines
                for (int i = 0; i < 4; i++)
                    reader.ReadLine();

                //OFFSETTIMER
                //skip 4 lines
                for (int i = 0; i < 4; i++)
                    reader.ReadLine();

                //OFFSETTIME
                reader.ReadLine(); //skip

                //UPDATETIME
                reader.ReadLine(); //skip

                //FORMATIONTYPE
                reader.ReadLine();
                reader.ReadLine();
                player.FormationType = (FormationType)Enum.Parse(typeof(FormationType), reader.ReadAndParseString("FORMATIONTYPE String "));
                reader.ReadLine();

                //SHIP INFO SIZE
                var shipInfoSize = reader.ReadAndParseInt("SHIPINFO - Size Int ");
                for (int i = 0; i < shipInfoSize; i++)
                    for (int j = 0; j < 5; j++)
                        reader.ReadLine();

                //HOLDFIREACTIVE
                reader.ReadLine();

                //AITYPE
                reader.ReadLine();
            });
        }

        private static void ReadWorldObjectSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var worldObject = new WorldObject(WotGridItem.WotTypes.FirstOrDefault(), 0, 0, 0);
                map.WorldObjects.Add(worldObject);

                //ID
                worldObject.Id = reader.ReadAndParseInt("ID Int ");

                //Type
                var typeString = reader.ReadAndParseString("Type String ");
                worldObject.Type = WotGridItem.WotTypes.First((t)=>t.Type == typeString);

                ReadWorldObjectStateSection(reader, worldObject, map);
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read WorldObject section."); }
        }

        private static void ReadWorldObjectStateSection(StreamReader reader, WorldObject worldObject, WorldMap map)
        {
            ReadSection("State", reader, () =>
            {
                //HasState (must be false for now)
                var hasState = reader.ReadAndParseBool("HasState Bool ");
                if (hasState)
                    throw new TPMapEditorException("WorldObject cannot have a state (not yet).");

                //Position
                var position = reader.ReadAndParseVector3("Position Vector3");
                worldObject.X = position.X;
                worldObject.Y = position.Y;
                worldObject.Z = position.Z;

                //Orientation
                var (rotationX, rotationY, rotationZ) = reader.ReadAndParseMatrix33("Orientation Matrix33");
                var rotationEulerXYZ = GetEulerXYZ(rotationX, rotationY, rotationZ);
                worldObject.XRotation = rotationEulerXYZ.X;
                worldObject.YRotation = rotationEulerXYZ.Y;
                worldObject.ZRotation = rotationEulerXYZ.Z;

                //PlayerIndex
                var playerIndex = reader.ReadAndParseInt("PlayerIndex Int ");
                if (playerIndex >= 0)
                {
                    try
                    {
                        worldObject.Player = map.Players[playerIndex];
                    }
                    catch { throw new TPMapEditorException("PlayerIndex is incorrect."); }
                }

                //other states
                for (int i = 0; i < 10; i++)
                    reader.ReadLine();
            });
        }

        private static void ReadGameSpecificSection(StreamReader reader, WorldMap map)
        {
            ReadSection("GameSpecific", reader, () =>
            {
                //World Description
                reader.ReadLine();

                //World Name
                reader.ReadLine();

                //Effect Event Keeper section
                ReadEffectEventKeeperSection(reader, map);

                //Skybox mesh
                var skyboxMeshString = reader.ReadAndParseString("Skybox mesh name String ");
                map.Skybox = AppSettings.Meshes.FirstOrDefault((meshString) => meshString == skyboxMeshString);

                //Ambient Light
                map.AmbientLightColor = reader.ReadAndParseColor("Ambient Light Colour");

                var roofLightOrientationVector = reader.ReadAndParseVector3("Vector for roof light orientation ");
                (int rloYaw, int rloPitch) = GetYawPitch(roofLightOrientationVector);
                map.RoofLightOrientationYaw = rloYaw;
                map.RoofLightOrientationPitch = rloPitch;

                //Hemispherical floor light color
                map.FloorLightColor = reader.ReadAndParseColor("Hemispherical floor light color Colour");

                //Hemispherical roof light color
                map.RoofLightColor = reader.ReadAndParseColor("Hemispherical roof light color Colour");

                //World Initialized State (skip for now)
                reader.ReadLine();

                //World Buffer (skip for now, no idea how it's used)
                reader.ReadLine();

                //Waypoint Path Info Vector - Size Int 
                var waypointPathCount = reader.ReadAndParseInt("Waypoint Path Info Vector - Size Int ");

                for (int i = 0; i < waypointPathCount; i++)
                {
                    try
                    {
                        ReadWaypointPathInfoVectorElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Waypoint Path Info section number {i} : {ex.Message}", ex); }
                }

                //World Polygons Vectors - Size
                var worldPolygonCount = reader.ReadAndParseInt("World Polygons Vectors - Size Int ");
                for (int i = 0; i < worldPolygonCount; i++)
                {
                    try
                    {
                        ReadWorldPolygonVectorsSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Polygon section number {i} : {ex.Message}", ex); }
                }

                //World Point Sets Vector - Size
                var worldPointSetCount = reader.ReadAndParseInt("World Point Sets Vector - Size Int ");
                for (int i = 0; i < worldPointSetCount; i++)
                {
                    try
                    {
                        ReadWorldPointSetVectorsSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point Set section number {i} : {ex.Message}", ex); }
                }

                //Flag List - Size
                var flagListCount = reader.ReadAndParseInt("Flag List - Size Int ");
                for (int i = 0; i < flagListCount; i++)
                {
                    try
                    {
                        ReadFlagListElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Flag List - Element section number {i} : {ex.Message}", ex); }
                }

                //Timer List - Size
                var timerListCount = reader.ReadAndParseInt("Timer List - Size Int ");
                for (int i = 0; i < timerListCount; i++)
                {
                    try
                    {
                        ReadTimerListElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Timer List - Element section number {i} : {ex.Message}", ex); }
                }

                //Speech Event List - Size
                var speechEventListCount = reader.ReadAndParseInt("Speech Event List - Size Int ");
                for (int i = 0; i < speechEventListCount; i++)
                {
                    try
                    {
                        ReadSpeechEventListElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Speech Event List - Element section number {i} : {ex.Message}", ex); }
                }

                //PlayerAllianceInfoVector - Size
                var playerAllianceListCount = reader.ReadAndParseInt("PlayerAllianceInfoVector - Size Int ");
                for (int i = 0; i < playerAllianceListCount; i++)
                {
                    try
                    {
                        ReadPlayerAllianceInfoVectorElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read PlayerAllianceInfoVector - Element section number {i} : {ex.Message}", ex); }
                }

                //Team List - Size (InGameTeams)
                var inGameTeamListCount = reader.ReadAndParseInt("Team List - Size Int ");
                for (int i = 0; i < inGameTeamListCount; i++)
                {
                    try
                    {
                        ReadInGameTeamListElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Team List - Element section number {i} : {ex.Message}", ex); }
                }

                //Set the InGameTeam for each player
                foreach (var player in map.Players)
                {
                    player.InGameTeam = player.TeamIndex < 0 ? null : map.InGameTeams[player.TeamIndex];
                }

                //Winning team (not used, or state maybe)
                reader.ReadLine();

                //Num Groups
                var groupCount = reader.ReadAndParseInt("Num Groups Int ");
                for (int i = 0; i < groupCount; i++)
                {
                    try
                    {
                        ReadGroupSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Group section number {i} : {ex.Message}", ex); }
                }

                //Skip World Rules section
                var worldRulesSectionPosition = reader.BaseStream.Position;
                SkipNamedSection(reader, "World Rules");

                //Objective System
                ReadObjectiveSystemSection(reader, map);

                //Rope
                SkipNamedSection(reader, "Rope");

                //Grappled Objects
                SkipNamedSection(reader, "Grappled Objects");

                //Boarding Actions
                SkipNamedSection(reader, "Boarding Actions");

                //Journal Entry
                ReadJournalEntrySection(reader, map);

                //World Map
                ReadWorldMapSection(reader, map);
            });
        }

        private static void ReadEffectEventKeeperSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Effect Event Keeper", reader, () =>
            {
                //NumEffectEventInfoChunks
                var numEffectEvent = reader.ReadAndParseInt("NumEffectEventInfoChunks Int ");

                //EffectEventInfo (skip for now)
                for (int i = 0; i < numEffectEvent; i++)
                    for (int j = 0; j < 5; j++)
                        reader.ReadLine();
            });
        }

        private static void ReadWaypointPathInfoVectorElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Waypoint Path Info Vector - Element", reader, () =>
            {
                var waypointPath = new WaypointPath(map, reader.ReadAndParseString("Waypoint Path Name String "));

                //Waypoint Path Points - Size
                var waypointPathPointsCount = reader.ReadAndParseInt("Waypoint Path Points - Size Int ");

                for (int i = 0; i < waypointPathPointsCount; i++)
                {
                    var vector = reader.ReadAndParseVector3("Waypoint Path Points - Element Vector3");
                    waypointPath.Points.Add(new(waypointPath, vector.X, vector.Y, vector.Z));
                }

                map.WaypointPaths.Add(waypointPath);
            });
        }

        private static void ReadWorldPolygonVectorsSection(StreamReader reader, WorldMap map)
        {
            ReadSection("World Polygons Vectors - Element", reader, () =>
            {
                var worldPolygon = new WorldPolygon(map, reader.ReadAndParseString("Name String "));

                //Points
                var worldPolygonPointsCount = reader.ReadAndParseInt("Points Int ");

                for (int i = 0; i < worldPolygonPointsCount; i++)
                {
                    var vector = reader.ReadAndParseVector2("Points Coord");
                    worldPolygon.Points.Add(new(worldPolygon, vector.X, vector.Y));
                }

                map.WorldPolygons.Add(worldPolygon);
            });
        }

        private static void ReadWorldPointSetVectorsSection(StreamReader reader, WorldMap map)
        {
            ReadSection("World Point Sets Vector - Element", reader, () =>
            {
                var worldPointSet = new WorldPointSet(map, reader.ReadAndParseString("Name String "));

                //Points
                var worldPointsCount = reader.ReadAndParseInt("World Points - Size Int ");

                for (int i = 0; i < worldPointsCount; i++)
                {
                    try
                    {
                        ReadWorldPointElementSection(reader, worldPointSet);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point section number {i} : {ex.Message}", ex); }
                }

                map.WorldPointSets.Add(worldPointSet);
            });
        }

        private static void ReadWorldPointElementSection(StreamReader reader, WorldPointSet worldPointSet)
        {
            ReadSection("World Points - Element", reader, () =>
            {
                var worldPoint = new WorldPoint(worldPointSet, 0, 0, 0, 0);

                //world point magnitude (probably not used)
                reader.ReadLine();

                ReadWorldPointBasisSection(reader, worldPoint);

                worldPointSet.Points.Add(worldPoint);
            });
        }

        private static void ReadWorldPointBasisSection(StreamReader reader, WorldPoint worldPoint)
        {
            ReadSection("World Point Basis", reader, () =>
            {
                //Position Vector3
                var position = reader.ReadAndParseVector3("Position Vector3");

                //LookAt Vector Length Float (probably not used)
                reader.ReadLine();

                //Orientation - Cross Vector3
                var orientationCross = reader.ReadAndParseVector3("Orientation - Cross Vector3");

                //Orientation - Forward Vector3
                var orientationForward = reader.ReadAndParseVector3("Orientation - Forward Vector3");

                //Orientation - Up Vector3
                var orientationUp = reader.ReadAndParseVector3("Orientation - Up Vector3");

                var eulerXYZ = GetEulerXYZ(orientationCross, orientationForward, orientationUp);

                worldPoint.X = position.X;
                worldPoint.Y = position.Y;
                worldPoint.Z = position.Z;
                worldPoint.XRotation = eulerXYZ.X;
                worldPoint.YRotation = eulerXYZ.Y;
                worldPoint.ZRotation = eulerXYZ.Z;
            });
        }

        private static void ReadFlagListElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Flag List - Element", reader, () =>
            {
                var flag = new Flag(map, reader.ReadAndParseString("Flag Name String "), reader.ReadAndParseBool("Flag Value Bool "));

                map.Flags.Add(flag);
            });
        }

        private static void ReadTimerListElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Timer List - Element", reader, () =>
            {
                var timer = new Timer(map, reader.ReadAndParseString("Timer Name String "), reader.ReadAndParseBool("Timer Status Bool "), 0);

                reader.ReadLine();
                reader.ReadLine();

                timer.StartTime = reader.ReadAndParseDouble("StartTime Double ");

                reader.ReadLine();

                map.Timers.Add(timer);
            });
        }

        private static void ReadSpeechEventListElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Speech Event List - Element", reader, () =>
            {
                var speechEvent = new SpeechEvent(map, reader.ReadAndParseString("Name String "))
                {
                    SoundFileName = reader.ReadAndParseString("Sound FileName String "),
                    TextColor = reader.ReadAndParseColor("Text Color Colour"),
                    FaceTexture = reader.ReadAndParseString("FaceTexture String "),
                    TalkingHeadLocation = (TalkingHeadLocation)reader.ReadAndParseInt("TalkingHeadLocation Int "),
                    HasBeenPlayedOnce = reader.ReadAndParseBool("Has Been Played Once Bool "),
                    IsSecondarySpeech = reader.ReadAndParseBool("Is Secondary Speech Bool "),
                    DisplayTime = reader.ReadAndParseDouble("Display Time Float "),
                    OpenChatBar = reader.ReadAndParseBool("Open Chat Bar Bool "),
                    OpenTalkingHead = reader.ReadAndParseBool("Open Talking Head Bool "),
                    HasText = reader.ReadAndParseBool("Has Text Bool "),
                    UseSoundFileLength = reader.ReadAndParseBool("Use Sound File Length Bool "),
                    AlwaysOpenSpeechEventBar = reader.ReadAndParseBool("Always Open Speech Event Bar Bool "),
                };
                //Valid Text StringID Bool (has to always be true, otherwise game crashes)
                reader.ReadLine();
                speechEvent.TextStringID = reader.ReadAndParseString("TextStringID String ");

                //Valid Speaker ID Bool (has to always be true, otherwise game crashes)
                reader.ReadLine();
                speechEvent.SpeakerID = reader.ReadAndParseString("SpeakerID String ");

                map.SpeechEvents.Add(speechEvent);
            });
        }

        private static void ReadPlayerAllianceInfoVectorElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("PlayerAllianceInfoVector - Element", reader, () =>
            {
                var playerAlliance = new PlayerAlliance(map.Players[reader.ReadAndParseInt("Player0 Int ")], map.Players[reader.ReadAndParseInt("Player1 Int ")]);
                map.PlayerAlliances.Add(playerAlliance);
            });
        }

        private static void ReadInGameTeamListElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Team List - Element", reader, () =>
            {
                var name = reader.ReadAndParseString("Team Name ID String ");
                var race = (Race)reader.ReadAndParseInt("Race Int ");
                var raceLocked = reader.ReadAndParseBool("Race Lock Bool ");

                map.InGameTeams.Add(new(name) { Race = race, RaceLocked = raceLocked });
            });
        }

        private static void ReadGroupSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Group", reader, () =>
            {
                var group = new Group(map, reader.ReadAndParseString("Name String "));

                //World Object IDs - Size
                var worldObjectCount = reader.ReadAndParseInt("World Object IDs - Size Int ");
                for (int i = 0; i < worldObjectCount; i++)
                {
                    //World Object IDs - Element
                    var worldObjectId = reader.ReadAndParseInt("World Object IDs - Element Int ");
                    var worldObject = map.WorldObjects.First((wot) => wot.Id == worldObjectId);
                    worldObject.Group = group;
                }

                map.Groups.Add(group);
            });
        }

        private static void SkipNamedSection(StreamReader reader, string sectionName)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith(sectionName))
                {
                    reader.ReadLine(); //start of section
                    SkipSection(reader);
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception($"Fail to read {sectionName} section."); }
        }

        private static void SkipSection(StreamReader reader)
        {
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line.Equals("{"))
                    SkipSection(reader);
                else if (line.Equals("}"))
                    break;
            }
        }

        private static void ReadObjectiveSystemSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Objective System", reader, () =>
            {
                //Current Objective Point Int (skip for now)
                reader.ReadLine();

                //Current Point Visible On StarMap Bool (skip for now)
                reader.ReadLine();

                //Objective Point Info - Size Int
                var objectivePointListCount = reader.ReadAndParseInt("Objective Point Info - Size Int ");
                for (int i = 0; i < objectivePointListCount; i++)
                {
                    try
                    {
                        ReadObjectivePointInfoElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Point Info - Element section number {i} : {ex.Message}", ex); }
                }

                //Objective Task Array - Size Int
                var objectiveTaskListCount = reader.ReadAndParseInt("Objective Task Array - Size Int ");
                for (int i = 0; i < objectiveTaskListCount; i++)
                {
                    try
                    {
                        ReadObjectiveTaskArrayElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Task Array - Element section number {i} : {ex.Message}", ex); }
                }
            });
        }

        private static void ReadObjectivePointInfoElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Objective Point Info - Element", reader, () =>
            {
                var name = reader.ReadAndParseString("Name String ");
                var pos = reader.ReadAndParseVector3("Position Vector3");
                map.ObjectivePoints.Add(new(map, name, pos.X, pos.Y, pos.Z));
            });
        }

        private static void ReadObjectiveTaskArrayElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Objective Task Array - Element", reader, () =>
            {
                var name = reader.ReadAndParseString("Name String ");
                var textStringID = reader.ReadAndParseString("TextStringID String ");

                map.ObjectiveTasks.Add(new(map, name, textStringID)
                {
                    Active = reader.ReadAndParseBool("Active Bool "),
                    Completed = reader.ReadAndParseBool("Completed Bool "),
                    Failed = reader.ReadAndParseBool("Failed Bool "),
                });
            });
        }

        private static void ReadJournalEntrySection(StreamReader reader, WorldMap map)
        {
            ReadSection("Journal Entry", reader, () =>
            {
                //Page Info - Size Int
                var pageInfoSize = reader.ReadAndParseInt("Page Info - Size Int ");
                for (int i = 0; i < pageInfoSize; i++)
                {
                    try
                    {
                        ReadPageInfoElementSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Page Info - Element section number {i} : {ex.Message}", ex); }
                }

                //Title StringID String 
                map.JournalTitle = reader.ReadAndParseString("Title StringID String ");
            });
        }

        private static void ReadPageInfoElementSection(StreamReader reader, WorldMap map)
        {
            ReadSection("Page Info - Element", reader, () =>
            {
                var textStringID = reader.ReadAndParseString("TextStringID String ");
                var speechEventFileName = reader.ReadAndParseString("SpeechEventFileName String ");
                var pictureTexture = reader.ReadAndParseString("PictureTexture String ");

                map.JournalEntries.Add(new(textStringID, speechEventFileName, pictureTexture));
            });
        }

        private static void ReadWorldMapSection(StreamReader reader, WorldMap map)
        {
            ReadSection("World Map", reader, () =>
            {
                map.StarmapTexture = reader.ReadAndParseString("Backdrop Texture Name String ");
            });
        }

        private static bool ReadAndParseBool(this StreamReader reader, string prefix) 
        {
            var line = reader.ReadLine().Trim();
            bool.TryParse(line.GetSafeSubstring(prefix), out var value);
            return value;
        }

        private static int ReadAndParseInt(this StreamReader reader, string prefix) 
        {
            var line = reader.ReadLine().Trim();
            int.TryParse(line.GetSafeSubstring(prefix), out var value);
            return value;
        }

        private static double ReadAndParseDouble(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            double.TryParse(line.GetSafeSubstring(prefix), out var value);
            return value;
        }

        private static string ReadAndParseString(this StreamReader reader, string prefix) 
        {
            var line = reader.ReadLine().Trim();
            return line.GetSafeSubstring(prefix).Trim('\'');
        }

        private static Color ReadAndParseColor(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSafeSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var r);
            float.TryParse(values[1], out var g);
            float.TryParse(values[2], out var b);
            float.TryParse(values[3], out var a);
            return Color.FromArgb((byte)(a * 255f), (byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f));
        }

        private static Vector3 ReadAndParseVector3(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSafeSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x);
            float.TryParse(values[1], out var y);
            float.TryParse(values[2], out var z);
            return new Vector3(x, y, z);
        }

        private static Vector2 ReadAndParseVector2(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSafeSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x);
            float.TryParse(values[1], out var y);
            return new Vector2(x, y);
        }

        private static (Vector3 x, Vector3 y, Vector3 z) ReadAndParseMatrix33(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSafeSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x1);
            float.TryParse(values[1], out var x2);
            float.TryParse(values[2], out var x3);
            float.TryParse(values[0], out var y1);
            float.TryParse(values[1], out var y2);
            float.TryParse(values[2], out var y3);
            float.TryParse(values[0], out var z1);
            float.TryParse(values[1], out var z2);
            float.TryParse(values[2], out var z3);
            return (new Vector3(x1, x2, x3), new Vector3(y1, y2, y3), new Vector3(z1, z2, z3));
            
        }

        private static string GetSafeSubstring(this string str, string val)
        {
            if(str.StartsWith(val))
                return str.Substring(val.Length);
            return string.Empty;
        }

        private static Vector3 GetEulerXYZ(Vector3 X, Vector3 Y, Vector3 Z)
        {
            // Build the rotation matrix from basis vectors
            // Lines correspond to local X, Y, Z axes
            Matrix4x4 m = new Matrix4x4(
                X.X, X.Y, X.Z, 0,
                Y.X, Y.Y, Y.Z, 0,
                Z.X, Z.Y, Z.Z, 0,
                0, 0, 0, 1
            );

            // Extract angles (in radians)
            double sy = -m.M13;
            double cy = Math.Sqrt(1 - sy * sy);

            double x, y, z; // Euler angles in radians

            if (cy > 1e-6)
            {
                x = Math.Atan2(m.M23, m.M33);  // rotation around X
                y = Math.Asin(-m.M13);         // rotation around Y
                z = Math.Atan2(m.M12, m.M11);  // rotation around Z
            }
            else
            {
                // Gimbal lock case
                x = 0;
                y = Math.Asin(-m.M13);
                z = Math.Atan2(-m.M21, m.M22);
            }

            // Convert to degrees
            return new Vector3(
                (float)Math.Round(x * 180.0 / Math.PI),
                (float)Math.Round(y * 180.0 / Math.PI),
                (float)Math.Round(z * 180.0 / Math.PI)
            );
        }

        /// <summary>
        /// Y forward and Z up (just to remember)
        /// </summary>
        private static (int yaw, int pitch) GetYawPitch(Vector3 dir)
        {
            dir = Vector3.Normalize(dir);

            double yaw = Math.Atan2(dir.X, dir.Y);  // rotate around Z (horizontal)
            double pitch = Math.Asin(-dir.Z);         // rotate around X (vertical)

            yaw *= 180f / Math.PI;
            pitch *= 180f / Math.PI;

            yaw = NormalizeAngle(yaw);

            int yawInt = (int)Math.Round(yaw);
            int pitchInt = (int)Math.Round(pitch);

            return (yawInt, pitchInt);
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % 360f;
            if (angle >= 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
