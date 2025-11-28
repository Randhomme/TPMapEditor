using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public class DataExport : IDisposable
    {
        private StreamWriter writer;
        private WorldMap map;
        private IProgress<string> progress;
        private IProgress<string> progressOperation;
        private int currentLineNumber = 0;
        const string F6 = "0.000000";

        public DataExport(string filePath, WorldMap map, IProgress<string> progress, IProgress<string> progressOperation)
        {
            this.writer = new StreamWriter(File.Open(filePath, FileMode.Create));
            this.map = map;
            this.progress = progress;
            this.progressOperation = progressOperation;
        }

        public void CreateMapFileAndWriteData()
        {
            progressOperation.Report("Begin map export ...");
            var time = DateTime.Now;
            try
            {
                // Write comment line, and give credits for your favorite video game
                writer.WriteLine("# (c) BaRkiNg DOG studios 2002");

                // Write sections
                int baseLevel = 0; //It should stay 0, no reason to add tabs
                WriteWorldInfoSection(baseLevel);
                WriteGameSection(baseLevel);
                WriteWorldSection(baseLevel);

                progressOperation.Report($"Map export completed in {(DateTime.Now - time).TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                progressOperation.Report("Map export failed.");
                progress.Report($"An error has occured.\n{ex.Message}");
            }
        }

        #region WorldInfo

        private void WriteWorldInfoSection(int level = 0)
        {
            WriteSection("WorldInfo", (level) =>
            {
                WriteLineLevel($"IsMultiplayerMap Bool {map.IsMultiplayer}", level);
                WriteLineLevel($"MustAssembleFleet Bool {map.MustAssembleFleet}", level);
                WriteLineLevel($"World Description String '{map.WorldDescription}'", level);
                WriteLineLevel($"WorldNameID String '{map.WorldName}'", level);
                WriteLineLevel($"Object Count Int {map.WorldObjects.Count}", level);
                WriteLineLevel($"Team List - Size Int {map.SelectableTeams.Count}", level);
                for(int i = 0; i < map.SelectableTeams.Count; i++)
                {
                    var team = map.SelectableTeams[i];
                    WriteSelectableTeamSection(team, level);
                }
                var playablePlayers = map.Players.Where((p) => p.IsPlayable);
                WriteLineLevel($"Number of Players Int {playablePlayers.Count()}", level);
                foreach(var player in playablePlayers)
                {
                    WriteLineLevel($"PlayerInfo - Player Name String '{player.Name}'", level);
                    var selectableTeamIndex = player.SelectableTeam is null ? -1 : map.SelectableTeams.IndexOf(player.SelectableTeam);
                    WriteLineLevel($"PlayerInfo - TeamIndex Int {selectableTeamIndex}", level);
                }
                WriteLineLevel($"IsCampaign Bool {map.IsCampaign}", level);
                WriteLineLevel($"Use Custom World Name Bool {map.UseCustomName}", level);
                WriteLineLevel($"Custom World Name String '{map.CustomName}'", level);
                WriteLineLevel($"Use Custom World Description Bool {map.UseCustomDescription}", level);
                WriteLineLevel($"Custom World Description String '{map.CustomDescription}'", level);
            }, level);
        }

        private void WriteSelectableTeamSection(Team team, int level)
        {
            WriteSection("Team List - Element", (level) =>
            {
                WriteLineLevel($"Team Name ID String '{team.RealName}'", level);
                WriteLineLevel($"Race Int {(int)team.Race}", level);
                WriteLineLevel($"Race Lock Bool {team.RaceLocked}", level);
            }, level);
        }

        #endregion

        #region Game

        private void WriteGameSection(int level = 0)
        {
            WriteSection("Game", (level) =>
            {
                WriteTimeSection(level);
                WriteLineLevel("Frame Int -1", level);
                WriteLineLevel("Paused Bool False", level);
                WriteLineLevel("ActivePlayerIndex Int -1", level);
            }, level);
        }

        private void WriteTimeSection(int level)
        {
            WriteSection("Time", (level) =>
            {
                WriteLineLevel("Game Tick Int 0", level);
                WriteLineLevel("Game Time Double 0", level);
            }, level);
        }

        #endregion

        #region World

        private void WriteWorldSection(int level = 0)
        {
            WriteSection("World", (level) =>
            {
                var worldName = string.IsNullOrEmpty(map.CustomName) ? "WorldName" : map.CustomName; // Unused
                StringDictionnary.WorldNames.TryGetValue(map.WorldName, out worldName);
                WriteLineLevel($"WorldName String '{worldName}'", level);
                WriteLineLevel("Random Seed Int 0", level); // Idk about this
                WriteLineLevel($"World Size - Min Vector3( {-map.Size / 2:F6},  {-map.Size / 2:F6}, {-map.ZSize / 2:F6} )", level);
                WriteLineLevel($"World Size - Max Vector3( {map.Size / 2:F6},  {map.Size / 2:F6}, {map.ZSize / 2:F6} )", level);
                WriteLineLevel("# Player List", level);
                WriteLineLevel($"PlayerList Int {map.Players.Count}", level);
                for(int i = 0; i < map.Players.Count; i++)
                {
                    var player = map.Players[i];
                    WritePlayerSection(player, level);
                }
                WriteLineLevel($"NextID Int {map.WorldObjects.LastOrDefault()?.Id ?? 0}", level);
                WriteLineLevel("# World Object List", level);
                WriteLineLevel($"WorldObjects Int {map.WorldObjects.Count}", level);
                for(int i = 0; i < map.WorldObjects.Count; i++)
                {
                    var worldObject = map.WorldObjects[i];
                    WriteLineLevel($"ID Int {worldObject.Id}", level);
                    WriteLineLevel($"Type String '{worldObject.Type.Type}'", level);
                    WriteWorldObjectStateSection(worldObject, level);
                }
            }, level);
        }

        private void WritePlayerSection(Player player, int level)
        {
            WriteSection("Player", (level) =>
            {
                WriteLineLevel($"Name String '{player.Name}'", level);
                WriteLineLevel($"Color Colour( {player.Color.R / 255f:F6}, {player.Color.G / 255f:F6}, {player.Color.B / 255f:F6}, {player.Color.A / 255f:F6} )", level);
                WriteLineLevel($"IsPlayable Bool {player.IsPlayable}", level);
                WriteLineLevel("Is Used In Game Bool False", level); // False for now, maybe state ?
                WriteLineLevel("Multiplayer Name String ''", level); //Empty for now, maybe state ?
                WriteLineLevel($"StartPoint Vector3( {player.X:F6}, {player.Y:F6}, {player.Z:F6} )", level);
                var playerRotationRad = player.Rotation * (float)Math.PI / 180f;
                WriteLineLevel($"StartPointForwardVector Vector3( {Math.Sin(playerRotationRad):F6}, {Math.Cos(playerRotationRad):F6}, 0.000000 )", level);
                var raceInt = player.InGameTeam is null ? 4 : (int)player.InGameTeam.Race;
                WriteLineLevel($"Race Int {raceInt}", level);
                WriteLineLevel("Points Float 0.000000", level);
                var inGameTeamIndex = player.InGameTeam is null ? -1 : map.InGameTeams.IndexOf(player.InGameTeam);
                WriteLineLevel($"TeamIndex Int {inGameTeamIndex}", level);
                WriteLineLevel($"FormationType Int {(int)player.FormationTypeStart}", level);
                WriteFleetAiSection(player, level);
                WriteLineLevel("FlagIndex Int 0", level);
            }, level);
        }

        private void WriteFleetAiSection(Player player, int level)
        {
            WriteSection("FleetAI", (level1) =>
            {
                WriteSection("UPDATETIMER", (level2) =>
                {
                    WriteLineLevel("StartTime Double 0", level2);
                }, level1);
                WriteSection("OFFSETTIMER", (level2) =>
                {
                    WriteLineLevel("StartTime Double 0", level2);
                }, level1);
                WriteLineLevel("OFFSETTIME Float 0.062500", level1);
                WriteLineLevel("UPDATETIME Float 0.500000", level1);
                WriteSection("FORMATION", (level2) =>
                {
                    WriteLineLevel($"FORMATIONTYPE String '{player.FormationType}'", level2);
                }, level1);
                WriteLineLevel("SHIPINFO - Size Int 0", level1);
                WriteLineLevel("HOLDFIREACTIVE Bool False", level1);
                WriteLineLevel("AITYPE String 'AIFLEET'", level1);
            }, level);
        }

        private void WriteWorldObjectStateSection(WorldObject worldObject, int level)
        {
            WriteSection("State", (level) =>
            {
                WriteLineLevel("HasState Bool False", level); // No state for now, of course
                WriteLineLevel($"Position Vector3( {worldObject.X:F6}, {worldObject.Y:F6}, {worldObject.Z:F6} )", level);
                var orMat33 = MathUtils.EulerXYZToMatrix33(worldObject.XRotation, worldObject.YRotation, worldObject.ZRotation);
                WriteLineLevel($"Orientation Matrix33( {orMat33[0,0]:F6}, {orMat33[1, 0]:F6}, {orMat33[2, 0]:F6}, {-orMat33[0, 2]:F6}, {-orMat33[1, 2]:F6}, {-orMat33[2, 2]:F6}, {orMat33[0, 1]:F6}, {orMat33[1, 1]:F6}, {orMat33[2, 1]:F6} )", level);
                var playerIndex = worldObject.Player is null ? -1 : map.Players.IndexOf(worldObject.Player);
                WriteLineLevel($"PlayerIndex Int {playerIndex}", level);
                WriteLineLevel("# AIEntity", level);
                WriteLineLevel("Type String ''", level);
                WriteLineLevel("# RenderEntity", level);
                WriteLineLevel("Type String ''", level);
                WriteLineLevel("# PhysicsEntity", level);
                WriteLineLevel("Type String ''", level);
                WriteLineLevel("# CollisionEntity", level);
                WriteLineLevel("Type String ''", level);
                WriteLineLevel("# CustomInfoEntity", level);
                WriteLineLevel("Type String ''", level);
            }, level);
        }

        #endregion

        private void WriteSection(string sectionName, Action<int> action, int level = 0)
        {
            writer.Flush();

            var pos = writer.BaseStream.Position;

            WriteLineLevel($"00000000 {sectionName}", level);
            WriteLineLevel("{", level);

            var lineNumberBefore = currentLineNumber;
            action.Invoke(level + 1);
            var sectionLineCount = currentLineNumber - lineNumberBefore;
            
            WriteLineLevel("}", level);

            // Go back to write the correct number of line for the section
            writer.Flush();
            writer.BaseStream.Seek(pos, SeekOrigin.Begin);
            WriteLineLevel($"{sectionLineCount:D8} {sectionName}", level);
            currentLineNumber--; // It's not a new line
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.End);

        }

        private void WriteLineLevel(string line, int level)
        {
            string tabs = "";
            for(int i = 0; i < level; i++)
            {
                tabs += "\t";
            }
            writer.WriteLine(tabs+line);
            currentLineNumber++;
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
