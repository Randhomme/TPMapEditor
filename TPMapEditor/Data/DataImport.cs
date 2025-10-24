using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
                ReadWorldInfoBloc(reader, map);
            }
            //TODO : handle the error, possibly with an IProgress thing
            catch { }
        }

        private static void ReadWorldInfoBloc(StreamReader reader, WorldMap map)
        {
            try
            {
                var line = reader.ReadLine().Trim();
                if (line.EndsWith("WorldInfo"))
                {
                    reader.ReadLine(); //skip line

                    //IsMultiplayerMap
                    var line2 = reader.ReadLine().Trim();
                    bool.TryParse(line2.GetSafeSubstring("IsMultiplayerMap Bool "), out var isMultiplayer);

                    //MustAssembleFleet
                    line2 = reader.ReadLine().Trim();
                    bool.TryParse(line2.GetSafeSubstring("MustAssembleFleet Bool "), out var mustAssembleFleet);

                    //World Description
                    line2 = reader.ReadLine().Trim();
                    var worldDescription = line2.GetSafeSubstring("World Description String ").Trim('\'');

                    //WorldNameID
                    line2 = reader.ReadLine().Trim();
                    var worldName = line2.GetSafeSubstring("WorldNameID String ").Trim('\'');

                    //Object count, skip
                    reader.ReadLine();

                    //Team List - Size
                    line2 = reader.ReadLine().Trim();
                    int.TryParse(line2.GetSafeSubstring("Team List - Size Int "), out var teamListSize);

                    //Team List - Element
                    for(int i = 0; i < teamListSize; i++)
                    {
                        reader.ReadLine(); //skip line
                        reader.ReadLine(); //skip line

                        //Team Name ID
                        line2 = reader.ReadLine().Trim();
                        var teamName = line2.GetSafeSubstring("Team Name ID String ").Trim('\'');

                        //Race
                        line2 = reader.ReadLine().Trim();
                        int.TryParse(line2.GetSafeSubstring("Race Int "), out var raceInt);
                        var race = (Race)raceInt;
                    }
                }
                else
                    throw new TPMapEditorException("WorldInfo bloc not found at the exepected position.");
            }
            catch(TPMapEditorException) { throw; }
            catch { throw new Exception("Failed to read WorldInfo bloc."); }
        }

        private static string GetSafeSubstring(this string str, string val)
        {
            if(str.StartsWith(val))
                return str.Substring(val.Length);
            return string.Empty;
        }
    }
}
