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
            try
            {
                lock (_lock)
                {
                    //skip comment line
                    reader.ReadLine();
                    ReadWorldInfoSection();
                    ReadGameSection();
                    ReadWorldSection();
                    map.ReorganizeWorldObjectIds();
                }
                progressOperation.Report($"Map import completed in {(DateTime.Now - time).TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                progressOperation.Report("Map import failed.");
                progress.Report($"An error has occured.\n{ex.Message}");
                map.Reset();
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
                    throw new TPMapEditorException($"{sectionName} section is invalid.");
            }
            catch (TPMapEditorException) { throw; }
            catch (Exception ex) { throw new Exception($"Fail to read {sectionName} section: {ex.Message}", ex); }
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
                    player = new(map, playerName, 0, 0, 0, 0, Colors.White) { IsPlayable = false };
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
                player.FormationTypeStart = (FormationType)reader.ReadAndParseInt("FormationType Int ");

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
                var type = WorldObjectType.WotTypes.FirstOrDefault((t)=>t.Name == typeString);
                var isValidWorldObject = true;
                if (type is null)
                {
                    progress.Report($"Warning: Unokwn type '{typeString}' for WorldObject #{id}.");
                    isValidWorldObject = false;
                }

                var worldObject = new WorldObject(map, type, 0, 0, 0) { Id = id };

                try
                {
                    ReadWorldObjectStateSection(worldObject);
                }
                catch (TPMapEditorException) { throw; }
                catch(Exception ex) { throw new TPMapEditorException($"Fail to read state section of WorldObject #{worldObject.Id}", ex); }
                
                if(isValidWorldObject)
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
                worldObject.HasState = reader.ReadAndParseBool("HasState Bool ");

                //Position
                var position = reader.ReadAndParseVector3("Position Vector3");
                worldObject.X = position.X;
                worldObject.Y = position.Y;
                worldObject.Z = position.Z;

                //Orientation
                var (rotationX, rotationY, rotationZ) = reader.ReadAndParseMatrix33("Orientation Matrix33");
                var rotationEulerXYZ = MathUtils.Matrix33ToEulerXYZ(rotationX, rotationY, rotationZ);
                worldObject.XRotation = rotationEulerXYZ.X;
                worldObject.YRotation = rotationEulerXYZ.Y;
                worldObject.ZRotation = rotationEulerXYZ.Z;

                //PlayerIndex
                var playerIndex = reader.ReadAndParseInt("PlayerIndex Int ");
                if (playerIndex >= 0)
                {
                    try { worldObject.Player = map.Players[playerIndex]; }
                    catch { progress.Report($"Warning: PlayerIndex of world object #{worldObject.Id} is incorrect."); }
                }

                worldObject.AIEntity = worldObject.RenderEntity = worldObject.PhysicsEntity = worldObject.CollisionEntity = worldObject.CustomInfoEntity = string.Empty;

                //# AIEntity
                var firstLine = reader.ReadLine();
                if (firstLine.Trim().StartsWith("#"))
                    worldObject.AIEntity += reader.ReadLine() + Environment.NewLine;
                else
                    worldObject.AIEntity += firstLine + Environment.NewLine;

                try
                {
                    //SkipNamedSection("State");
                    SkipNamedSection("State", (line) =>
                    {
                        worldObject.AIEntity += line + Environment.NewLine;
                    });
                }
                catch { }
                worldObject.AIEntity = worldObject.AIEntity.TrimEnd();

                reader.ReadLine(); //skip line between state section (it's normally a comment line)

                //# RenderEntity
                worldObject.RenderEntity += reader.ReadLine() + Environment.NewLine;
                try
                {
                    //SkipNamedSection("State");
                    SkipNamedSection("State", (line) =>
                    {
                        worldObject.RenderEntity += line + Environment.NewLine;
                    });
                }
                catch { }
                worldObject.RenderEntity = worldObject.RenderEntity.TrimEnd();

                reader.ReadLine(); //skip line between state section (it's normally a comment line)

                //# PhysicsEntity
                worldObject.PhysicsEntity += reader.ReadLine() + Environment.NewLine;
                try
                {
                    //SkipNamedSection("State");
                    SkipNamedSection("State", (line) =>
                    {
                        worldObject.PhysicsEntity += line + Environment.NewLine;
                    });
                }
                catch { }
                worldObject.PhysicsEntity = worldObject.PhysicsEntity.TrimEnd();

                reader.ReadLine(); //skip line between state section (it's normally a comment line)

                //# CollisionEntity
                worldObject.CollisionEntity += reader.ReadLine() + Environment.NewLine;
                try
                {
                    //SkipNamedSection("State");
                    SkipNamedSection("State", (line) =>
                    {
                        worldObject.CollisionEntity += line + Environment.NewLine;
                    });
                }
                catch { }
                worldObject.CollisionEntity = worldObject.CollisionEntity.TrimEnd();

                reader.ReadLine(); //skip line between state section (it's normally a comment line)

                //# CustomInfoEntity
                worldObject.CustomInfoEntity += reader.ReadLine() + Environment.NewLine;
                try
                {
                    //SkipNamedSection("State");
                    SkipNamedSection("State", (line) =>
                    {
                        worldObject.CustomInfoEntity += line + Environment.NewLine;
                    });
                }
                catch { }
                worldObject.CustomInfoEntity = worldObject.CustomInfoEntity.TrimEnd();

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

                if (string.IsNullOrEmpty(map.Skybox)) progress.Report($"Warning: skybox '{skyboxMeshString}' is not a valid mesh.");

                //Ambient Light
                map.AmbientLightColor = reader.ReadAndParseColor("Ambient Light Colour");

                var roofLightOrientationVector = reader.ReadAndParseVector3("Vector for roof light orientation Vector3");
                (int rloYaw, int rloPitch) = MathUtils.Vector3ToYawPitch(roofLightOrientationVector);
                map.RoofLightOrientationYaw = rloYaw;
                map.RoofLightOrientationPitch = rloPitch;

                //Hemispherical floor light color
                map.FloorLightColor = reader.ReadAndParseColor("Hemispherical floor light color Colour");

                //Hemispherical roof light color
                map.RoofLightColor = reader.ReadAndParseColor("Hemispherical roof light color Colour");

                //World Initialized State (skip for now)
                reader.ReadLine();

                //World Buffer
                map.WorldBuffer = reader.ReadAndParseDouble("World Buffer Size Float ");

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
                    player.InGameTeam = player.TeamIndex < 0 ? null : map.InGameTeams.ElementAtOrDefault(player.TeamIndex);
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
                            map.JournalMusic = DataImportExtensions.ParseString("Journal Music Name String ", line);
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
                var worldPoint = new WorldPoint(worldPointSet, 0, 0, 0, 0)
                {
                    Magnitude = reader.ReadAndParseDouble("World Point Magnitude Float ")
                };

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

                var eulerXYZ = MathUtils.Matrix33ToEulerXYZ(orientationCross, orientationForward, orientationUp);

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
                    var worldObject = map.WorldObjects.FirstOrDefault((wot) => wot.Id == worldObjectId);
                    if(worldObject == null)
                        progress.Report($"Warning: No WorldObject found with id {worldObjectId} for group {group.Name}.");
                    else
                        worldObject.Group = group;
                }

                map.Groups.Add(group);
            });
        }

        private void ReadWorldCrewListElement()
        {
            var crewName = reader.ReadAndParseString("World Crew List - Element String ");
            var wotGridItem = WorldObjectType.WotTypes.FirstOrDefault((type) => type.Name == crewName);
            if (wotGridItem != null)
            {
                if(wotGridItem.CustomInfoDefinition == CustomInfoDefinition.CrewCustomInfoFactory)
                {
                    map.WorldCrews.Add(wotGridItem);
                }
                else
                    progress.Report($"Warning: World object {crewName} is not a valid crew member.");
            }
            else
                progress.Report($"Warning: World object {crewName} does not exists in your TPGame folder.");
        }

        private void ReadWorldArmsListElement()
        {
            var gunName = reader.ReadAndParseString("World Arms List - Element String ");
            var wotGridItem = WorldObjectType.WotTypes.FirstOrDefault((type) => type.Name == gunName);
            if (wotGridItem != null)
            {
                if (wotGridItem.CustomInfoDefinition == CustomInfoDefinition.GunCustomInfoFactory)
                {
                    map.WorldArms.Add(wotGridItem);
                }
                else
                    progress.Report($"Warning: World object {gunName} is not a valid crew member.");
            }
            else
                progress.Report($"Warning: World object {gunName} does not exists in your TPGame folder.");
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
                var isValidCondition = true;
                if (EnumExtensions.TryGetValueFromDisplayName<Enums.RuleCondition>(typeString, out var type))
                    condition.Type = type;
                else
                {
                    progress.Report($"Warning: Invalid condition type {typeString}");
                    isValidCondition = false;
                }

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
                        progress.Report($"Warning: Unknown condition field for : " );
                    else
                        ProcessRuleField(targetField, line);                    
                }
                if (isValidCondition)
                    worldRule.Conditions.Add(condition);
            });
        }

        private void ReadActionListSection(WorldRule worldRule)
        {
            ReadSection("Action List", () =>
            {
                var action = new Rule.RuleAction(map);
                var typeString = reader.ReadAndParseString("Type String ");
                var isValidAction = true;
                if (EnumExtensions.TryGetValueFromDisplayName<Enums.RuleAction>(typeString, out var type))
                    action.Type = type;
                else
                {
                    progress.Report($"Warning: Invalid action type {typeString}");
                    isValidAction = false;
                }

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
                    else
                        ProcessRuleField(targetField, line);
                }
                if (isValidAction)
                    worldRule.Actions.Add(action);
            });
        }

        private void ProcessRuleField(RuleField targetField, string line)
        {
            switch (targetField)
            {
                case RuleFieldAiStance ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<AiStance>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid AiStance '{raw}'.");
                            ruleField.Value = AiStance.AISTANCE;
                        }
                    }
                    break;

                case RuleFieldBannerType ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<BannerType>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid BannerType '{raw}'.");
                            ruleField.Value = BannerType.NoBanner;
                        }
                    }
                    break;

                case RuleFieldBool ruleField:
                    {
                        var stringValue = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        ruleField.Value = bool.Parse(stringValue);
                    }
                    break;

                case RuleFieldCrewSkillLevel ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<CrewSkillLevel>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid CrewSkillLevel '{raw}'.");
                            ruleField.Value = CrewSkillLevel.Green;
                        }
                    }
                    break;

                case RuleFieldDialogueAudio ruleField:
                    var dialogue = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!AppSettings.DialogueFilesList.Contains(dialogue))
                    {
                        progress.Report($"Warning: Dialogue audio file '{dialogue}' is not in the predefined list.");
                    }
                    ruleField.Value = dialogue;
                    break;

                case RuleFieldDouble ruleField:
                    ruleField.Value = DataImportExtensions.ParseDouble(ruleField.RealLabel + " ", line);
                    break;

                case RuleFieldEffect ruleField:
                    var effect = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!AppSettings.Effects.Contains(effect))
                    {
                        progress.Report($"Warning: Effect '{effect}' is not in the predefined list.");
                    }
                    ruleField.Value = effect;
                    break;

                case RuleFieldEquivalence ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<Equivalence>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid Equivalence '{raw}'.");
                            ruleField.Value = Equivalence.EqualTo;
                        }
                    }
                    break;

                case RuleFieldFlag ruleField:
                    {
                        var flagName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var flag = map.Flags.FirstOrDefault(f => f.Name == flagName);
                        if (flag == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: Flag '{flagName}' does not exist.");
                            }
                        }
                        ruleField.Value = flag;
                    }
                    break;

                case RuleFieldFlagTexture ruleField:
                    var flagTex = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!AppSettings.FlagTextures.Contains(flagTex))
                    {
                        progress.Report($"Warning: Flag texture '{flagTex}' is not in the predefined list.");
                    }
                    ruleField.Value = flagTex;
                    break;

                case RuleFieldFollowMode ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<FollowMode>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid FollowMode '{raw}'.");
                            ruleField.Value = FollowMode.ToEnd;
                        }
                    }
                    break;

                case RuleFieldFormationType ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<FormationType>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid FormationType '{raw}'.");
                            ruleField.Value = FormationType.None;
                        }
                    }
                    break;

                case RuleFieldGroup ruleField:
                    {
                        var groupName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        Group group;
                        if (groupName.Equals(Group.DefaultName))
                            group = Group.DefaultGroup;
                        else
                            group = map.Groups.FirstOrDefault(g => g.Name == groupName);
                        if (group == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: Group '{groupName}' not found.");
                            }
                        }
                        ruleField.Value = group;
                    }
                    break;

                case RuleFieldGroupUnit ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        Group group;
                        if (name.Equals(Group.DefaultName))
                            group = Group.DefaultGroup;
                        else
                            group = map.Groups.FirstOrDefault(g => g.Name == name);
                        if (group != null)
                        {
                            ruleField.Value = group;
                            ruleField.IsGroupUnitUnit = false;
                        }
                        else
                        {
                            var nameSplit = name.Split(',');
                            var unitGroupName = nameSplit.First();
                            var unitName = nameSplit.Last();
                            Group unitGroup;
                            if (unitGroupName.Equals(Group.DefaultName))
                                unitGroup = Group.DefaultGroup;
                            else
                                unitGroup = map.Groups.FirstOrDefault(g => g.Name == unitGroupName);
                            if (unitGroup != null)
                            {
                                ShipUnit unit;
                                if (unitName.Equals(ShipUnit.DefaultName))
                                    unit = ShipUnit.DefaultShipUnit;
                                else
                                    unit = map.ShipUnits.FirstOrDefault(u => u.Name == unitName);
                                if (unit != null)
                                {
                                    ruleField.SelectedGroup = unitGroup;
                                    ruleField.Value = unit;
                                    ruleField.IsGroupUnitUnit = true;
                                }
                                else
                                {
                                    progress.Report($"Warning: Unit '{unitName}' not found.");
                                }
                            }
                            else
                            {
                                if (!ruleField.IsOptional) progress.Report($"Warning: Group/Unit '{name}' not found.");
                                ruleField.Value = null;
                            }
                        }
                    }
                    break;

                case RuleFieldGuiTexture ruleField:
                    var guiTexture = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!AppSettings.GuiTextures.Contains(guiTexture))
                    {
                        progress.Report($"Warning: GUI base texture '{guiTexture}' is not in the predefined list.");
                    }
                    ruleField.Value = guiTexture;
                    break;

                case RuleFieldInGameMessage ruleField:
                    var inGameMsg = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!StringDictionnary.InGameMessagesDictionnary.ContainsKey(inGameMsg))
                    {
                        progress.Report($"Warning: In-Game Message '{inGameMsg}' is not in the predefined list.");
                    }
                    ruleField.Value = inGameMsg;
                    break;

                case RuleFieldInt rfInt:
                    rfInt.Value = DataImportExtensions.ParseInt(rfInt.RealLabel + " ", line);
                    break;

                case RuleFieldMapTextPoint ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var mt = map.MapTextPoints.FirstOrDefault(m => m.Name == name);
                        if (mt == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: MapTextPoint '{name}' not found.");
                            }
                        }
                        ruleField.Value = mt;
                    }
                    break;

                case RuleFieldMusic ruleField:
                    var music = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!AppSettings.Musics.Contains(music))
                    {
                        progress.Report($"Warning: Music '{music}' is not in the predefined list.");
                    }
                    ruleField.Value = music;
                    break;

                case RuleFieldObservableCollection rfCollection:
                    {
                        var stringValue = DataImportExtensions.ParseString(rfCollection.RealLabel + " ", line);
                        rfCollection.IsShown = bool.Parse(stringValue);
                    }
                    break;

                case RuleFieldObjectivePoint ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (name.Equals(ObjectivePoint.DefaultName))
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: ObjectivePoint '{name}' is the default name.\n'{line}', position {reader.CurrentPosition}");
                            }
                            ruleField.Value = ObjectivePoint.DefaultObjectivePoint;
                        }
                        else
                        {
                            var op = map.ObjectivePoints.FirstOrDefault(o => o.Name == name);
                            if (op == null)
                            {
                                if (!ruleField.IsOptional)
                                {
                                    progress.Report($"Warning: ObjectivePoint '{name}' not found.");
                                }
                            }
                            ruleField.Value = op;
                        }
                    }
                    break;

                case RuleFieldObjectiveTask ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var ot = map.ObjectiveTasks.FirstOrDefault(o => o.Name == name);
                        if (ot == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: ObjectiveTask '{name}' not found.");
                            }
                        }
                        ruleField.Value = ot;
                    }
                    break;

                case RuleFieldPlayer ruleField:
                    {
                        var playerName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (playerName.Equals(Player.DefaultName))
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: Player '{playerName}' is the default name.\n'{line}', position {reader.CurrentPosition}");
                            }
                            ruleField.Value = Player.DefaultPlayer;
                        }
                        else
                        {
                            var player = map.Players.FirstOrDefault(p => p.Name == playerName);
                            if (player == null)
                            {
                                if (!ruleField.IsOptional)
                                {
                                    progress.Report($"Warning: Player '{playerName}' not found.");
                                }
                            }
                            ruleField.Value = player;
                        }
                    }
                    break;

                case RuleFieldShipName ruleField:
                    var shipName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (!StringDictionnary.ShipNames.ContainsKey(shipName))
                    {
                        progress.Report($"Warning: Ship name '{shipName}' is not in the predefined list.");
                    }
                    ruleField.Value = shipName;
                    break;

                case RuleFieldShipUnitName ruleField:
                    var shipUnitName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    if (string.IsNullOrEmpty(shipUnitName))
                    {
                        shipUnitName = NamedMapObject.GenerateName("Ship", map.ShipUnits);
                        progress.Report($"Warning: Empty unit name replaced by {shipUnitName}.");
                    }
                    if(ruleField.Value!=null)
                        ruleField.Value.Name = shipUnitName;
                    break;

                case RuleFieldSinglePlayerMission ruleField:
                    var spMission = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    var spMissionFromList = AppSettings.SinglePlayerMissions.FirstOrDefault(s => s.Equals(spMission, StringComparison.OrdinalIgnoreCase));
                    if (spMissionFromList == null)
                    {
                        progress.Report($"Warning: Single Player Mission '{spMission}' is not in the predefined list.");
                    }
                    ruleField.Value = spMissionFromList;
                    break;

                case RuleFieldSpeechEvent ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var se = map.SpeechEvents.FirstOrDefault(s => s.Name == name);
                        if (se == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: SpeechEvent '{name}' not found.");
                            }
                        }
                        ruleField.Value = se;
                    }
                    break;

                case RuleFieldString ruleField:
                    ruleField.Value = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                    break;

                case RuleFieldTeam ruleField:
                    {
                        var teamName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var team = map.InGameTeams.FirstOrDefault(t => t.RealName == teamName);
                        if (team == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: In game team '{teamName}' not found.");
                            }
                        }
                        ruleField.Value = team;
                    }
                    break;

                case RuleFieldTimer ruleField:
                    {
                        var timerName = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var timer = map.Timers.FirstOrDefault(t => t.Name == timerName);
                        if (timer == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: Timer '{timerName}' not found.");
                            }
                        }
                        ruleField.Value = timer;
                    }
                    break;

                case RuleFieldUnit ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var nameSplit = name.Split(',');
                        var unitGroupName = nameSplit.First();
                        var unitName = nameSplit.Last();
                        Group unitGroup;
                        if (unitGroupName.Equals(Group.DefaultName))
                            unitGroup = Group.DefaultGroup;
                        else
                            unitGroup = map.Groups.FirstOrDefault(g => g.Name == unitGroupName);
                        if (unitGroup != null)
                        {
                            ShipUnit unit;
                            if (unitName.Equals(ShipUnit.DefaultName))
                                unit = ShipUnit.DefaultShipUnit;
                            else
                                unit = map.ShipUnits.FirstOrDefault(u => u.Name == unitName);
                            if (unit != null)
                            {
                                ruleField.SelectedGroup = unitGroup;
                                ruleField.Value = unit;
                            }
                            else
                            {
                                progress.Report($"Warning: Unit '{unitName}' not found.");
                            }
                        }
                        else
                        {
                            if (!ruleField.IsOptional) progress.Report($"Warning: Group/Unit '{name}' not found.");
                            ruleField.Value = null;
                        }
                    }
                    break;

                case RuleFieldVitalSection ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<VitalSection>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid VitalSection '{raw}'.");
                            ruleField.Value = VitalSection.VitalToMission;
                        }
                    }
                    break;

                case RuleFieldWaypointPath ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (WaypointPath.DefaultName.Contains(name))
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WaypointPath '{name}' is a default name.\n'{line}', position {reader.CurrentPosition}");
                            }
                            ruleField.Value = WaypointPath.DefaultWaypointPath;
                        }
                        else
                        {
                            var obj = map.WaypointPaths.FirstOrDefault(o => o.Name == name);
                            if (obj == null)
                            {
                                if (!ruleField.IsOptional)
                                {
                                    progress.Report($"Warning: WaypointPath '{name}' not found.");
                                }
                            }
                            ruleField.Value = obj;
                        }
                    }
                    break;

                case RuleFieldWorldObjectEtheriumCurrent ruleField:
                    {
                        var id = DataImportExtensions.ParseInt(ruleField.RealLabel + " ", line);
                        var worldObject = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (worldObject == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldObject with id '{id}' not found.");
                            }
                        }
                        else if(worldObject.Type.CustomInfoDefinition != CustomInfoDefinition.EtheriumCurrentCustomInfoFactory)
                        {
                            progress.Report($"Warning: WorldObject '{worldObject}' is used as etherium current.");
                        }
                        ruleField.Value = worldObject;
                    }
                    break;

                case RuleFieldWorldObjectIsland ruleField:
                    {
                        var id = DataImportExtensions.ParseInt(ruleField.RealLabel + " ", line);
                        var wot = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (wot == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldObject with id '{id}' not found.");
                            }
                        }
                        else if (wot.Type.CustomInfoDefinition != CustomInfoDefinition.IslandCustomInfoFactory)
                        {
                            progress.Report($"Warning: WorldObject '{wot}' is used as island.");
                        }
                        ruleField.Value = wot;
                    }
                    break;

                case RuleFieldWorldObjectNebula ruleField:
                    {
                        var id = DataImportExtensions.ParseInt(ruleField.RealLabel + " ", line);
                        var wot = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (wot == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldObject with id '{id}' not found.");
                            }
                        }
                        else if (wot.Type.CustomInfoDefinition != CustomInfoDefinition.NebulaCustomInfoFactory)
                        {
                            progress.Report($"Warning: WorldObject '{wot}' is used as nebula.");
                        }
                        ruleField.Value = wot;
                    }
                    break;

                case RuleFieldWorldObjectShip ruleField:
                    {
                        var id = DataImportExtensions.ParseInt(ruleField.RealLabel + " ", line);
                        var wot = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (wot == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldObject with id '{id}' not found.");
                            }
                        }
                        else if (wot.Type.CustomInfoDefinition != CustomInfoDefinition.ShipCustomInfoFactory)
                        {
                            progress.Report($"Warning: WorldObject '{wot}' is used as ship.");
                        }
                        ruleField.Value = wot;
                    }
                    break;

                case RuleFieldWorldObject ruleField:
                    {
                        var id = DataImportExtensions.ParseInt(ruleField.RealLabel + " ", line);
                        var wot = map.WorldObjects.FirstOrDefault(w => w.Id == id);
                        if (wot == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldObject with id '{id}' not found.");
                            }
                        }
                        ruleField.Value = wot;
                    }
                    break;

                case RuleFieldWorldObjectType ruleField:
                    {
                        var raw = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (EnumExtensions.TryGetValueFromDisplayName<KillableWorldObjectType>(raw, out var val))
                            ruleField.Value = val;
                        else
                        {
                            progress.Report($"Warning: Invalid KillableWorldObjectType '{raw}'.");
                            ruleField.Value = KillableWorldObjectType.Ship;
                        }
                    }
                    break;

                case RuleFieldWorldPointSet ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        if (name.Equals(WorldPointSet.DefaultName))
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldPointSet '{name}' is the default name.\n'{line}', position {reader.CurrentPosition}");
                            }
                            ruleField.Value = WorldPointSet.DefaultWorldPointSet;
                        }
                        else
                        {
                            var obj = map.WorldPointSets.FirstOrDefault(o => o.Name == name);
                            if (obj == null)
                            {
                                if (!ruleField.IsOptional)
                                {
                                    progress.Report($"Warning: WorldPointSet '{name}' not found.");
                                }
                            }
                            ruleField.Value = obj;
                        }
                    }
                    break;

                case RuleFieldWorldPolygon ruleField:
                    {
                        var name = DataImportExtensions.ParseString(ruleField.RealLabel + " ", line);
                        var poly = map.WorldPolygons.FirstOrDefault(p => p.Name == name);
                        if (poly == null)
                        {
                            if (!ruleField.IsOptional)
                            {
                                progress.Report($"Warning: WorldPolygon '{name}' not found.");
                            }
                        }
                        ruleField.Value = poly;
                    }
                    break;

                default:
                    // unknown/unsupported RuleField type
                    throw new NotImplementedException("RuleField type not implemented: " + targetField.GetType().Name);

            }
        }

        private void SkipNamedSection(string sectionName, Action<string>? action)
        {
            var pos = reader.CurrentPosition;
            try
            {
                var line = reader.ReadLine();
                var trimmedLine = line.Trim();
                if (trimmedLine.EndsWith(sectionName))
                {
                    action?.Invoke(line);
                    action?.Invoke(reader.ReadLine()); //start of section
                    SkipSection(action);
                }
                else
                    throw new TPMapEditorException($"{sectionName} section not found at the exepected position.");
            }
            catch (TPMapEditorException) { reader.CurrentPosition = pos; throw; }
            catch { reader.CurrentPosition = pos; throw new Exception($"Fail to skip {sectionName} section."); }
        }

        private void SkipNamedSection(string sectionName)
        {
            var pos = reader.CurrentPosition;
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
            catch (TPMapEditorException) { reader.CurrentPosition = pos; throw; }
            catch { reader.CurrentPosition = pos; throw new Exception($"Fail to skip {sectionName} section."); }
        }

        private void SkipSection(Action<string>? action)
        {
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                action?.Invoke(line);
                var trimmedLine = line.Trim();
                if (trimmedLine.Equals("{"))
                    SkipSection(action);
                else if (trimmedLine.Equals("}"))
                    break;
            }
        }

        private void SkipSection()
        {
            while (!reader.EndOfStream)
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
                //Current Objective Point Int
                var currentObjectivePointIndex = reader.ReadAndParseInt("Current Objective Point Int ");

                //Current Point Visible On StarMap Bool
                map.IsCurrentObjectivePointVisibleOnStarMap = reader.ReadAndParseBool("Current Point Visible On StarMap Bool ");

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

                try
                {
                    if (currentObjectivePointIndex < 0)
                        map.CurrentObjectivePoint = null;
                    else
                        map.CurrentObjectivePoint = map.ObjectivePoints[currentObjectivePointIndex];
                }
                catch { throw new TPMapEditorException("Fail to set current objective point."); }

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
