using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using TPMapEditor.Data.Rule;
using TPMapEditor.Enums;
using TPMapEditor.Enums.WorldObjectDefinition;
using TPMapEditor.Exceptions;
using TPMapEditor.Settings;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public class DataImport : IDisposable
    {
        private readonly PositionnedStreamReader reader;
        private WorldMap map;
        private IProgress<string> progress;
        private IProgress<string> progressOperation;
        private object _lock;

        public DataImport(string filePath, WorldMap map, IProgress<string> progress, IProgress<string> progressOperation, object _lock)
        {
            reader = new PositionnedStreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            this.map = map;
            this.progress = progress;
            this.progressOperation = progressOperation;
            this._lock = _lock;
        }

        public void ReadMapFileAndAddData()
        {
            progressOperation.Report("Begin map import ...");
            var time = DateTime.Now;
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
                progressOperation.Report($"Map import completed in {(DateTime.Now - time).TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                progressOperation.Report("Map import failed.");
                progress.Report($"An error has occured.\n{ex.Message}");
            }
        }

        private void ReadSection(string sectionName, Action action)
        {
            //progress.Report($"Reading {sectionName} section ...");
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith(sectionName))
                {
                    reader.ReadLine(); //start of section '{'

                    action.Invoke();

                    reader.ReadLine(); //end of section '}'

                    //progress.Report($"Done reading {sectionName} section.");
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch (Exception ex) { throw new Exception($"Fail to read {sectionName} section: {ex.Message}."); }
        }

        private void ReadWorldInfoSection()
        {
            ReadSection("WorldInfo", () =>
            {
                progressOperation.Report("Reading WorldInfo section ...");

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
                progressOperation.Report("Reading World section ...");

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
                    progressOperation.Report($"Reading Player section {i + 1} / {playerListSize} ...");
                    try
                    {
                        ReadPlayerSection(i);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Player section number {i + 1} : {ex.Message}", ex); }
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
                    progressOperation.Report($"Reading WorldObject section {i + 1} / {worldObjectCount} ...");
                    try
                    {
                        ReadWorldObjectSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read WorldObject section number {i + 1} : {ex.Message}", ex); }
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
                var type = WotGridItem.WotTypes.FirstOrDefault((t)=>t.Type == typeString);

                if (type is null)
                    throw new TPMapEditorException($"WorldObject type '{type}' not found.");

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
                progressOperation.Report("Reading GameSpecific section ...");

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
                    progressOperation.Report($"Reading Waypoint Path Info section {i + 1} / {waypointPathCount} ...");
                    try
                    {
                        ReadWaypointPathInfoVectorElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Waypoint Path Info section number {i + 1} : {ex.Message}", ex); }
                }

                //World Polygons Vectors - Size
                var worldPolygonCount = reader.ReadAndParseInt("World Polygons Vectors - Size Int ");
                for (int i = 0; i < worldPolygonCount; i++)
                {
                    progressOperation.Report($"Reading World Polygon section {i + 1} / {waypointPathCount} ...");
                    try
                    {
                        ReadWorldPolygonVectorsSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Polygon section number {i + 1} : {ex.Message}", ex); }
                }

                //World Point Sets Vector - Size
                var worldPointSetCount = reader.ReadAndParseInt("World Point Sets Vector - Size Int ");
                for (int i = 0; i < worldPointSetCount; i++)
                {
                    progressOperation.Report($"Reading World Point Set section {i + 1} / {worldPointSetCount} ...");
                    try
                    {
                        ReadWorldPointSetVectorsSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point Set section number {i + 1} : {ex.Message}", ex); }
                }

                //Flag List - Size
                var flagListCount = reader.ReadAndParseInt("Flag List - Size Int ");
                for (int i = 0; i < flagListCount; i++)
                {
                    progressOperation.Report($"Reading Flag section {i + 1} / {flagListCount} ...");
                    try
                    {
                        ReadFlagListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Flag List - Element section number {i + 1} : {ex.Message}", ex); }
                }

                //Timer List - Size
                var timerListCount = reader.ReadAndParseInt("Timer List - Size Int ");
                for (int i = 0; i < timerListCount; i++)
                {
                    progressOperation.Report($"Reading Timer section {i + 1} / {timerListCount} ...");
                    try
                    {
                        ReadTimerListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Timer List - Element section number {i + 1} : {ex.Message}", ex); }
                }

                //Speech Event List - Size
                var speechEventListCount = reader.ReadAndParseInt("Speech Event List - Size Int ");
                for (int i = 0; i < speechEventListCount; i++)
                {
                    progressOperation.Report($"Reading Speech Event section {i + 1} / {speechEventListCount} ...");
                    try
                    {
                        ReadSpeechEventListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Speech Event List - Element section number {i + 1} : {ex.Message}", ex); }
                }

                //PlayerAllianceInfoVector - Size
                var playerAllianceListCount = reader.ReadAndParseInt("PlayerAllianceInfoVector - Size Int ");
                for (int i = 0; i < playerAllianceListCount; i++)
                {
                    progressOperation.Report($"Reading Player Alliance section {i + 1} / {playerAllianceListCount} ...");
                    try
                    {
                        ReadPlayerAllianceInfoVectorElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read PlayerAllianceInfoVector - Element section number {i + 1} : {ex.Message}", ex); }
                }

                //Team List - Size (InGameTeams)
                var inGameTeamListCount = reader.ReadAndParseInt("Team List - Size Int ");
                for (int i = 0; i < inGameTeamListCount; i++)
                {
                    progressOperation.Report($"Reading Team section {i + 1} / {inGameTeamListCount} ...");
                    try
                    {
                        ReadInGameTeamListElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Team List - Element section number {i + 1} : {ex.Message}", ex); }
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
                    progressOperation.Report($"Reading Group section {i + 1} / {groupCount} ...");
                    try
                    {
                        ReadGroupSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Group section number {i + 1} : {ex.Message}", ex); }
                }

                //Skip World Rules section
                var worldRulesSectionPosition = reader.CurrentPosition;
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
                    progressOperation.Report($"Reading World Crew section {i + 1} / {worldCrewListCount} ...");
                    try
                    {
                        ReadWorldCrewListElement();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Crew List - Element number {i + 1} : {ex.Message}", ex); }

                }

                //World Crew List - Size Int
                var worlArmsListCount = reader.ReadAndParseInt("World Arms List - Size Int ");
                for (int i = 0; i < worlArmsListCount; i++)
                {
                    progressOperation.Report($"Reading World Arm section {i + 1} / {worlArmsListCount} ...");
                    try
                    {
                        ReadWorldArmsListElement();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Arms List - Element number {i + 1} : {ex.Message}", ex); }

                }

                //MapText System
                ReadMapTextSystemSection();

                try
                {
                    while (!reader.EndOfStream)
                    {
                        var streamPosition = reader.BaseStream.Position;
                        var line = reader.ReadLine().Trim();
                        if (line.StartsWith("Journal Music Name"))
                        {
                            map.JournalMusic = line.GetSafeSubstring("Journal Music Name String ").Trim('\'');
                        }
                        else if (line.StartsWith("PlayEndMovie"))
                        {
                            map.PlayEndMovie = DataImportExtensions.ParseBool(line, "PlayEndMovie Bool ");
                        }
                        else if (line.StartsWith("Is Alliance Change Allowed"))
                        {
                            map.IsAllianceChangeAllowed = DataImportExtensions.ParseBool(line, "Is Alliance Change Allowed Bool ");
                        }
                        else if (line.StartsWith("Islands Make Sounds"))
                        {
                            map.IslandsMakeSounds = DataImportExtensions.ParseBool(line, "Islands Make Sounds Bool ");
                        }
                        else if (line.Equals("{"))
                        {
                            SkipSection();
                        }
                        else if (line.Equals("}"))
                        {
                            reader.BaseStream.Position = streamPosition;
                            break;
                        }
                    }
                }
                catch (Exception ex) { throw new TPMapEditorException($"Fail to read end of Game Specific section : {ex.Message}", ex); }

                var endPosition = reader.CurrentPosition;

                //go back to read world rules
                reader.CurrentPosition = worldRulesSectionPosition;
                ReadWorldRulesSection();

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
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Point section number {i + 1} : {ex.Message}", ex); }
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

        private void ReadMapTextSystemSection()
        {
            ReadSection("MapText System", () =>
            {
                //MapText Point Info - Size Int
                var mapTextPointInfoCount = reader.ReadAndParseInt("MapText Point Info - Size Int ");
                for (int i = 0; i < mapTextPointInfoCount; i++)
                {
                    progressOperation.Report($"Reading MapText Point section {i + 1} / {mapTextPointInfoCount} ...");
                    try
                    {
                        ReadMapTextPointInfoElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read MapText Point Info - Element number {i + 1} : {ex.Message}", ex); }

                }
            });
        }

        private void ReadMapTextPointInfoElementSection()
        {
            ReadSection("MapText Point Info - Element", () =>
            {
                var name = reader.ReadAndParseString("Name String ");
                var text = reader.ReadAndParseString("DisplayedText String ");
                var position = reader.ReadAndParseVector3("Position Vector3");
                var visible = reader.ReadAndParseBool("Visible Bool ");

                map.MapTextPoints.Add(new(map, name, text, position.X, position.Y, position.Z, visible));
            });
        }

        private void ReadWorldRulesSection()
        {
            ReadSection("World Rules", () =>
            {
                //Rule List Int
                var ruleListCount = reader.ReadAndParseInt("Rule List Int ");
                for(int i = 0; i < ruleListCount; i++)
                {
                    progressOperation.Report($"Reading World Rule section {i + 1} / {ruleListCount} ...");
                    try
                    {
                        var worldRule = new WorldRule(map, reader.ReadAndParseString("Rule Name String "))
                        {
                            RunOnce = reader.ReadAndParseBool("Run Once Bool ")
                        };
                        //skip IsActive, since a desactivated rule cannot be activated
                        reader.ReadLine();

                        //NumConditions Int
                        var numCondition = reader.ReadAndParseInt("NumConditions Int ");
                        for (int j = 0; j < numCondition; j++)
                        {
                            try
                            {
                                ReadConditionListSection(worldRule);
                            }
                            catch (Exception ex) { throw new TPMapEditorException($"Fail to read Condition number {j + 1} : {ex.Message}", ex); }
                        }

                        //NumActions Int
                        var numAction = reader.ReadAndParseInt("NumActions Int ");
                        for (int j = 0; j < numAction; j++)
                        {
                            try
                            {
                                ReadActionListSection(worldRule);
                            }
                            catch (Exception ex) { throw new TPMapEditorException($"Fail to read Action number {j + 1} : {ex.Message}", ex); }
                        }
                        map.WorldRules.Add(worldRule);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Rule number {i + 1} : {ex.Message}", ex); }
                }
            });
        }

        private void ReadConditionListSection(WorldRule worldRule)
        {
            ReadSection("Condition List", () =>
            {
                var condition = new Rule.RuleCondition(map);
                var typeString = reader.ReadAndParseString("Type String ");
                if (EnumExtensions.TryGetValueFromDisplayName<Enums.RuleCondition>(typeString, out var type))
                    condition.Type = type;
                else
                    throw new TPMapEditorException($"Invalid condition type {typeString}");

                while (!reader.EndOfStream)
                {
                    var pos = reader.CurrentPosition;
                    var line = reader.ReadLine() ?? throw new TPMapEditorException("Unexpected end of file when reading condition fields.");
                    line = line.Trim();

                    if (line.Equals("}"))
                    {
                        reader.CurrentPosition = pos;
                        break;
                    }

                    RuleField? targetField = null;
                    foreach (var field in condition.RuleFields)
                    {
                        if(field is RuleFieldObservableCollection ruleFieldObservableCollection && ruleFieldObservableCollection.Value != null)
                        {
                            foreach (var subField in ruleFieldObservableCollection.Value)
                            {
                                if (line.StartsWith(subField.RealLabel))
                                {
                                    targetField = subField;
                                    break;
                                }
                            }
                            if (targetField != null)
                                break; //break outer loop
                        }
                        if (line.StartsWith(field.RealLabel))
                        {
                            targetField = field;
                            break;
                        }
                    }

                    if (targetField == null)
                        throw new TPMapEditorException("Unknown condition field: " + line);

                    ProcessRuleField(targetField, line);
                    
                }
                worldRule.Conditions.Add(condition);
            });
        }

        private void ReadActionListSection(WorldRule worldRule)
        {
            ReadSection("Action List", () =>
            {
                var action = new Rule.RuleAction(map);
                var typeString = reader.ReadAndParseString("Type String ");
                if (EnumExtensions.TryGetValueFromDisplayName<Enums.RuleAction>(typeString, out var type))
                    action.Type = type;
                else
                    throw new TPMapEditorException($"Invalid action type {typeString}");

                while (!reader.EndOfStream)
                {
                    var pos = reader.CurrentPosition;
                    var line = reader.ReadLine() ?? throw new TPMapEditorException("Unexpected end of file when reading action fields.");
                    line = line.Trim();

                    if (line.Equals("}"))
                    {
                        reader.CurrentPosition = pos;
                        break;
                    }

                    RuleField? targetField = null;
                    foreach (var field in action.RuleFields)
                    {
                        if (field is RuleFieldObservableCollection ruleFieldObservableCollection && ruleFieldObservableCollection.Value != null)
                        {
                            foreach (var subField in ruleFieldObservableCollection.Value)
                            {
                                if (line.StartsWith(subField.RealLabel))
                                {
                                    targetField = subField;
                                    break;
                                }
                            }
                            if (targetField != null)
                                break; //break outer loop
                        }
                        if (field.RealLabel != null && line.StartsWith(field.RealLabel))
                        {
                            targetField = field;
                            break;
                        }
                    }

                    if (targetField == null)
                        throw new TPMapEditorException("Unknown action field: " + line);

                    ProcessRuleField(targetField, line);

                }
                worldRule.Actions.Add(action);
            });
        }

        private void ProcessRuleField(RuleField targetField, string line)
        {
            //Do all the RuleField types from Rule folder
            switch (targetField)
            {
                case RuleFieldBool rfBool:
                    {
                        var stringValue = DataImportExtensions.ParseString(rfBool.RealLabel + " ", line);
                        rfBool.Value = bool.Parse(stringValue);
                    }
                    break;

                case RuleFieldInt rfInt:
                    rfInt.Value = DataImportExtensions.ParseInt(rfInt.RealLabel + " ", line);
                    break;

                case RuleFieldDouble rfDouble:
                    rfDouble.Value = DataImportExtensions.ParseDouble(rfDouble.RealLabel + " ", line);
                    break;

                case RuleFieldString rfString:
                    rfString.Value = DataImportExtensions.ParseString(rfString.RealLabel + " ", line);
                    break;

                case RuleFieldFlag rfFlag:
                    {
                        var flagName = DataImportExtensions.ParseString(rfFlag.RealLabel + " ", line);
                        var flag = map.Flags.FirstOrDefault(f => f.Name == flagName);
                        if (!rfFlag.IsOptional && flag == null)
                            progress.Report($"Warning: Flag '{flagName}' does not exist.");
                        rfFlag.Value = flag;
                    }
                    break;

                case RuleFieldGroup rfGroup:
                    {
                        var groupName = DataImportExtensions.ParseString(rfGroup.RealLabel + " ", line);
                        var group = map.Groups.FirstOrDefault(g => g.Name == groupName);
                        if (!rfGroup.IsOptional && group == null)
                        {
                            progress.Report($"Warning: Group '{groupName}' not found.");
                        }
                        rfGroup.Value = group;
                    }
                    break;

                case RuleFieldGroupUnit rfGroupUnit:
                    {
                        var name = DataImportExtensions.ParseString(rfGroupUnit.RealLabel + " ", line);
                        var group = map.Groups.FirstOrDefault(g => g.Name == name);
                        if (group != null)
                        {
                            rfGroupUnit.Value = group;
                            rfGroupUnit.IsGroupUnitUnit = false;
                        }
                        else
                        {
                            var unitName = name.Split(',').Last();
                            var unit = map.ShipUnits.FirstOrDefault(u => u.Name == unitName);
                            if (unit != null)
                            {
                                rfGroupUnit.Value = unit;
                                rfGroupUnit.IsGroupUnitUnit = true;
                            }
                            else
                            {
                                progress.Report($"Warning: Group/Unit '{name}' not found.");
                                rfGroupUnit.Value = null;
                            }
                        }
                    }
                    break;

                case RuleFieldPlayer rfPlayer:
                    {
                        var playerName = DataImportExtensions.ParseString(rfPlayer.RealLabel + " ", line);
                        var player = map.Players.FirstOrDefault(p => p.Name == playerName);
                        if (!rfPlayer.IsOptional && player == null)
                        {
                            progress.Report($"Warning: Player '{playerName}' not found.");
                        }
                        rfPlayer.Value = player;
                    }
                    break;

                case RuleFieldTeam rfTeam:
                    {
                        var teamName = DataImportExtensions.ParseString(rfTeam.RealLabel + " ", line);
                        var team = map.InGameTeams.FirstOrDefault(t => t.RealName == teamName);
                        if (!rfTeam.IsOptional && team == null)
                        {
                            progress.Report($"Warning: Team '{teamName}' not found.");
                        }
                        rfTeam.Value = team;
                    }
                    break;

                case RuleFieldTimer rfTimer:
                    {
                        var timerName = DataImportExtensions.ParseString(rfTimer.RealLabel + " ", line);
                        var timer = map.Timers.FirstOrDefault(t => t.Name == timerName);
                        if (!rfTimer.IsOptional && timer == null)
                        {
                            progress.Report($"Warning: Timer '{timerName}' not found.");
                        }
                        rfTimer.Value = timer;
                    }
                    break;

                case RuleFieldWorldObject rfWorldObj:
                    {
                        var id = DataImportExtensions.ParseInt(rfWorldObj.RealLabel + " ", line);
                        var wot = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (!rfWorldObj.IsOptional && wot == null)
                        {
                            progress.Report($"Warning: WorldObject with id '{id}' not found.");
                        }
                        rfWorldObj.Value = wot;
                    }
                    break;

                case RuleFieldUnit rfUnit:
                    {
                        var unitName = DataImportExtensions.ParseString(rfUnit.RealLabel + " ", line);
                        var unit = map.ShipUnits.FirstOrDefault(u => u.Name == unitName);
                        if (!rfUnit.IsOptional && unit == null)
                        {
                            progress.Report($"Warning: Unit '{unitName}' not found.");
                        }
                        rfUnit.Value = unit;
                    }
                    break;

                case RuleFieldWorldPointSet rfPointSet:
                    {
                        var name = DataImportExtensions.ParseString(rfPointSet.RealLabel + " ", line);
                        var set = map.WorldPointSets.FirstOrDefault(s => s.Name == name);
                        if (!rfPointSet.IsOptional && set == null)
                        {
                            progress.Report($"Warning: WorldPointSet '{name}' not found.");
                        }
                        rfPointSet.Value = set;
                    }
                    break;

                case RuleFieldWorldPolygon rfPolygon:
                    {
                        var name = DataImportExtensions.ParseString(rfPolygon.RealLabel + " ", line);
                        var poly = map.WorldPolygons.FirstOrDefault(p => p.Name == name);
                        if (!rfPolygon.IsOptional && poly == null)
                        {
                            progress.Report($"Warning: WorldPolygon '{name}' not found.");
                        }
                        rfPolygon.Value = poly;
                    }
                    break;

                case RuleFieldMapTextPoint rfMapText:
                    {
                        var name = DataImportExtensions.ParseString(rfMapText.RealLabel + " ", line);
                        var mt = map.MapTextPoints.FirstOrDefault(m => m.Name == name);
                        if (!rfMapText.IsOptional && mt == null)
                        {
                            progress.Report($"Warning: MapTextPoint '{name}' not found.");
                        }
                        rfMapText.Value = mt;
                    }
                    break;

                case RuleFieldSpeechEvent rfSpeech:
                    {
                        var name = DataImportExtensions.ParseString(rfSpeech.RealLabel + " ", line);
                        var se = map.SpeechEvents.FirstOrDefault(s => s.Name == name);
                        if (!rfSpeech.IsOptional && se == null)
                        {
                            progress.Report($"Warning: SpeechEvent '{name}' not found.");
                        }
                        rfSpeech.Value = se;
                    }
                    break;

                case RuleFieldObjectivePoint rfObjPoint:
                    {
                        var name = DataImportExtensions.ParseString(rfObjPoint.RealLabel + " ", line);
                        var op = map.ObjectivePoints.FirstOrDefault(o => o.Name == name);
                        if (!rfObjPoint.IsOptional && op == null)
                        {
                            progress.Report($"Warning: ObjectivePoint '{name}' not found.");
                        }
                        rfObjPoint.Value = op;
                    }
                    break;

                case RuleFieldObjectiveTask rfObjTask:
                    {
                        var name = DataImportExtensions.ParseString(rfObjTask.RealLabel + " ", line);
                        var ot = map.ObjectiveTasks.FirstOrDefault(o => o.Name == name);
                        if (!rfObjTask.IsOptional && ot == null)
                        {
                            progress.Report($"Warning: ObjectiveTask '{name}' not found.");
                        }
                        rfObjTask.Value = ot;
                    }
                    break;

                case RuleFieldWaypointPath rfPath:
                    {
                        var name = DataImportExtensions.ParseString(rfPath.RealLabel + " ", line);
                        var wp = map.WaypointPaths.FirstOrDefault(w => w.Name == name);
                        if (!rfPath.IsOptional && wp == null)
                        {
                            progress.Report($"Warning: WaypointPath '{name}' not found.");
                        }
                        rfPath.Value = wp;
                    }
                    break;

                case RuleFieldWorldObjectType rfWotType:
                    {
                        var raw = DataImportExtensions.ParseString(rfWotType.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<KillableWorldObjectType>(raw, out var val))
                            rfWotType.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid KillableWorldObjectType '{raw}'.");
                            rfWotType.Value = KillableWorldObjectType.Ship;
                        }
                    }
                    break;

                case RuleFieldFormationType rfFormation:
                    {
                        var raw = DataImportExtensions.ParseString(rfFormation.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<FormationType>(raw, out var val))
                            rfFormation.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid FormationType '{raw}'.");
                            rfFormation.Value = FormationType.None;
                        }
                    }
                    break;

                case RuleFieldBannerType rfBanner:
                    {
                        var raw = DataImportExtensions.ParseString(rfBanner.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<BannerType>(raw, out var val))
                            rfBanner.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid BannerType '{raw}'.");
                            rfBanner.Value = BannerType.NoBanner;
                        }
                    }
                    break;

                case RuleFieldEquivalence rfEquiv:
                    {
                        var raw = DataImportExtensions.ParseString(rfEquiv.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<Equivalence>(raw, out var val))
                            rfEquiv.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid Equivalence '{raw}'.");
                            rfEquiv.Value = Equivalence.EqualTo;
                        }
                    }
                    break;

                case RuleFieldAiStance rfAiStance:
                    {
                        var raw = DataImportExtensions.ParseString(rfAiStance.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<AiStance>(raw, out var val))
                            rfAiStance.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid AiStance '{raw}'.");
                            rfAiStance.Value = AiStance.AISTANCE;
                        }
                    }
                    break;

                case RuleFieldCrewSkillLevel rfCrewSkill:
                    {
                        var raw = DataImportExtensions.ParseString(rfCrewSkill.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<CrewSkillLevel>(raw, out var val))
                            rfCrewSkill.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid CrewSkillLevel '{raw}'.");
                            rfCrewSkill.Value = CrewSkillLevel.Green;
                        }
                    }
                    break;

                case RuleFieldFollowMode rfFollow:
                    {
                        var raw = DataImportExtensions.ParseString(rfFollow.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<FollowMode>(raw, out var val))
                            rfFollow.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid FollowMode '{raw}'.");
                            rfFollow.Value = FollowMode.ToEnd;
                        }
                    }
                    break;

                case RuleFieldVitalSection rfVital:
                    {
                        var raw = DataImportExtensions.ParseString(rfVital.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<VitalSection>(raw, out var val))
                            rfVital.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid VitalSection '{raw}'.");
                            rfVital.Value = VitalSection.VitalToMission;
                        }
                    }
                    break;

                case RuleFieldFlagTexture rfFlagTex:
                    rfFlagTex.Value = DataImportExtensions.ParseString(rfFlagTex.RealLabel + " ", line);
                    break;

                case RuleFieldEffect rfEffect:
                    rfEffect.Value = DataImportExtensions.ParseString(rfEffect.RealLabel + " ", line);
                    break;

                case RuleFieldShipName rfShipName:
                    rfShipName.Value = DataImportExtensions.ParseString(rfShipName.RealLabel + " ", line);
                    break;

                case RuleFieldSinglePlayerMission rfSPMission:
                    rfSPMission.Value = DataImportExtensions.ParseString(rfSPMission.RealLabel + " ", line);
                    break;

                case RuleFieldDialogueAudio rfDialogue:
                    rfDialogue.Value = DataImportExtensions.ParseString(rfDialogue.RealLabel + " ", line);
                    break;

                case RuleFieldGuiTexture rfGui:
                    rfGui.Value = DataImportExtensions.ParseString(rfGui.RealLabel + " ", line);
                    break;

                case RuleFieldInGameMessage rfInGameMsg:
                    rfInGameMsg.Value = DataImportExtensions.ParseString(rfInGameMsg.RealLabel + " ", line);
                    break;

                case RuleFieldMusic rfMusic:
                    rfMusic.Value = DataImportExtensions.ParseString(rfMusic.RealLabel + " ", line);
                    break;

                case RuleFieldObservableCollection rfCollection:
                    {
                        var stringValue = DataImportExtensions.ParseString(rfCollection.RealLabel + " ", line);
                        rfCollection.IsShown = bool.Parse(stringValue);
                    }
                    break;

                default:
                    // unknown/unsupported RuleField type
                    throw new NotImplementedException("RuleField type not implemented: " + targetField.GetType().Name);

            }
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
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Point Info - Element section number {i + 1} : {ex.Message}", ex); }
                }

                //Objective Task Array - Size Int
                var objectiveTaskListCount = reader.ReadAndParseInt("Objective Task Array - Size Int ");
                for (int i = 0; i < objectiveTaskListCount; i++)
                {
                    try
                    {
                        ReadObjectiveTaskArrayElementSection();
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Objective Task Array - Element section number {i + 1} : {ex.Message}", ex); }
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
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read Page Info - Element section number {i + 1} : {ex.Message}", ex); }
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
