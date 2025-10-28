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
                    player.X = startPoint.x;
                    player.Y = startPoint.y;
                    player.Z = startPoint.z;

                    //StartPointForwardVector
                    var startPointForwardVector = reader.ReadAndParseVector3("StartPointForwardVector Vector3");
                    player.Rotation = Math.Atan2(startPointForwardVector.x, startPointForwardVector.y) * 180 / Math.PI;

                    //Race
                    reader.ReadLine(); //probably not used

                    //Points
                    reader.ReadLine(); //probably not used

                    //Team Index
                    reader.ReadLine(); //probably not used

                    //Formation type
                    var formationTypeStart = (FormationType)reader.ReadAndParseInt("FormationType Int ");

                    //FleetAI section
                    ReadFleetAISection(reader, map);

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

        private static void ReadFleetAISection(StreamReader reader, WorldMap map)
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
                    var formationType = Enum.Parse(typeof(FormationType), reader.ReadAndParseString("FORMATIONTYPE String "));
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

        private static (float x, float y, float z) ReadAndParseVector3(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSafeSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x);
            float.TryParse(values[1], out var y);
            float.TryParse(values[2], out var z);
            return (x, y, z);
        }

        private static string GetSafeSubstring(this string str, string val)
        {
            if(str.StartsWith(val))
                return str.Substring(val.Length);
            return string.Empty;
        }
    }
}
