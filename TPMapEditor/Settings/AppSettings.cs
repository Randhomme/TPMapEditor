using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using TPMapEditor.Data;
using TPMapEditor.Enums;
using TPMapEditor.Enums.WorldObjectDefinition;

namespace TPMapEditor.Settings
{
    public partial class AppSettings : ObservableObject
    {
        private readonly string filename = "TPMapEditor.xml";
        [ObservableProperty]
        private string tpGamePath = string.Empty;
        public static IList<string> FlagTextures { get; } = new List<string>();
        public static IList<string> Effects { get; } = new List<string>();
        public static IList<string> SinglePlayerMissions { get; } = new List<string>();
        public static IList<string> GuiTextures { get; } = new List<string>();
        public static IList<string> GuiTexturesFull { get; } = new List<string>();
        public static IList<string> Musics { get; } = new List<string>();
        public static IList<string> Meshes { get; } = new List<string>();
        public static IList<string> DialogueFilesList { get; } = new List<string>();
        public static IList<string> HudTexturesList { get; } = new List<string>();
        public ObservableCollection<GameHeadersFile> TPTeamNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeechEvents { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeakerNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPShipNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPInGameMessages { get; } = new();
        public ObservableCollection<GameHeadersFile> TPJournalTitles { get; } = new();
        public ObservableCollection<GameHeadersFile> TPObjectiveTasks { get; } = new();
        public ObservableCollection<GameHeadersFile> TPSpeechEventsJournals { get; } = new();
        public ObservableCollection<GameHeadersFile> TPMapTextItems { get; } = new();
        public ObservableCollection<GameHeadersFile> TPWorldNames { get; } = new();
        public ObservableCollection<GameHeadersFile> TPWorldDescriptions { get; } = new();
        [XmlIgnore]
        public string EffectsDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string FlagTexturesDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string GameHeadersFiles { get; set; } = string.Empty;
        [XmlIgnore]
        public string GameStringsEnglish { get; set; } = string.Empty;
        [XmlIgnore]
        public string GuiTexturesDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string MeshesDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string SoundDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string WorldFilesDirectory { get; set; } = string.Empty;
        [XmlIgnore]
        public string WorldObjectFilesDirectory { get; set; } = string.Empty;

        partial void OnTpGamePathChanged(string? old, string newv)
        {
            UpdateAppSettingsStrings();
            UpdateAppSettingsFolders();
            UpdateGameHeadersFilesList();
            UpdateDialogueFilesList();
            UpdateHudTexturesList();
            UpdateFlagTexturesList();
            UpdateEffectsList();
            UpdateStringsDictionnaries();
            UpdateWorldObjectTypeList();
            UpdateSinglePlayerMissionsList();
            UpdateGuiTexturesList();
            UpdateMusicsList();
            UpdateMeshesList();
        }

        public void UpdateStringsDictionnaries()
        {
            var gameStrings = new Dictionary<string, string>();
            UpdateGameStringsDictionary(gameStrings);
            UpdateTeamNamesDictionary(gameStrings);
            UpdateSpeechEventDictionnary(gameStrings);
            UpdateSpeakersDictionnary(gameStrings);
            UpdateShipNamesDictionnary(gameStrings);
            UpdateInGameMessagesDictionary(gameStrings);
            UpdateJournalTitlesDictionary(gameStrings);
            UpdateObjectiveTasksDictionary(gameStrings);
            UpdateSpeechEventsJournalsDictionary(gameStrings);
            UpdateMapTextItemsDictionary(gameStrings);
            UpdateWorldNamesDictionary(gameStrings);
            UpdateWorldDescriptionsDictionary(gameStrings);
        }

        private void UpdateAppSettingsStrings()
        {
            var stringsFilePath = Path.Combine(this.TpGamePath, "Strings.ini");
            if (File.Exists(stringsFilePath))
            {
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(stringsFilePath)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.StartsWith("Game Header Files:"))
                            {
                                var gameHeadersFiles = Path.Combine(TpGamePath, line.Substring("Game Header Files:".Length).Trim());
                                this.GameHeadersFiles = gameHeadersFiles;
                            }
                            if (line.StartsWith("Game Strings - English:"))
                            {
                                var gameStringsFile = Path.Combine(TpGamePath, line.Substring("Game Strings - English:".Length).Trim());
                                this.GameStringsEnglish = gameStringsFile;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading Strings.ini: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Strings.ini file not found in the selected folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateAppSettingsFolders()
        {
            var appSettingsFile = Path.Combine(this.TpGamePath, "AppSettings.ini");
            if (File.Exists(appSettingsFile))
            {
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(appSettingsFile)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.StartsWith("SOUND DIRECTORY:"))
                            {
                                SoundDirectory = Path.Combine(TpGamePath, line.Substring("SOUND DIRECTORY:".Length).Trim()); ;
                            }
                            else if (line.StartsWith("WORLD OBJECT FILES DIRECTORY:"))
                            {
                                WorldObjectFilesDirectory = Path.Combine(TpGamePath, line.Substring("WORLD OBJECT FILES DIRECTORY:".Length).Trim()); ;
                            }
                            else if (line.StartsWith("FLAG TEXTURES:"))
                            {
                                FlagTexturesDirectory = Path.Combine(TpGamePath, line.Substring("FLAG TEXTURES:".Length).Trim());
                            }
                            else if (line.StartsWith("EFFECTS DIRECTORY:"))
                            {
                                EffectsDirectory = Path.Combine(TpGamePath, line.Substring("EFFECTS DIRECTORY:".Length).Trim());
                            }
                            else if (line.StartsWith("WORLD FILES:"))
                            {
                                WorldFilesDirectory = Path.Combine(TpGamePath, line.Substring("WORLD FILES:".Length).Trim());
                            }
                            else if (line.StartsWith("GUI TEXTURES:"))
                            {
                                GuiTexturesDirectory = Path.Combine(TpGamePath, line.Substring("GUI TEXTURES:".Length).Trim());
                            }
                            else if (line.StartsWith("MESH DATA:"))
                            {
                                MeshesDirectory = Path.Combine(TpGamePath, line.Substring("MESH DATA::".Length).Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading AppSettings.ini: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("AppSettings.ini file not found in the selected folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateGameHeadersFilesList()
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                GameHeadersFile.GameHeadersFilesList.Clear();
                foreach (var file in Directory.GetFiles(GameHeadersFiles, "*.h"))
                {
                    var fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        GameHeadersFile.GameHeadersFilesList.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateMeshesList()
        {
            Meshes.Clear();
            if (Directory.Exists(MeshesDirectory))
            {
                foreach (var file in Directory.GetFiles(MeshesDirectory, "*.mdb"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Meshes.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Directory '{MeshesDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateMusicsList()
        {
            Musics.Clear();
            var directory = Path.Combine(SoundDirectory, "Music");
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.ogg"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Musics.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Directory '{directory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateGuiTexturesList()
        {
            GuiTextures.Clear();
            if (Directory.Exists(GuiTexturesDirectory))
            {
                var invalidTextureNames = new List<string>();
                foreach (var file in Directory.GetFiles(GuiTexturesDirectory, "*.tga"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    GuiTexturesFull.Add(fileName);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (Regex.IsMatch(fileName, @"_\d_\d$"))
                        {
                            var baseTextureName = Regex.Replace(fileName, @"_\d_\d$", "");
                            if (!GuiTextures.Contains(baseTextureName))
                                GuiTextures.Add(baseTextureName);
                        }
                        else
                        {
                            invalidTextureNames.Add(fileName);
                        }
                    }
                }
                foreach(var invalidTextureName in invalidTextureNames)
                {
                    for (int i = GuiTextures.Count - 1; i >= 0; i--)
                    {
                        if (invalidTextureName.StartsWith(GuiTextures[i]))
                            GuiTextures.RemoveAt(i);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Directory '{GuiTexturesDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateSinglePlayerMissionsList()
        {
            SinglePlayerMissions.Clear();
            var directory = Path.Combine(WorldFilesDirectory, "SinglePlayer");
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.twt"))
                {
                    var fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        SinglePlayerMissions.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Directory '{directory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateDialogueFilesList()
        {
            DialogueFilesList.Clear();
            var dialogueDirectory = Path.Combine(SoundDirectory, "Dialogue");
            if (Directory.Exists(dialogueDirectory))
            {
                foreach (var file in Directory.GetFiles(dialogueDirectory, "*.ogg"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        DialogueFilesList.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Dialogue directory '{dialogueDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateHudTexturesList()
        {
            var hudFilePath = Path.Combine(this.TpGamePath, "hud.hdt");
            if (File.Exists(hudFilePath))
            {
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(hudFilePath), Encoding.GetEncoding("Windows-1252")))
                    {
                        HudTexturesList.Clear();
                        for (int i = 0; i < 185; i++)
                        {
                            if (!reader.EndOfStream)
                                reader.ReadLine(); // Skip the first 185 lines
                        }
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            // skip one line
                            if(!reader.EndOfStream)
                                reader.ReadLine();
                            if (!reader.EndOfStream)
                            {
                                var line2 = reader.ReadLine();
                                if (line2.Trim().StartsWith("TextureName"))
                                {
                                    var lineSplit = line.Split(' ');
                                    if(lineSplit.Length > 1)
                                    {
                                        // Extract the face texture name
                                        var faceTextureName = lineSplit[1];
                                        if (!string.IsNullOrEmpty(faceTextureName))
                                        {
                                            HudTexturesList.Add(faceTextureName);
                                        }
                                    }
                                    // skip another 24 lines
                                    for (int j = 0; j < 24; j++)
                                    {
                                        if (!reader.EndOfStream)
                                            reader.ReadLine();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading hud.hdt: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("hud.hdt file not found in the selected folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateFlagTexturesList()
        {
            FlagTextures.Clear();
            if (Directory.Exists(FlagTexturesDirectory))
            {
                foreach (var file in Directory.GetFiles(FlagTexturesDirectory, "*.tga"))
                {
                    var fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        FlagTextures.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Flag textures directory '{FlagTexturesDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateEffectsList()
        {
            Effects.Clear();
            if (Directory.Exists(EffectsDirectory))
            {
                foreach (var file in Directory.GetFiles(EffectsDirectory, "*.eft"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Effects.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Effects directory '{EffectsDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateGameStringsDictionary(Dictionary<string, string> gameStrings)
        {
            if (File.Exists(GameStringsEnglish))
            {
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(GameStringsEnglish), Encoding.GetEncoding("Windows-1252")))
                    {
                        string key, value;
                        while ((key = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrEmpty(key) || key.StartsWith("\"") || key.EndsWith("\"") || key.StartsWith("//"))
                                continue;
                            if ((value = reader.ReadLine()) != null && (value = value.Trim('"')) != null)
                                gameStrings[key] = value;
                            else
                                gameStrings[key] = key;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading game strings file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Game strings file '{GameStringsEnglish}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateTeamNamesDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.TeamNames.Clear();
                foreach (var file in this.TPTeamNames)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var teamName = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(teamName, out var teamString))
                                    {
                                        StringDictionnary.TeamNames[teamName] = teamString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateSpeechEventDictionnary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.SpeechEvents.Clear();
                foreach (var file in this.TPSpeechEvents)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var parts = line.Substring(8).Split(' ');
                                    if (parts.Length >= 2)
                                    {
                                        var eventName = parts[0];
                                        if (gameStrings.TryGetValue(eventName, out var eventText))
                                        {
                                            StringDictionnary.SpeechEvents[eventName] = eventText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateSpeakersDictionnary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.SpeakerNames.Clear();
                foreach (var file in this.TPSpeakerNames)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var parts = line.Substring(8).Split(' ');
                                    if (parts.Length >= 2)
                                    {
                                        var speakerName = parts[0];
                                        if (gameStrings.TryGetValue(speakerName, out var speakerText))
                                        {
                                            StringDictionnary.SpeakerNames[speakerName] = speakerText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateShipNamesDictionnary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.ShipNames.Clear();
                foreach (var file in this.TPShipNames)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var parts = line.Substring(8).Split(' ');
                                    if (parts.Length >= 2)
                                    {
                                        var shipName = parts[0];
                                        if (gameStrings.TryGetValue(shipName, out var shipDisplayName))
                                        {
                                            StringDictionnary.ShipNames[shipName] = shipDisplayName;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateInGameMessagesDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.InGameMessagesDictionnary.Clear();
                foreach (var file in this.TPInGameMessages)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.InGameMessagesDictionnary[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateJournalTitlesDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.JournalTitles.Clear();
                foreach (var file in this.TPJournalTitles)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.JournalTitles[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateObjectiveTasksDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.ObjectiveTasks.Clear();
                foreach (var file in this.TPObjectiveTasks)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.ObjectiveTasks[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateSpeechEventsJournalsDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.SpeechEventsJournals.Clear();
                foreach (var file in this.TPSpeechEventsJournals)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.SpeechEventsJournals[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateMapTextItemsDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.MapTextItems.Clear();
                foreach (var file in this.TPMapTextItems)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.MapTextItems[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateWorldNamesDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.WorldNames.Clear();
                foreach (var file in this.TPWorldNames)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.WorldNames[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateWorldDescriptionsDictionary(Dictionary<string, string> gameStrings)
        {
            if (Directory.Exists(GameHeadersFiles))
            {
                StringDictionnary.WorldDescriptions.Clear();
                foreach (var file in this.TPWorldDescriptions)
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(Path.Combine(GameHeadersFiles, file.FileName))))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("#define"))
                                {
                                    var defineString = line.Substring(8).Split(' ')[0]; // 8 is the length of "#define "
                                    if (gameStrings.TryGetValue(defineString, out var gameString))
                                    {
                                        StringDictionnary.WorldDescriptions[defineString] = gameString;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading header file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Game headers folder '{GameHeadersFiles}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateWorldObjectTypeList()
        {
            if (Directory.Exists(WorldObjectFilesDirectory))
            {
                WotGridItem.WotTypes.Clear();
                foreach (var file in Directory.GetFiles(WorldObjectFilesDirectory, "*.wot"))
                {
                    try
                    {
                        using (var reader = new StreamReader(File.OpenRead(file)))
                        {
                            var wotGridItem = new WotGridItem();
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("Type String"))
                                {
                                    string typeString = line.Substring("Type String:".Length).Trim('\'');

                                    wotGridItem.Type = typeString;
                                    // Attempt to load the image for the world object type
                                    if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "ImageData/WorldObjects/" + typeString + ".png"))
                                    {
                                        wotGridItem.Image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ImageData/WorldObjects/" + typeString + ".png"));
                                    }
                                    else
                                    {
                                        wotGridItem.Image = new BitmapImage(new Uri("pack://application:,,,/Images/WotPlaceholder.png"));
                                    }
                                }
                                else if(line.Equals("Definition String 'CUSTOMINFODEFINITION'"))
                                {
                                    reader.ReadLine();
                                    if (!reader.EndOfStream)
                                    {
                                        var factoryType = reader.ReadLine().Substring(19).Trim('\'');
                                        if(Enum.TryParse<CustomInfoDefinition>(factoryType, out var customInfoDefinition))
                                        {
                                            wotGridItem.CustomInfoDefinition = customInfoDefinition;
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid custom info definition.");
                                        }
                                    }
                                }
                            }
                            WotGridItem.WotTypes.Add(wotGridItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading world object file '{file}': {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"World object files directory '{WorldObjectFilesDirectory}' does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddDefaultHeadersToLists()
        {
            TPTeamNames.Clear();
            TPSpeechEvents.Clear();
            TPSpeakerNames.Clear();
            TPTeamNames.Add(new("TPTEAMNAMES_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS000_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS001_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS002_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS003_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS004_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTS005_GameStrings.h"));
            TPSpeechEvents.Add(new("TPSPEECHEVENTTUTORIAL_GameStrings.h"));
            TPSpeakerNames.Add(new("TPSPEAKERNAMES_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES00_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES01_GameStrings.h"));
            TPShipNames.Add(new("TPCAMPAIGNSHIPNAMES02_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMENAVY00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMENAVY01_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPIRATE00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPIRATE01_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPROCYON00_GameStrings.h"));
            TPShipNames.Add(new("TPSHIPNAMEPROCYON01_GameStrings.h"));
            TPInGameMessages.Add(new("TPINGAMEMESSAGE_GameStrings.h"));
            TPJournalTitles.Add(new("TPJOURNALSCREEN_GameStrings.h"));
            TPObjectiveTasks.Add(new("TPOBJECTIVES_GameStrings.h"));
            TPObjectiveTasks.Add(new("TPOBJECTIVES2_GameStrings.h"));
            TPSpeechEventsJournals.Add(new("TPSPEECHEVENTSJOURNALS_GameStrings.h"));
            TPMapTextItems.Add(new("TPMAPTEXTITEMS_GameStrings.h"));
            TPWorldNames.Add(new("TPWORLDNAMES_GameStrings.h"));
            TPWorldNames.Add(new("TPJOURNALSCREEN_GameStrings.h"));
            TPWorldDescriptions.Add(new("TPWORLDDESCRIPTION_GameStrings.h"));
        }

        public AppSettings Load()
        {
            try
            {
                using (var reader = new StreamReader(File.OpenRead(filename)))
                {
                    var xmls = new XmlSerializer(typeof(AppSettings));
                    return (AppSettings)xmls.Deserialize(reader);
                }
            }
            catch
            {
                AddDefaultHeadersToLists();
                Save();
                return this;
            }
        }

        public void Save()
        {
            try
            {
                using (var writer = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.ReadWrite)))
                {
                    var xmls = new XmlSerializer(typeof(AppSettings));
                    xmls.Serialize(writer, this);
                }
            }
            catch { }
        }
    }
}
