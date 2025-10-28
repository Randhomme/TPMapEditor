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
                    for (int i = 0; i < 6; i++)
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
            return Color.FromScRgb(a, r, g, b);
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
            // Columns correspond to local X, Y, Z axes
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
    }
}
