using System;
using System.IO;
using System.Linq;
using TPMapEditor.Data.Rule;
using TPMapEditor.Enums;
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
                map.ReorganizeWorldObjectIds();

                // Give credits for your favorite video game
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
                    WriteTeamListElementSection(team, level);
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

        private void WriteTeamListElementSection(Team team, int level)
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
                WriteLineLevel($"World Size - Min Vector3( {-map.Size / 2:F6}, {-map.Size / 2:F6}, {-map.ZSize / 2:F6} )", level);
                WriteLineLevel($"World Size - Max Vector3( {map.Size / 2:F6}, {map.Size / 2:F6}, {map.ZSize / 2:F6} )", level);
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
                    WriteLineLevel($"Type String '{worldObject.Type.Name}'", level);
                    WriteWorldObjectStateSection(worldObject, level);
                }
                WriteGameSpecificSection(level);
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
                WriteLineLevel($"HasState Bool {worldObject.HasState}", level);
                WriteLineLevel($"Position Vector3( {worldObject.X:F6}, {worldObject.Y:F6}, {worldObject.Z:F6} )", level);
                var orMat33 = MathUtils.EulerXYZToMatrix33(worldObject.XRotation, worldObject.YRotation, worldObject.ZRotation);
                WriteLineLevel($"Orientation Matrix33( {orMat33[0,0]:F6}, {orMat33[1, 0]:F6}, {orMat33[2, 0]:F6}, {orMat33[0, 1]:F6}, {orMat33[1, 1]:F6}, {orMat33[2, 1]:F6}, {orMat33[0, 2]:F6}, {orMat33[1, 2]:F6}, {orMat33[2, 2]:F6} )", level);
                var playerIndex = worldObject.Player is null ? -1 : map.Players.IndexOf(worldObject.Player);
                WriteLineLevel($"PlayerIndex Int {playerIndex}", level);
                WriteLineLevel("# AIEntity", level);
                WriteLineLevel(worldObject.AIEntity, 0);
                WriteLineLevel("# RenderEntity", level);
                WriteLineLevel(worldObject.RenderEntity, 0);
                WriteLineLevel("# PhysicsEntity", level);
                WriteLineLevel(worldObject.PhysicsEntity, 0);
                WriteLineLevel("# CollisionEntity", level);
                WriteLineLevel(worldObject.CollisionEntity, 0);
                WriteLineLevel("# CustomInfoEntity", level);
                WriteLineLevel(worldObject.CustomInfoEntity, 0);
            }, level);
        }

        private void WriteGameSpecificSection(int level)
        {
            WriteSection("GameSpecific", (level) =>
            {
                WriteLineLevel($"World Description Sting ID String '{map.WorldDescription}'", level);
                WriteLineLevel($"World Name Sting ID String '{map.WorldName}'", level);
                WriteSection("Effect Event Keeper", (level) =>
                {
                    WriteLineLevel("NumEffectEventInfoChunks Int 0", level);
                }, level);
                WriteLineLevel($"Skybox mesh name String '{map.Skybox}'", level);
                WriteLineLevel($"Ambient Light Colour( {map.AmbientLightColor.R/255f:F6}, {map.AmbientLightColor.G / 255f:F6}, {map.AmbientLightColor.B / 255f:F6}, {map.AmbientLightColor.A / 255f:F6} )", level);
                var rlov = MathUtils.YawPitchToVector3(map.RoofLightOrientationYaw, map.RoofLightOrientationPitch);
                WriteLineLevel($"Vector for roof light orientation Vector3( {rlov.X:F6}, {rlov.Y:F6}, {rlov.Z:F6} )", level);
                WriteLineLevel($"Hemispherical floor light color Colour( {map.FloorLightColor.R / 255f:F6}, {map.FloorLightColor.G / 255f:F6}, {map.FloorLightColor.B / 255f:F6}, {map.FloorLightColor.A / 255f:F6} )", level);
                WriteLineLevel($"Hemispherical roof light color Colour( {map.RoofLightColor.R / 255f:F6}, {map.RoofLightColor.G / 255f:F6}, {map.RoofLightColor.B / 255f:F6}, {map.RoofLightColor.A / 255f:F6} )", level);
                WriteLineLevel("World Initialized State Bool False", level);
                WriteLineLevel($"World Buffer Size Float {map.WorldBuffer:F6}", level);
                WriteLineLevel($"Waypoint Path Info Vector - Size Int {map.WaypointPaths.Count}", level);
                for (int i = 0; i < map.WaypointPaths.Count; i++)
                {
                    var path = map.WaypointPaths[i];
                    WriteWaypointPathInfoVectorElement(path, level);
                }
                WriteLineLevel($"World Polygons Vectors - Size Int {map.WorldPolygons.Count}", level);
                for(int i = 0; i < map.WorldPolygons.Count; i++)
                {
                    var polygon = map.WorldPolygons[i];
                    WriteWorldPolygonsVectors(polygon, level);
                }
                WriteLineLevel($"World Point Sets Vector - Size Int {map.WorldPointSets.Count}", level);
                for(int i = 0; i < map.WorldPointSets.Count; i++)
                {
                    var pointSet = map.WorldPointSets[i];
                    WriteWorldPointSetsVector(pointSet, level);
                }
                WriteLineLevel($"Flag List - Size Int {map.Flags.Count}", level);
                for(int i = 0; i < map.Flags.Count; i++)
                {
                    var flag = map.Flags[i];
                    WriteFlagListElementSection(flag, level);
                }
                WriteLineLevel($"Timer List - Size Int {map.Timers.Count}", level);
                for (int i = 0; i < map.Timers.Count; i++)
                {
                    var timer = map.Timers[i];
                    WriteTimerListElementSection(timer, level);
                }
                WriteLineLevel($"Speech Event List - Size Int {map.SpeechEvents.Count}", level);
                for (int i = 0; i < map.SpeechEvents.Count; i++)
                {
                    var speechEvent = map.SpeechEvents[i];
                    WriteSpeechEventListElementSection(speechEvent, level);
                }
                WriteLineLevel($"PlayerAllianceInfoVector - Size Int {map.PlayerAlliances.Count}", level);
                for (int i = 0; i < map.PlayerAlliances.Count; i++)
                {
                    var playerAlliance = map.PlayerAlliances[i];
                    WritePlayerAllianceInfoVectorElementSection(playerAlliance, level);
                }
                WriteLineLevel($"Team List - Size Int {map.InGameTeams.Count}", level);
                for (int i = 0; i < map.InGameTeams.Count; i++)
                {
                    var team = map.InGameTeams[i];
                    WriteTeamListElementSection(team, level);
                }
                WriteLineLevel("Winning Team Int -1", level);
                var usableGroups = map.Groups.Where((g) => !g.Name.Equals(Group.DefaultName));
                WriteLineLevel($"Num Groups Int {usableGroups.Count()}", level);
                foreach(var group in usableGroups)
                {
                    WriteGroupSection(group, level);
                }
                WriteWorldRulesSection(level);
                WriteObjectiveSystemSection(level);
                WriteRopeSection(level);
                WriteGrappledObjectsSection(level);
                WriteBoardingActionsSection(level);
                WriteJournalEntrySection(level);
                WriteWorldMapSection(level);
                WriteLineLevel($"Can Assemble Fleets Bool {map.MustAssembleFleet}", level);
                WriteLineLevel($"World Crew List - Size Int {map.WorldCrews.Count}", level);
                for(int i = 0; i < map.WorldCrews.Count; i++)
                {
                    var crew = map.WorldCrews[i];
                    WriteLineLevel($"World Crew List - Element String '{crew.Name}'", level);
                }
                WriteLineLevel($"World Arms List - Size Int {map.WorldArms.Count}", level);
                for(int i = 0; i < map.WorldArms.Count; i++)
                {
                    var arm = map.WorldArms[i];
                    WriteLineLevel($"World Arms List - Element String '{arm.Name}'", level);
                }
                WriteMapTextSystemSection(level);
                WriteLineLevel("READAIENTITYCOUNTS Bool False", level);
                WriteLineLevel($"Journal Music Name String '{map.JournalMusic}'", level);
                WriteLineLevel($"PlayEndMovie Bool {map.PlayEndMovie}", level);
                WriteLineLevel($"IsCampaign Bool {map.IsCampaign}", level);
                WriteLineLevel($"Is Alliance Change Allowed Bool {map.IsAllianceChangeAllowed}", level);
                WriteLineLevel($"Use Custom World Name Bool {map.UseCustomName}", level);
                WriteLineLevel($"Custom World Name String '{map.CustomName}'", level);
                WriteLineLevel($"Use Custom World Description Bool {map.UseCustomDescription}", level);
                WriteLineLevel($"Custom World Description String '{map.CustomDescription}'", level);
                WriteLineLevel($"Islands Make Sounds Bool {map.IslandsMakeSounds}", level);
                WriteLineLevel("DATA_NEBULA_CAMERA_EFFECT Int 0", level);
                WriteLineLevel("DATA_NEXT_NEBULA_CAMERA_EFFECT Int 0", level);
                WriteSection("DATA_NEBULA_CAMERA_EFFECT_FADE_IN_TIMER", (level) =>
                {
                    WriteLineLevel("StartTime Double 0", level);
                }, level);
                WriteSection("DATA_NEBULA_CAMERA_EFFECT_SPIN_TIMER", (level) =>
                {
                    WriteLineLevel("StartTime Double 0", level);
                }, level);
            }, level);
        }

        private void WriteWaypointPathInfoVectorElement(WaypointPath path, int level)
        {
            WriteSection("Waypoint Path Info Vector - Element", (level) =>
            {
                WriteLineLevel($"Waypoint Path Name String '{path.Name}'", level);
                WriteLineLevel($"Waypoint Path Points - Size Int {path.Points.Count}", level);
                for(int i = 0; i < path.Points.Count; i++)
                {
                    var pathPoint = path.Points[i];
                    WriteLineLevel($"Waypoint Path Points - Element Vector3( {pathPoint.X:F6}, {pathPoint.Y:F6}, {pathPoint.Z:F6} )", level);
                }
            }, level);
        }

        private void WriteWorldPolygonsVectors(WorldPolygon polygon, int level)
        {
            WriteSection("World Polygons Vectors - Element", (level) =>
            {
                WriteLineLevel($"Name String '{polygon.Name}'", level);
                WriteLineLevel($"Points Int {polygon.Points.Count}", level);
                for(int i = 0; i < polygon.Points.Count; i++)
                {
                    var polygonPoint = polygon.Points[i];
                    WriteLineLevel($"Points Coord( {polygonPoint.X:F6}, {polygonPoint.Y:F6} )", level);
                }
            }, level);
        }

        private void WriteWorldPointSetsVector(WorldPointSet pointSet, int level)
        {
            WriteSection("World Point Sets Vector - Element", (level) =>
            {
                WriteLineLevel($"Name String '{pointSet.Name}'", level);
                WriteLineLevel($"World Points - Size Int {pointSet.Points.Count}", level);
                for(int i = 0; i < pointSet.Points.Count; i++)
                {
                    var point = pointSet.Points[i];
                    WriteSection("World Points - Element", (level) =>
                    {
                        WriteLineLevel("World Point Magnitude Float 0.000000", level);
                        WriteSection("World Point Basis", (level) =>
                        {
                            WriteLineLevel($"Position Vector3( {point.X:F6}, {point.Y:F6}, {point.Z:F6} )", level);
                            WriteLineLevel("LookAt Vector Length Float 1.000000", level);
                            var orMat33 = MathUtils.EulerXYZToMatrix33(point.XRotation, point.YRotation, point.ZRotation);
                            WriteLineLevel($"Orientation - Cross Vector3( {orMat33[0, 0]:F6}, {orMat33[1, 0]:F6}, {orMat33[2, 0]:F6} )", level);
                            WriteLineLevel($"Orientation - Forward Vector3( {orMat33[0, 1]:F6}, {orMat33[1, 1]:F6}, {orMat33[2, 1]:F6} )", level);
                            WriteLineLevel($"Orientation - Up Vector3( {orMat33[0, 2]:F6}, {orMat33[1, 2]:F6}, {orMat33[2, 2]:F6} )", level);
                        }, level);
                    }, level);
                }
            }, level);
        }

        private void WriteFlagListElementSection(Flag flag, int level)
        {
            WriteSection("Flag List - Element", (level) =>
            {
                WriteLineLevel($"Flag Name String '{flag.Name}'", level);
                WriteLineLevel($"Flag Value Bool {flag.Value}", level);
            }, level);
        }

        private void WriteTimerListElementSection(Timer timer, int level)
        {
            WriteSection("Timer List - Element", (level) =>
            {
                WriteLineLevel($"Timer Name String '{timer.Name}'", level);
                WriteLineLevel($"Timer Status Bool {timer.Status}", level);
                WriteSection("Timer Value Chunk", (level) =>
                {
                    WriteLineLevel($"StartTime Double {timer.StartTime}", level);
                }, level);
            }, level);
        }

        private void WriteSpeechEventListElementSection(SpeechEvent speechEvent, int level)
        {
            WriteSection("Speech Event List - Element", (level) =>
            {
                WriteLineLevel($"Name String '{speechEvent.Name}'", level);
                WriteLineLevel($"Sound FileName String '{speechEvent.SoundFileName}'", level);
                WriteLineLevel($"Text Color Colour( {speechEvent.TextColor.R / 255f:F6}, {speechEvent.TextColor.G / 255f:F6}, {speechEvent.TextColor.B / 255f:F6}, {speechEvent.TextColor.A / 255f:F6} )", level);
                WriteLineLevel($"FaceTexture String '{speechEvent.FaceTexture}'", level);
                WriteLineLevel($"TalkingHeadLocation Int {(int)speechEvent.TalkingHeadLocation}", level);
                WriteLineLevel($"Has Been Played Once Bool {speechEvent.HasBeenPlayedOnce}", level);
                WriteLineLevel($"Is Secondary Speech Bool {speechEvent.IsSecondarySpeech}", level);
                WriteLineLevel($"Display Time Float {speechEvent.DisplayTime:F6}", level);
                WriteLineLevel($"Open Chat Bar Bool {speechEvent.OpenChatBar}", level);
                WriteLineLevel($"Open Talking Head Bool {speechEvent.OpenTalkingHead}", level);
                WriteLineLevel($"Has Text Bool {speechEvent.HasText}", level);
                WriteLineLevel($"Use Sound File Length Bool {speechEvent.HasText}", level);
                WriteLineLevel($"Always Open Speech Event Bar Bool {speechEvent.AlwaysOpenSpeechEventBar}", level);
                WriteLineLevel("Valid Text StringID Bool True", level);
                WriteLineLevel($"TextStringID String '{speechEvent.TextStringID}'", level);
                WriteLineLevel("Valid Speaker ID Bool True", level);
                WriteLineLevel($"SpeakerID String '{speechEvent.SpeakerID}'", level);
            }, level);
        }

        private void WritePlayerAllianceInfoVectorElementSection(PlayerAlliance playerAlliance, int level)
        {
            WriteSection("PlayerAllianceInfoVector - Element", (level) =>
            {
                WriteLineLevel($"Player0 Int {map.Players.IndexOf(playerAlliance.Player1)}", level);
                WriteLineLevel($"Player1 Int {map.Players.IndexOf(playerAlliance.Player2)}", level);
            }, level);
        }

        private void WriteGroupSection(Group group, int level)
        {
            WriteSection("Group", (level) =>
            {
                WriteLineLevel($"Name String '{group.Name}'", level);
                WriteLineLevel($"World Object IDs - Size Int {group.WorldObjects.Count}", level);
                for(int i = 0; i < group.WorldObjects.Count; i++)
                {
                    var worldObject = group.WorldObjects[i];
                    WriteLineLevel($"World Object IDs - Element Int {worldObject.Id}", level);
                }
            }, level);
        }

        private void WriteWorldRulesSection(int level)
        {
            WriteSection("World Rules", (level) =>
            {
                WriteLineLevel($"Rule List Int {map.WorldRules.Count}", level);
                for(int i = 0; i < map.WorldRules.Count; i++)
                {
                    var rule = map.WorldRules[i];
                    WriteLineLevel($"Rule Name String '{rule.Name}'", level);
                    WriteLineLevel($"Run Once Bool {rule.RunOnce}", level);
                    WriteLineLevel("Is Active Bool True", level);
                    WriteLineLevel($"NumConditions Int {rule.Conditions.Count}", level);
                    for (int j = 0; j < rule.Conditions.Count; j++)
                    {
                        var condition = rule.Conditions[j];
                        WriteConditionListSection(condition, level);
                    }
                    WriteLineLevel($"NumActions Int {rule.Actions.Count}", level);
                    for (int j = 0; j < rule.Actions.Count; j++)
                    {
                        var action = rule.Actions[j];
                        WriteActionListSection(action, level);
                    }
                }
            }, level);
        }

        private void WriteConditionListSection(Rule.RuleCondition condition, int level)
        {
            WriteSection("Condition List", (level) =>
            {
                WriteLineLevel($"Type String '{condition.Type.GetName()}'", level);
                for(int i = 0; i < condition.RuleFields.Count; i++)
                {
                    var field = condition.RuleFields[i];
                    if (field is RuleFieldObservableCollection fieldObservableCollection && fieldObservableCollection.Value != null)
                    {
                        if(fieldObservableCollection.RealLabel != null)
                        {
                            WriteLineLevel($"{fieldObservableCollection}", level);
                        }
                        for(int j = 0; j < fieldObservableCollection.Value.Count; j++)
                        {
                            var field2 = fieldObservableCollection.Value[j];
                            WriteLineLevel($"{field2}", level);
                        }
                    }
                    else
                    {
                        WriteLineLevel($"{field}", level);
                    }
                }
            }, level);
        }

        private void WriteActionListSection(Rule.RuleAction action, int level)
        {
            WriteSection("Action List", (level) =>
            {
                WriteLineLevel($"Type String '{action.Type.GetName()}'", level);
                for (int i = 0; i < action.RuleFields.Count; i++)
                {
                    var field = action.RuleFields[i];
                    if (field is RuleFieldObservableCollection fieldObservableCollection && fieldObservableCollection.Value != null)
                    {
                        if (fieldObservableCollection.RealLabel != null)
                        {
                            WriteLineLevel($"{fieldObservableCollection}", level);
                        }
                        for (int j = 0; j < fieldObservableCollection.Value.Count; j++)
                        {
                            var field2 = fieldObservableCollection.Value[j];
                            WriteLineLevel($"{field2}", level);
                        }
                    }
                    else
                    {
                        WriteLineLevel($"{field}", level);
                    }
                }
            }, level);
        }

        private void WriteObjectiveSystemSection(int level)
        {
            WriteSection("Objective System", (level) =>
            {

                WriteLineLevel($"Current Objective Point Int {map.ObjectivePoints.IndexOf(map.CurrentObjectivePoint)}", level);
                WriteLineLevel($"Current Point Visible On StarMap Bool {map.IsCurrentObjectivePointVisibleOnStarMap}", level);
                WriteLineLevel($"Objective Point Info - Size Int {map.ObjectivePoints.Count}", level);
                for(int i = 0; i < map.ObjectivePoints.Count; i++)
                {
                    var point = map.ObjectivePoints[i];
                    WriteObjectivePointInfoElementSection(point, level);
                }
                WriteLineLevel($"Objective Task Array - Size Int {map.ObjectiveTasks.Count}", level);
                for (int i = 0; i < map.ObjectiveTasks.Count; i++)
                {
                    var task = map.ObjectiveTasks[i];
                    WriteObjectiveTaskArrayElement(task, level);
                }
            }, level);
        }

        private void WriteObjectivePointInfoElementSection(ObjectivePoint point, int level)
        {
            WriteSection("Objective Point Info - Element", (level) =>
            {
                WriteLineLevel($"Name String '{point.Name}'", level);
                WriteLineLevel($"Position Vector3( {point.X:F6}, {point.Y:F6}, {point.Z:F6} )", level);
            }, level);
        }

        private void WriteObjectiveTaskArrayElement(ObjectiveTask task, int level)
        {
            WriteSection("Objective Task Array - Element", (level) =>
            {
                WriteLineLevel($"Name String '{task.Name}'", level);
                WriteLineLevel($"TextStringID String '{task.TextStringId}'", level);
                WriteLineLevel($"Active Bool {task.Active}", level);
                WriteLineLevel($"Completed Bool {task.Completed}", level);
                WriteLineLevel($"Failed Bool {task.Failed}", level);
            }, level);
        }

        private void WriteRopeSection(int level)
        {
            WriteSection("Rope", (level) =>
            {
                WriteLineLevel("RopeInfo - Size Int 0", level);
            }, level);
        }

        private void WriteGrappledObjectsSection(int level)
        {
            WriteSection("Grappled Objects", (level) =>
            {
                WriteLineLevel("Grappled Objects Info - Size Int 0", level);
            }, level);
        }

        private void WriteBoardingActionsSection(int level)
        {
            WriteSection("Boarding Actions", (level) =>
            {
                WriteLineLevel("Boarding Actions Info - Size Int 0", level);
            }, level);
        }

        private void WriteJournalEntrySection(int level)
        {
            WriteSection("Journal Entry", (level) =>
            {
                WriteLineLevel($"Page Info - Size Int {map.JournalEntries.Count}", level);
                for(int i = 0; i < map.JournalEntries.Count; i++)
                {
                    var entry = map.JournalEntries[i];
                    WritePageInfoElementSection(entry, level);
                }
                WriteLineLevel($"Title StringID String '{map.JournalTitle}'", level);
            }, level);
        }

        private void WritePageInfoElementSection(JournalEntry entry, int level)
        {
            WriteSection("Page Info - Element", (level) =>
            {
                WriteLineLevel($"TextStringID String '{entry.TextStringId}'", level);
                WriteLineLevel($"SpeechEventFileName String '{entry.SpeechEventFileName}'", level);
                WriteLineLevel($"PictureTexture String '{entry.PictureTexture}'", level);
            }, level);
        }

        private void WriteWorldMapSection(int level)
        {
            WriteSection("World Map", (level) =>
            {
                WriteLineLevel($"Backdrop Texture Name String '{map.StarmapTexture}'", level);
            }, level);
        }

        private void WriteMapTextSystemSection(int level)
        {
            WriteSection("MapText System", (level) =>
            {
                WriteLineLevel($"MapText Point Info - Size Int {map.MapTextPoints.Count}", level);
                for(int i = 0; i < map.MapTextPoints.Count; i++)
                {
                    var point = map.MapTextPoints[i];
                    WriteMapTextPointInfoElementSection(point, level);
                }
            }, level);
        }

        private void WriteMapTextPointInfoElementSection(MapTextPoint point, int level)
        {
            WriteSection("MapText Point Info - Element", (level) =>
            {
                WriteLineLevel($"Name String '{point.Name}'", level);
                WriteLineLevel($"DisplayedText String '{point.RealText}'", level);
                WriteLineLevel($"Position Vector3( {point.X:F6}, {point.Y:F6}, {point.Z:F6} )", level);
                WriteLineLevel($"Visible Bool {point.Visible}", level);
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
