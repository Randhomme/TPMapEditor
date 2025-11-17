using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Enums;
using TPMapEditor.Enums.WorldObjectDefinition;
using TPMapEditor.Exceptions;
using TPMapEditor.Settings;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public class DataImport : IDisposable
    {
        private readonly StreamReader reader;
        private WorldMap map;
        private IProgress<string> progress;
        private object _lock;

        public DataImport(string filePath, WorldMap map, IProgress<string> progress, object _lock)
        {
            reader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            this.map = map;
            this.progress = progress;
            this._lock = _lock;
        }

        public void ReadMapFileAndAddData()
        {
            progress.Report("Begin map import ...");
            //skip comment line
            reader.ReadLine();
            try
            {
                lock (_lock)
                {
                    ReadWorldInfoSection();
                    ReadGameSection();
                    ReadWorldSection();
                }
            }
            //TODO : handle the error, possibly with an IProgress thing
            catch (Exception ex)
            {
                progress.Report($"An error has occured : {ex.Message}");
            }
        }

        private void ReadSection(string sectionName, Action action)
        {
            progress.Report($"Reading {sectionName} section ...");
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith(sectionName))
                {
                    reader.ReadLine(); //start of section '{'

                    action.Invoke();

                    reader.ReadLine(); //end of section '}'

                    progress.Report($"Done reading {sectionName} section.");
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception($"Fail to read {sectionName} section."); }
        }

        private void ReadWorldInfoSection()
        {
            ReadSection("WorldInfo", () =>
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

        private void ReadGameSection()
        {
            ReadSection("Game", () =>
            {
                for (int i = 0; i < 8; i++)
                    reader.ReadLine();
            });
        }

        private void ReadWorldSection()
        {
            ReadSection("World", () =>
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
                        ReadPlayerSection(i);
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
                        ReadWorldObjectSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read WorldObject section number {i} : {ex.Message}", ex); }
                }

                //GameSpecific section
                ReadGameSpecificSection();
            });
        }

        private void ReadPlayerSection(int playerIndex)
        {
            ReadSection("Player", () =>
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
                ReadFleetAISection(player);

                //FlagIndex
                reader.ReadLine(); //probably state thing ?
            });
        }

        private void ReadFleetAISection(Player player)
        {
            ReadSection("FleetAI", () =>
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

        private void ReadWorldObjectSection()
        {
            try
            {

                //ID
                var id = reader.ReadAndParseInt("ID Int ");

                //Type
                var typeString = reader.ReadAndParseString("Type String ");
                var type = WotGridItem.WotTypes.First((t)=>t.Type == typeString);

                var worldObject = new WorldObject(type, 0, 0, 0) { Id = id };

                ReadWorldObjectStateSection(worldObject);
                
                map.WorldObjects.Add(worldObject);
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read WorldObject section."); }
        }

        private void ReadWorldObjectStateSection(WorldObject worldObject)
        {
            ReadSection("State", () =>
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
                var rotationEulerXYZ = MathUtils.GetEulerXYZ(rotationX, rotationY, rotationZ);
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

        private void ReadGameSpecificSection()
        {
            ReadSection("GameSpecific", () =>
            {
                //World Description
                reader.ReadLine();

                //World Name
                reader.ReadLine();

                //Effect Event Keeper section
                ReadEffectEventKeeperSection();

                //Skybox mesh
                var skyboxMeshString = reader.ReadAndParseString("Skybox mesh name String ");
                map.Skybox = AppSettings.Meshes.FirstOrDefault((meshString) => meshString == skyboxMeshString);

                //Ambient Light
                map.AmbientLightColor = reader.ReadAndParseColor("Ambient Light Colour");

                var roofLightOrientationVector = reader.ReadAndParseVector3("Vector for roof light orientation ");
                (int rloYaw, int rloPitch) = MathUtils.GetYawPitch(roofLightOrientationVector);
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
                        ReadWaypointPathInfoVectorElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Waypoint Path Info section number {i} : {ex.Message}", ex); }
                }

                //World Polygons Vectors - Size
                var worldPolygonCount = reader.ReadAndParseInt("World Polygons Vectors - Size Int ");
                for (int i = 0; i < worldPolygonCount; i++)
                {
                    try
                    {
                        ReadWorldPolygonVectorsSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Polygon section number {i} : {ex.Message}", ex); }
                }

                //World Point Sets Vector - Size
                var worldPointSetCount = reader.ReadAndParseInt("World Point Sets Vector - Size Int ");
                for (int i = 0; i < worldPointSetCount; i++)
                {
                    try
                    {
                        ReadWorldPointSetVectorsSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point Set section number {i} : {ex.Message}", ex); }
                }

                //Flag List - Size
                var flagListCount = reader.ReadAndParseInt("Flag List - Size Int ");
                for (int i = 0; i < flagListCount; i++)
                {
                    try
                    {
                        ReadFlagListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Flag List - Element section number {i} : {ex.Message}", ex); }
                }

                //Timer List - Size
                var timerListCount = reader.ReadAndParseInt("Timer List - Size Int ");
                for (int i = 0; i < timerListCount; i++)
                {
                    try
                    {
                        ReadTimerListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Timer List - Element section number {i} : {ex.Message}", ex); }
                }

                //Speech Event List - Size
                var speechEventListCount = reader.ReadAndParseInt("Speech Event List - Size Int ");
                for (int i = 0; i < speechEventListCount; i++)
                {
                    try
                    {
                        ReadSpeechEventListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Speech Event List - Element section number {i} : {ex.Message}", ex); }
                }

                //PlayerAllianceInfoVector - Size
                var playerAllianceListCount = reader.ReadAndParseInt("PlayerAllianceInfoVector - Size Int ");
                for (int i = 0; i < playerAllianceListCount; i++)
                {
                    try
                    {
                        ReadPlayerAllianceInfoVectorElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read PlayerAllianceInfoVector - Element section number {i} : {ex.Message}", ex); }
                }

                //Team List - Size (InGameTeams)
                var inGameTeamListCount = reader.ReadAndParseInt("Team List - Size Int ");
                for (int i = 0; i < inGameTeamListCount; i++)
                {
                    try
                    {
                        ReadInGameTeamListElementSection();
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
                        ReadGroupSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Group section number {i} : {ex.Message}", ex); }
                }

                //Skip World Rules section
                var worldRulesSectionPosition = reader.BaseStream.Position;
                SkipNamedSection("World Rules");

                //Objective System
                ReadObjectiveSystemSection();

                //Rope
                SkipNamedSection("Rope");

                //Grappled Objects
                SkipNamedSection("Grappled Objects");

                //Boarding Actions
                SkipNamedSection("Boarding Actions");

                //Journal Entry
                ReadJournalEntrySection();

                //World Map
                ReadWorldMapSection();

                //Can Assemble Fleets (not used)
                reader.ReadLine();

                //World Crew List - Size Int
                var worldCrewListCount = reader.ReadAndParseInt("World Crew List - Size Int ");
                for(int i = 0; i < worldCrewListCount; i++)
                {
                    try
                    {
                        ReadWorldCrewListElement();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Crew List - Element number {i} : {ex.Message}", ex); }

                }

                //World Crew List - Size Int
                var worlArmsListCount = reader.ReadAndParseInt("World Arms List - Size Int ");
                for (int i = 0; i < worldCrewListCount; i++)
                {
                    try
                    {
                        ReadWorldArmsListElement();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Arms List - Element number {i} : {ex.Message}", ex); }

                }
            });
        }

        private void ReadEffectEventKeeperSection()
        {
            ReadSection("Effect Event Keeper", () =>
            {
                //NumEffectEventInfoChunks
                var numEffectEvent = reader.ReadAndParseInt("NumEffectEventInfoChunks Int ");

                //EffectEventInfo (skip for now)
                for (int i = 0; i < numEffectEvent; i++)
                    for (int j = 0; j < 5; j++)
                        reader.ReadLine();
            });
        }

        private void ReadWaypointPathInfoVectorElementSection()
        {
            ReadSection("Waypoint Path Info Vector - Element", () =>
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

        private void ReadWorldPolygonVectorsSection()
        {
            ReadSection("World Polygons Vectors - Element", () =>
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

        private void ReadWorldPointSetVectorsSection()
        {
            ReadSection("World Point Sets Vector - Element", () =>
            {
                var worldPointSet = new WorldPointSet(map, reader.ReadAndParseString("Name String "));

                //Points
                var worldPointsCount = reader.ReadAndParseInt("World Points - Size Int ");

                for (int i = 0; i < worldPointsCount; i++)
                {
                    try
                    {
                        ReadWorldPointElementSection(worldPointSet);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point section number {i} : {ex.Message}", ex); }
                }

                map.WorldPointSets.Add(worldPointSet);
            });
        }

        private void ReadWorldPointElementSection(WorldPointSet worldPointSet)
        {
            ReadSection("World Points - Element", () =>
            {
                var worldPoint = new WorldPoint(worldPointSet, 0, 0, 0, 0);

                //world point magnitude (probably not used)
                reader.ReadLine();

                ReadWorldPointBasisSection(worldPoint);

                worldPointSet.Points.Add(worldPoint);
            });
        }

        private void ReadWorldPointBasisSection(WorldPoint worldPoint)
        {
            ReadSection("World Point Basis", () =>
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

                var eulerXYZ = MathUtils.GetEulerXYZ(orientationCross, orientationForward, orientationUp);

                worldPoint.X = position.X;
                worldPoint.Y = position.Y;
                worldPoint.Z = position.Z;
                worldPoint.XRotation = eulerXYZ.X;
                worldPoint.YRotation = eulerXYZ.Y;
                worldPoint.ZRotation = eulerXYZ.Z;
            });
        }

        private void ReadFlagListElementSection()
        {
            ReadSection("Flag List - Element", () =>
            {
                var flag = new Flag(map, reader.ReadAndParseString("Flag Name String "), reader.ReadAndParseBool("Flag Value Bool "));

                map.Flags.Add(flag);
            });
        }

        private void ReadTimerListElementSection()
        {
            ReadSection("Timer List - Element", () =>
            {
                var timer = new Timer(map, reader.ReadAndParseString("Timer Name String "), reader.ReadAndParseBool("Timer Status Bool "), 0);

                reader.ReadLine();
                reader.ReadLine();

                timer.StartTime = reader.ReadAndParseDouble("StartTime Double ");

                reader.ReadLine();

                map.Timers.Add(timer);
            });
        }

        private void ReadSpeechEventListElementSection()
        {
            ReadSection("Speech Event List - Element", () =>
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

        private void ReadPlayerAllianceInfoVectorElementSection()
        {
            ReadSection("PlayerAllianceInfoVector - Element", () =>
            {
                var playerAlliance = new PlayerAlliance(map.Players[reader.ReadAndParseInt("Player0 Int ")], map.Players[reader.ReadAndParseInt("Player1 Int ")]);
                map.PlayerAlliances.Add(playerAlliance);
            });
        }

        private void ReadInGameTeamListElementSection()
        {
            ReadSection("Team List - Element", () =>
            {
                var name = reader.ReadAndParseString("Team Name ID String ");
                var race = (Race)reader.ReadAndParseInt("Race Int ");
                var raceLocked = reader.ReadAndParseBool("Race Lock Bool ");

                map.InGameTeams.Add(new(name) { Race = race, RaceLocked = raceLocked });
            });
        }

        private void ReadGroupSection()
        {
            ReadSection("Group", () =>
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

        private void ReadWorldCrewListElement()
        {
            var crewName = reader.ReadAndParseString("World Crew List - Element String ");
            var wotGridItem = WotGridItem.WotTypes.FirstOrDefault((type) => type.Type == crewName);
            if (wotGridItem != null)
            {
                if(wotGridItem.CustomInfoDefinition == CustomInfoDefinition.CrewCustomInfoFactory)
                {
                    map.WorldCrews.Add(wotGridItem);
                }
                else
                    throw new TPMapEditorException($"World object {crewName} is not a valid crew member.");
            }
            else
                throw new TPMapEditorException($"World object {crewName} does not exists in your TPGame folder.");
        }

        private void ReadWorldArmsListElement()
        {
            var gunName = reader.ReadAndParseString("World Arms List - Element String ");
            var wotGridItem = WotGridItem.WotTypes.FirstOrDefault((type) => type.Type == gunName);
            if (wotGridItem != null)
            {
                if (wotGridItem.CustomInfoDefinition == CustomInfoDefinition.GunCustomInfoFactory)
                {
                    map.WorldArms.Add(wotGridItem);
                }
                else
                    throw new TPMapEditorException($"World object {gunName} is not a valid weapon.");
            }
            else
                throw new TPMapEditorException($"World object {gunName} does not exists in your TPGame folder.");
        }

        private void SkipNamedSection(string sectionName)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith(sectionName))
                {
                    reader.ReadLine(); //start of section
                    SkipSection();
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception($"Fail to read {sectionName} section."); }
        }

        private void SkipSection()
        {
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line.Equals("{"))
                    SkipSection();
                else if (line.Equals("}"))
                    break;
            }
        }

        private void ReadObjectiveSystemSection()
        {
            ReadSection("Objective System", () =>
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
                        ReadObjectivePointInfoElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Point Info - Element section number {i} : {ex.Message}", ex); }
                }

                //Objective Task Array - Size Int
                var objectiveTaskListCount = reader.ReadAndParseInt("Objective Task Array - Size Int ");
                for (int i = 0; i < objectiveTaskListCount; i++)
                {
                    try
                    {
                        ReadObjectiveTaskArrayElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Task Array - Element section number {i} : {ex.Message}", ex); }
                }
            });
        }

        private void ReadObjectivePointInfoElementSection()
        {
            ReadSection("Objective Point Info - Element", () =>
            {
                var name = reader.ReadAndParseString("Name String ");
                var pos = reader.ReadAndParseVector3("Position Vector3");
                map.ObjectivePoints.Add(new(map, name, pos.X, pos.Y, pos.Z));
            });
        }

        private void ReadObjectiveTaskArrayElementSection()
        {
            ReadSection("Objective Task Array - Element", () =>
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

        private void ReadJournalEntrySection()
        {
            ReadSection("Journal Entry", () =>
            {
                //Page Info - Size Int
                var pageInfoSize = reader.ReadAndParseInt("Page Info - Size Int ");
                for (int i = 0; i < pageInfoSize; i++)
                {
                    try
                    {
                        ReadPageInfoElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Page Info - Element section number {i} : {ex.Message}", ex); }
                }

                //Title StringID String 
                map.JournalTitle = reader.ReadAndParseString("Title StringID String ");
            });
        }

        private void ReadPageInfoElementSection()
        {
            ReadSection("Page Info - Element", () =>
            {
                var textStringID = reader.ReadAndParseString("TextStringID String ");
                var speechEventFileName = reader.ReadAndParseString("SpeechEventFileName String ");
                var pictureTexture = reader.ReadAndParseString("PictureTexture String ");

                map.JournalEntries.Add(new(textStringID, speechEventFileName, pictureTexture));
            });
        }

        private void ReadWorldMapSection()
        {
            ReadSection("World Map", () =>
            {
                map.StarmapTexture = reader.ReadAndParseString("Backdrop Texture Name String ");
            });
        }

        

        public void Dispose()
        {
            reader.Dispose();
        }
    }

    
}
