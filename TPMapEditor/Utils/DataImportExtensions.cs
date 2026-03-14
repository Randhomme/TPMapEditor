using System.IO;
using System.Numerics;
using System.Windows.Media;
using TPMapEditor.Exceptions;

namespace TPMapEditor.Utils
{
    public static class DataImportExtensions
    {
        public static bool ReadAndParseBool(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            return ParseBool(line, prefix);
        }

        public static bool ParseBool(string line, string prefix)
        {
            bool.TryParse(line.GetSubstring(prefix), out var value);
            return value;
        }

        public static int ReadAndParseInt(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            return ParseInt(prefix, line);
        }

        public static int ParseInt(string prefix, string line)
        {
            int.TryParse(line.GetSubstring(prefix), out var value);
            return value;
                
        }

        public static double ReadAndParseDouble(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            return ParseDouble(prefix, line);
        }

        public static double ParseDouble(string prefix, string line)
        {
            double.TryParse(line.GetSubstring(prefix), out var value);
            return value;
        }

        public static string ReadAndParseString(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            return ParseString(prefix, line);
        }

        public static string ParseString(string prefix, string line)
        {
            return line.GetSubstring(prefix).Trim('\'');
        }

        public static Color ReadAndParseColor(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var r);
            float.TryParse(values[1], out var g);
            float.TryParse(values[2], out var b);
            float.TryParse(values[3], out var a);
            return Color.FromArgb((byte)(a * 255f), (byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f));
        }

        public static Vector3 ReadAndParseVector3(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x);
            float.TryParse(values[1], out var y);
            float.TryParse(values[2], out var z);
            return new Vector3(x, y, z);
        }

        public static Vector2 ReadAndParseVector2(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x);
            float.TryParse(values[1], out var y);
            return new Vector2(x, y);
        }

        public static (Vector3 x, Vector3 y, Vector3 z) ReadAndParseMatrix33(this StreamReader reader, string prefix)
        {
            var line = reader.ReadLine().Trim();
            line = line.GetSubstring(prefix).Trim('(', ')');
            var values = line.Split(',');
            float.TryParse(values[0], out var x1);
            float.TryParse(values[1], out var x2);
            float.TryParse(values[2], out var x3);
            float.TryParse(values[3], out var y1);
            float.TryParse(values[4], out var y2);
            float.TryParse(values[5], out var y3);
            float.TryParse(values[6], out var z1);
            float.TryParse(values[7], out var z2);
            float.TryParse(values[8], out var z3);
            return (new Vector3(x1, x2, x3), new Vector3(y1, y2, y3), new Vector3(z1, z2, z3));

        }

        private static string GetSubstring(this string str, string val)
        {
            if (str.StartsWith(val))
                return str.Substring(val.Length);
            else
                throw new TPMapEditorException($"'{val.Trim()}' not found in '{str}'");
        }
    }
}
