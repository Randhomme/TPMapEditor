using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        private static void ReadWorldInfoSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("WorldInfo"))
                {
                    reader.ReadLine(); //skip line

                    //IsMultiplayerMap
                    map.IsMultiplayer = reader.ReadAndParseBool("IsMultiplayerMap Bool ");

                    //MustAssembleFleet
                    map.MustAssembleFleet = reader.ReadAndParseBool("MustAssembleFleet Bool ");

                    //World Description
                    var worldDescription = reader.ReadAndParseString("World Description String ");
                    StringDictionnary.WorldDescriptions.TryGetValue(worldDescription, out var displayedDescription);
                    map.CustomDescription = displayedDescription;

                    //WorldNameID
                    var worldName = reader.ReadAndParseString("WorldNameID String ");
                    StringDictionnary.WorldNames.TryGetValue(worldName, out var displayedName);
                    map.CustomName = displayedName;

                    //Object count, skip
                    reader.ReadLine();

                    //Team List - Size
                    var teamListSize = reader.ReadAndParseInt("Team List - Size Int ");

                    //Team List - Element
                    for(int i = 0; i < teamListSize; i++)
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

                        map.Teams.Add(new(teamName) { Race = race, RaceLocked = raceLocked });
                    }

                    //Number of Players (playable)
                    var numberOfPlayers = reader.ReadAndParseInt("Number of Players Int ");

                    //Player loop
                    for(int i = 0; i < numberOfPlayers; i++)
                    {
                        //PlayerInfo - Player Name
                        var playerName = reader.ReadAndParseString("PlayerInfo - Player Name String ");

                        //PlayerInfo - TeamIndex
                        var playerTeam = reader.ReadAndParseInt("PlayerInfo - TeamIndex Int ");

                        map.Players.Add(new(playerName, map, 0, 0, 0, 0, Colors.Red) { IsPlayable = true, Team = map.Teams[playerTeam] });
                    }

                    //IsCampaign
                    map.IsCampaign = reader.ReadAndParseBool("IsCampaign Bool ");

                    //Use Custom World Name
                    var useCustomWorldName = reader.ReadAndParseBool("Use Custom World Name Bool ");

                    //Custom World Name
                    if (useCustomWorldName)
                        map.CustomName = reader.ReadAndParseString("Custom World Name String ");
                    else
                        reader.ReadLine();

                    //Use Custom World Name
                    var useCustomWorldDescription = reader.ReadAndParseBool("Use Custom World Description Bool ");

                    //Custom World Name
                    if (useCustomWorldDescription)
                        map.CustomName = reader.ReadAndParseString("Custom World Description String ");
                    else
                        reader.ReadLine();

                    //end of section
                    reader.ReadLine();
                }
                else
                    throw new TPMapEditorException("WorldInfo section not found at the exepected position.");
            }
            catch(TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read WorldInfo section."); }
        }

        private static void ReadGameSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("Game"))
                {
                    for (int i = 0; i < 9; i++)
                        reader.ReadLine();
                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("Game section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read Game section."); }
        }

        private static void ReadWorldSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("World"))
                {
                    reader.ReadLine(); //start of section

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
                            ReadPlayerSection(reader, map);
                        }
                        catch(Exception ex) { throw new TPMapEditorException($"Fail to read Player section number {i} : {ex.Message}", ex); }
                    }

                    //WorldObject List section
                    ReadWorldObjectList(reader, map);

                    //GameSpecific section
                    ReadGameSpecificSection(reader, map);

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("World section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read World section."); }
        }

        private static void ReadPlayerSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("Player"))
                {
                    //start of section
                    reader.ReadLine();

                    //Name
                    var playerName = reader.ReadAndParseString("Name String ");
                    var player = map.Players.FirstOrDefault((p) => p.Name == playerName);
                    if(player is null)
                    {
                        player = new(playerName, map, 0, 0, 0, 0, Colors.White);
                        map.Players.Add(player);
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
                    reader.ReadLine(); //probably not used

                    //Formation type
                    var formationTypeStart = (FormationType)reader.ReadAndParseInt("FormationType Int ");

                    //FleetAI section
                    ReadFleetAISection(reader, player);

                    //FlagIndex
                    reader.ReadLine(); //probably not used

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("Player section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read Player section."); }
        }

        private static void ReadFleetAISection(StreamReader reader, Player player)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("FleetAI"))
                {
                    reader.ReadLine(); //start of section

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

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("FleetAI section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read FleetAI section."); }
        }

        private static void ReadWorldObjectList(StreamReader reader, WorldMap map)
        {
            try
            {
                //NextID (not used)
                var line1 = reader.ReadLine();

                //#World Object List (comment line)
                reader.ReadLine();

                //WorldObject (number of WorldObject)
                var worldObjectCount = reader.ReadAndParseInt("WorldObjects Int ");

                //WorldObject loop
                for (int i = 0; i < worldObjectCount; i++)
                {
                    try
                    {
                        ReadWorldObjectSection(reader, map);
                    }
                    catch (Exception ex) { throw new TPMapEditorException($"Fail to read WorldObject section number {i} : {ex.Message}", ex); }
                }

            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read World Object List section."); }
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
            catch { throw new Exception("Fail to read FleetAI section."); }
        }

        private static void ReadWorldObjectStateSection(StreamReader reader, WorldObject worldObject, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("State"))
                {
                    reader.ReadLine(); //start of section

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

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("State section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read FleetAI section."); }
        }

        private static void ReadGameSpecificSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("GameSpecific"))
                {
                    reader.ReadLine(); //start of section

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

                    for(int i = 0; i < waypointPathCount; i++)
                    {
                        try
                        {
                            ReadWaypointPathInfoVectorElementSection(reader, map);
                        }
                        catch (Exception ex) { throw new TPMapEditorException($"Fail to read Waypoint Path Info section number {i} : {ex.Message}", ex); }
                    }

                    //World Polygons Vectors - Size
                    var worldPolygonCount = reader.ReadAndParseInt("World Polygons Vectors - Size Int ");
                    for(int i = 0; i < worldPolygonCount; i++)
                    {
                        try
                        {
                            ReadWorldPolygonVectorsSection(reader, map);
                        }
                        catch (Exception ex) { throw new TPMapEditorException($"Fail to read World Polygon section number {i} : {ex.Message}", ex); }
                    }

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("GameSpecific section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read GameSpecific section."); }
        }

        private static void ReadEffectEventKeeperSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("Effect Event Keeper"))
                {
                    reader.ReadLine(); //start of section

                    //NumEffectEventInfoChunks
                    var numEffectEvent = reader.ReadAndParseInt("NumEffectEventInfoChunks Int ");

                    //EffectEventInfo (skip for now)
                    for (int i = 0; i < numEffectEvent; i++)
                        for (int j = 0; j < 5; j++)
                            reader.ReadLine();

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("GameSpecific section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read GameSpecific section."); }
        }

        private static void ReadWaypointPathInfoVectorElementSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("Waypoint Path Info Vector - Element"))
                {
                    reader.ReadLine(); //start of section

                    var waypointPath = new WaypointPath(reader.ReadAndParseString("Waypoint Path Name String "), map);

                    //Waypoint Path Points - Size
                    var waypointPathPointsCount = reader.ReadAndParseInt("Waypoint Path Points - Size Int ");

                    for (int i = 0; i < waypointPathPointsCount; i++)
                    {
                        var vector = reader.ReadAndParseVector3("Waypoint Path Points - Element Vector3");
                        waypointPath.Points.Add(new(waypointPath, vector.X, vector.Y, vector.Z));
                    }

                    map.WaypointPaths.Add(waypointPath);

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("Waypoint Path Info Vector - Element section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read Waypoint Path Info Vector - Element section."); }
        }

        private static void ReadWorldPolygonVectorsSection(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("World Polygons Vectors - Element"))
                {
                    reader.ReadLine(); //start of section

                    var worldPolygon = new WorldPolygon(reader.ReadAndParseString("Name String "), map);

                    //Points
                    var worldPolygonPointsCount = reader.ReadAndParseInt("Points Int ");

                    for (int i = 0; i < worldPolygonPointsCount; i++)
                    {
                        var vector = reader.ReadAndParseVector2("Points Coord");
                        worldPolygon.Points.Add(new(worldPolygon, vector.X, vector.Y));
                    }

                    map.WorldPolygons.Add(worldPolygon);

                    reader.ReadLine(); //end of section
                }
                else
                    throw new TPMapEditorException("World Polygons Vectors - Element section not found at the exepected position.");
            }
            catch (TPMapEditorException) { throw; }
            catch { throw new Exception("Fail to read World Polygons Vectors - Element section."); }
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
        /// <param name="dir"></param>
        /// <returns></returns>
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
