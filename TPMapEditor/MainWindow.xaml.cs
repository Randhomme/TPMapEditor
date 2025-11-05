using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPMapEditor.Controls;
using TPMapEditor.Converter;
using TPMapEditor.Data;
using TPMapEditor.Dialogs;
using TPMapEditor.Settings;

namespace TPMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        private Point moveActionPoint;
        private AppSettings settings;
        [ObservableProperty]
        private WotGridItem? selectedWotGridItem;
        [ObservableProperty]
        private WorldObject? selectedWorldObject;
        [ObservableProperty]
        private Player? selectedPlayer;
        [ObservableProperty]
        private WaypointPathPoint? selectedWaypointPathPoint;
        [ObservableProperty]
        private WaypointPath? selectedWaypointPath;
        [ObservableProperty]
        private WorldPolygonPoint? selectedWorldPolygonPoint;
        [ObservableProperty]
        private WorldPolygon? selectedWorldPolygon;
        [ObservableProperty]
        private WorldPoint? selectedWorldPoint;
        [ObservableProperty]
        private WorldPointSet? selectedPointSet;
        [ObservableProperty]
        private ObjectivePoint? selectedObjectivePoint;
        [ObservableProperty]
        private MapTextPoint? selectedMapTextPoint;

        public IList<WorldObject> SelectedWorldObjects { get; }
        public IList<Player> SelectedPlayers { get; }
        public IList<WaypointPathPoint> SelectedWaypointPathPoints { get; }
        public IList<WaypointPath> SelectedWaypointPaths { get; }
        public IList<WorldPolygonPoint> SelectedWorldPolygonPoints { get; }
        public IList<WorldPolygon> SelectedWorldPolygons { get; }
        public IList<WorldPoint> SelectedWorldPoints { get; }
        public IList<WorldPointSet> SelectedPointSets { get; }
        public IList<ObjectivePoint> SelectedObjectivePoints { get; }
        public IList<MapTextPoint> SelectedMapTextPoints { get; }

        public WorldMap Map { get; }
        
        private void LoadSettings()
        {
            settings = settings.Load();
        }

        public MainWindow()
        {
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            settings = new AppSettings();
            LoadSettings();
            Map = new WorldMap();
            SelectedWorldObjects = new List<WorldObject>();
            SelectedPlayers = new List<Player>();
            SelectedWaypointPathPoints = new List<WaypointPathPoint>();
            SelectedWaypointPaths = new List<WaypointPath>();
            SelectedWorldPolygonPoints = new List<WorldPolygonPoint>();
            SelectedWorldPolygons = new List<WorldPolygon>();
            SelectedWorldPoints = new List<WorldPoint>();
            SelectedPointSets = new List<WorldPointSet>();
            SelectedObjectivePoints = new List<ObjectivePoint>();
            SelectedMapTextPoints = new List<MapTextPoint>();
            InitializeComponent();
            WotRadioButton.IsChecked = true;
            HidePlayerElements();
            HidePathElements();
        }

        #region MenuCommand

        [RelayCommand]
        private void OnMapImport()
        {
            var ofd = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = ".twt",
                Filter = "Map file (.twt)|*.twt",
                Title = "Select a map file",
            };
            if (ofd.ShowDialog(this) == true)
            {
                if(MessageBox.Show("The current map will be cleared. Continue ?", "Map import", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Map.Reset();
                    DataImport.ReadMapFileAndAddData(ofd.FileName, Map);
                }
            }
        }

        [RelayCommand]
        private void OnWorldInfoEdit()
        {
            new WorldInfoDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(this, Map.Size, Map.ZSize);
            if (msd.ShowDialog() == true)
            {
                Map.Size = msd.Size;
                Map.ZSize = msd.ZSize;
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new PlayerDialog(this, Map).ShowDialog();
            if (SelectedPlayer != null)
            {
                if (!Map.Players.Contains(SelectedPlayer))
                    SelectedPlayer = null;
            }
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new WorldObjectDialog(this, Map).ShowDialog();
            if (SelectedWorldObject != null)
            {
                if (!Map.WorldObjects.Contains(SelectedWorldObject))
                    SelectedWorldObject = null;
            }
        }

        [RelayCommand]
        private void OnTeamsEdit()
        {
            if (StringDictionnary.TeamNames.Count <= 0)
                MessageBox.Show("You need to add at least one team name to create a team.");
            var td = new TeamsDialog(this, Map);
            td.ShowDialog();
        }

        [RelayCommand]
        private void OnGroupsEdit()
        {
            new GroupDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnFlagsEdit()
        {
            new FlagDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            if (Map.Players.Count > 1)
                new PlayerAllianceDialog(this, Map).ShowDialog();
            else
                MessageBox.Show("You need at least 2 players to create alliances.");
        }

        [RelayCommand]
        private void OnTimersEdit()
        {
            new TimerDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnSpeechEventsEdit()
        {
            new SpeechEventDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            new WorldRuleDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnObjectiveTasksEdit()
        {
            new ObjectiveTaskDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnJournalEntriesEdit()
        {
            new JournalEntryDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWaypointPathsEdit()
        {
            new WaypointPathDialog(this, Map).ShowDialog();
            if (SelectedWaypointPath != null)
            {
                if (!Map.WaypointPaths.Contains(SelectedWaypointPath))
                {
                    SelectedWaypointPathPoint = null;
                    SelectedWaypointPath = null;
                }
            }
        }

        [RelayCommand]
        private void OnAppSettingsEdit()
        {
            var asd = new AppSettingsDialog(this, settings);
            asd.ShowDialog();
        }

        #endregion MenuCommand

        #region MainWindow events

        //uncheck radio button on mouse down
        private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton!.IsChecked == true && e.ChangedButton == MouseButton.Left)
            {
                radioButton.IsChecked = false;
                e.Handled = true;
            }
        }

        //uncheck radio button when Enable changed
        private void RadioButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var rb = (RadioButton)sender;
            rb.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Unsaved changes will be lost. Exit now ?",
                "Exit?", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            settings.Save();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(settings.TpGamePath))
            {
                MessageBox.Show("You should set the TPGame path in the application settings before using the map editor.", "TPGame Path Not Set", MessageBoxButton.OK, MessageBoxImage.Warning);
                OnAppSettingsEdit();
            }
        }

        private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            if (!ctrlPressed)
                return;

            if (e.Delta < 0 && MapScrollViewer.ScrollableHeight == 0 && MapScrollViewer.ScrollableWidth == 0)
                return;

            const double zoomFactor = 1.1;
            double scale = (e.Delta > 0) ? zoomFactor : 1 / zoomFactor;

            Point mousePos = e.GetPosition(MapScrollViewer);

            double absoluteX = MapScrollViewer.HorizontalOffset + mousePos.X;
            double absoluteY = MapScrollViewer.VerticalOffset + mousePos.Y;
            double realScale = MapViewBox.ActualWidth / MapGrid.ActualWidth;
            double newScale = realScale * scale;

            ZoomTransform.ScaleX = newScale;
            ZoomTransform.ScaleY = newScale;

            double newAbsoluteX = absoluteX * scale;
            double newAbsoluteY = absoluteY * scale;

            MapScrollViewer.ScrollToHorizontalOffset(newAbsoluteX - mousePos.X);
            MapScrollViewer.ScrollToVerticalOffset(newAbsoluteY - mousePos.Y);

            e.Handled = true;
        }

        #endregion

        #region WorldObject

        private void WotRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWotElements();
        }

        private void WotRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWotElements();
        }

        private void ShowWotElements()
        {
            WotGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldObjectItemsControl, 1);
            WorldObjectItemsControl.Opacity = 1;
            WorldObjectItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].IsSelected = true;
            //if (SelectedWorldObject != null)
            //    SelectedWorldObject.IsLastSelected = true;
            MoveCheckBox.Checked += MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldObject();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click += DeleteWorldObjectButton_Click;
        }

        private void HideWotElements()
        {
            WotDataGrid.SelectedItem = null;
            WotGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldObjectItemsControl, 0);
            WorldObjectItemsControl.Opacity = 0.5;
            WorldObjectItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].IsSelected = false;
            MoveCheckBox.Checked -= MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldObject();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldObjectButton_Click;
        }

        private void MapGridOutsideWorldObjectPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldObjectPreviewControl, mousePos.X - WorldObjectPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldObjectPreviewControl, mousePos.Y - WorldObjectPreviewControl.ActualHeight / 2);
        }

        private void WorldObjectPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Map.WorldObjects.Add(new(SelectedWotGridItem!, Canvas.GetLeft(WorldObjectPreviewControl) + WorldObjectPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldObjectPreviewControl) - WorldObjectPreviewControl.ActualHeight / 2, WotSliderRotate.Value));
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldObjectPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WotDataGrid.SelectedItem = null;
        }

        private void OnWorldObjectClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldObject clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemoveWorldObjectFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectWorldObjectFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddWorldObjectToSelection(clickedObject);
                    }
                }
                else
                {
                    ClearWorldObjectSelection();
                    AddWorldObjectToSelection(clickedObject);
                }

                if(MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldObjectSelection();
        }

        private void AddWorldObjectToSelection(WorldObject worldObject)
        {
            if (SelectedWorldObject != null)
                SelectedWorldObject.IsLastSelected = false;
            worldObject.IsSelected = worldObject.IsLastSelected = true;
            SelectedWorldObjects.Add(worldObject);
            SelectedWorldObject = worldObject;
        }

        private void SelectWorldObjectFromSelection(WorldObject worldObject)
        {
            if (SelectedWorldObject != null)
                SelectedWorldObject.IsLastSelected = false;
            worldObject.IsLastSelected = true;
            SelectedWorldObject = worldObject;
        }

        private void RemoveWorldObjectFromSelection(WorldObject worldObject)
        {
            worldObject.IsSelected = worldObject.IsLastSelected = false;
            SelectedWorldObjects.Remove(worldObject);
            SelectedWorldObject = null;
        }

        private void ClearWorldObjectSelection()
        {
            foreach (var v in SelectedWorldObjects)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
        }

        private void MoveWorldObjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldObject();
        }

        private void MoveWorldObjectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldObject();
        }

        private void EnableMoveWorldObject()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldObject()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldObject_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldObject_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldObject_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldObjects.Count; i++)
            {
                var selectedObject = SelectedWorldObjects[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldObjectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var worldObject in SelectedWorldObjects)
            {
                Map.WorldObjects.Remove(worldObject);
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
        }

        #endregion

        #region Player

        private void PlayerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPlayerElements();
        }

        private void PlayerRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePlayerElements();
        }

        private void ShowPlayerElements()
        {
            PlayerGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(PlayerItemsControl, 1);
            PlayerItemsControl.Opacity = 1;
            PlayerItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].IsSelected = true;
            //if (SelectedPlayer != null)
            //    SelectedPlayer.IsLastSelected = true;
            MoveCheckBox.Checked += MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked += MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMovePlayer();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click += DeletePlayerPointButton_Click;
        }

        private void HidePlayerElements()
        {
            AddPlayerCheckBox.IsChecked = false;
            PlayerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(PlayerItemsControl, 0);
            PlayerItemsControl.Opacity = 0.5;
            PlayerItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].IsSelected = false;
            MoveCheckBox.Checked -= MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked -= MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMovePlayer();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click -= DeletePlayerPointButton_Click;
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is Player clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemovePlayerFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectPlayerFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddPlayerToSelection(clickedObject);
                    }
                }
                else
                {
                    ClearPlayerSelection();
                    AddPlayerToSelection(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void EditPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlayer != null)
            {
                var cp = new ColorPicker(this, SelectedPlayer.Color);
                if (cp.ShowDialog() == true)
                    SelectedPlayer.Color = cp.NewColor;
            }
        }

        private void MapGridOutsidePlayerPreview_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(PlayerPreviewControl, mousePos.X - PlayerPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PlayerPreviewControl, mousePos.Y - PlayerPreviewControl.ActualHeight / 2);
        }

        private void PlayerPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Map.Players.Add(new(NamedElement.GenerateName("Player", Map.Players), Map, Canvas.GetLeft(PlayerPreviewControl) + PlayerPreviewControl.ActualWidth / 2, -Canvas.GetTop(PlayerPreviewControl) - PlayerPreviewControl.ActualHeight / 2, 0, PlayerSliderRotate.Value, Colors.Red));
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void PlayerPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPlayerCheckBox.IsChecked = false;
        }

        private void MapGridOutsidePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearPlayerSelection();
        }

        private void AddPlayerToSelection(Player player)
        {
            if (SelectedPlayer != null)
                SelectedPlayer.IsLastSelected = false;
            player.IsSelected = player.IsLastSelected = true;
            SelectedPlayers.Add(player);
            SelectedPlayer = player;
        }

        private void SelectPlayerFromSelection(Player player)
        {
            if (SelectedPlayer != null)
                SelectedPlayer.IsLastSelected = false;
            player.IsLastSelected = true;
            SelectedPlayer = player;
        }

        private void RemovePlayerFromSelection(Player player)
        {
            player.IsSelected = player.IsLastSelected = false;
            SelectedPlayers.Remove(player);
            SelectedPlayer = null;
        }

        private void ClearPlayerSelection()
        {
            foreach (var v in SelectedPlayers)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
        }

        private void MovePlayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMovePlayer();
        }

        private void MovePlayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMovePlayer();
        }

        private void EnableMovePlayer()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMovePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMovePlayer_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMovePlayer()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMovePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMovePlayer_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMovePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMovePlayer_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMovePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMovePlayer_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMovePlayer_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedPlayers.Count; i++)
            {
                var selectedWot = SelectedPlayers[i];
                selectedWot.X += x;
                selectedWot.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeletePlayerPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var worldObject in SelectedPlayers)
            {
                Map.Players.Remove(worldObject);
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
        }

        #endregion

        #region WaypointPath

        private void PathRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPathElements();
        }

        private void PathRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePathElements();
        }

        private void ShowPathElements()
        {
            PathGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WaypointPathItemsControl, 1);
            WaypointPathItemsControl.Opacity = 1;
            WaypointPathItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWaypointPathPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWaypointPathPoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click += DeleteWaypointPathPointButton_Click;
        }

        private void HidePathElements()
        {
            //wotPreview.Visibility = Visibility.Collapsed;
            PathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WaypointPathItemsControl, 0);
            WaypointPathItemsControl.Opacity = 0.5;
            WaypointPathItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            MoveCheckBox.Checked -= MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWaypointPathPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWaypointPathPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWaypointPathPointButton_Click;
        }

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPathPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemoveWaypointPathPointFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectWaypointPathPointFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddWaypointPathPointToSelection(clickedObject);
                        //add path if not selected
                        if (!clickedObject.Parent.IsSelected)
                        {
                            AddWaypointPathToSelectionWithoutPoints(clickedObject.Parent);
                        }
                    }
                }
                else
                {
                    ClearWaypointPathPointSelection();
                    AddWaypointPathPointToSelection(clickedObject);
                    //add path if not selected
                    if (!clickedObject.Parent.IsSelected)
                    {
                        AddWaypointPathToSelectionWithoutPoints(clickedObject.Parent);
                    }
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWaypointPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPath clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemoveWaypointPathFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectWaypointPathFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddWaypointPathToSelectionWithPoints(clickedObject);
                    }
                }
                else
                {
                    ClearWaypointPathSelection();
                    AddWaypointPathToSelectionWithPoints(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWaypointPathSelection();
        }

        private void AddWaypointPathToSelectionWithPoints(WaypointPath waypointPath)
        {
            //select path
            if (SelectedWaypointPath != null)
                SelectedWaypointPath.IsLastSelected = false;
            waypointPath.IsSelected = waypointPath.IsLastSelected = true;
            SelectedWaypointPath = waypointPath;
            SelectedWaypointPaths.Add(waypointPath);

            //select points
            foreach(var p in SelectedWaypointPath.Points)
            {
                SelectedWaypointPathPoint = null;
                p.IsLastSelected = false;
                if (!p.IsSelected)
                {
                    p.IsSelected = true;
                    SelectedWaypointPathPoints.Add(p);
                }
            }
        }

        private void AddWaypointPathToSelectionWithoutPoints(WaypointPath waypointPath)
        {
            if (SelectedWaypointPath != null)
                SelectedWaypointPath.IsLastSelected = false;
            waypointPath.IsSelected = waypointPath.IsLastSelected = true;
            SelectedWaypointPath = waypointPath;
            SelectedWaypointPaths.Add(waypointPath);
        }

        private void SelectWaypointPathFromSelection(WaypointPath waypointPath)
        {
            if (SelectedWaypointPath != null)
                SelectedWaypointPath.IsLastSelected = false;
            waypointPath.IsLastSelected = true;
            SelectedWaypointPath = waypointPath;
        }

        private void RemoveWaypointPathFromSelection(WaypointPath waypointPath)
        {
            waypointPath.IsSelected = waypointPath.IsLastSelected = false;
            SelectedWaypointPath = null;
            SelectedWaypointPaths.Remove(waypointPath);

            //remove points from selection
            foreach(var p in waypointPath.Points)
            {
                if (p.IsSelected)
                {
                    p.IsSelected = false;
                    SelectedWaypointPathPoints.Remove(p);
                }
                if(p.IsLastSelected)
                {
                    p.IsLastSelected = false;
                    SelectedWaypointPathPoint = null;
                }

            }
        }

        private void ClearWaypointPathSelection()
        {
            foreach (var v in SelectedWaypointPaths)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWaypointPaths.Clear();
            SelectedWaypointPath = null;
            ClearWaypointPathPointSelection();
        }

        private void AddWaypointPathPointToSelection(WaypointPathPoint waypointPathPoint)
        {
            if (SelectedWaypointPathPoint != null)
                SelectedWaypointPathPoint.IsLastSelected = false;
            waypointPathPoint.IsSelected = waypointPathPoint.IsLastSelected = true;
            SelectedWaypointPathPoints.Add(waypointPathPoint);
            SelectedWaypointPathPoint = waypointPathPoint;
        }

        private void SelectWaypointPathPointFromSelection(WaypointPathPoint waypointPathPoint)
        {
            if (SelectedWaypointPathPoint != null)
                SelectedWaypointPathPoint.IsLastSelected = false;
            waypointPathPoint.IsLastSelected = true;
            SelectedWaypointPathPoint = waypointPathPoint;
        }

        private void RemoveWaypointPathPointFromSelection(WaypointPathPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = waypointPathPoint.IsLastSelected = false;
            SelectedWaypointPathPoints.Remove(waypointPathPoint);
            SelectedWaypointPathPoint = null;
        }

        private void ClearWaypointPathPointSelection()
        {
            foreach (var v in SelectedWaypointPathPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPathPoint = null;
        }

        private void MoveWaypointPathPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWaypointPathPoint();
        }

        private void MoveWaypointPathPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWaypointPathPoint();
        }

        private void EnableMoveWaypointPathPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWaypointPathPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWaypointPathPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWaypointPathPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWaypointPathPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWaypointPathPoints.Count; i++)
            {
                var selectedObject = SelectedWaypointPathPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var p in SelectedWaypointPathPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if(p.Parent.Points.Count == 0)
                {
                    Map.WaypointPaths.Remove(p.Parent);
                }
            }
        }

        private void MapGridOutsideWaypointPathPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(PathPreviewControl, mousePos.X - PathPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PathPreviewControl, mousePos.Y - PathPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(PathPointPreviewControl, mousePos.X - PathPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PathPointPreviewControl, mousePos.Y - PathPointPreviewControl.ActualHeight / 2);
        }

        private void PathPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = new WaypointPath(NamedElement.GenerateName("Path", Map.WaypointPaths), Map);
            var point = new WaypointPathPoint(path, Canvas.GetLeft(PathPreviewControl) + PathPreviewControl.ActualWidth / 2, -Canvas.GetTop(PathPreviewControl) - PathPreviewControl.ActualHeight / 2, 0);
            path.Points.Add(point);
            Map.WaypointPaths.Add(path);
            AddWaypointPathToSelectionWithoutPoints(path);
            AddWaypointPathPointToSelection(point);
            AddPointPathRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void PathPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewPathRadioButton.IsChecked = false;
        }

        private void PathPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWaypointPath != null)
            {
                var point = new WaypointPathPoint(SelectedWaypointPath, Canvas.GetLeft(PathPointPreviewControl) + PathPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(PathPointPreviewControl) - PathPointPreviewControl.ActualHeight / 2, 0);
                AddWaypointPathPointToSelection(point);
                SelectedWaypointPath?.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void PathPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPointPathRadioButton.IsChecked = false;
        }

        private void EditWaypointPathColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWaypointPath != null)
            {
                var cp = new ColorPicker(this, SelectedWaypointPath.Color);
                if (cp.ShowDialog() == true)
                    SelectedWaypointPath.Color = cp.NewColor;
            }
        }

        #endregion

        #region WorldPolygon

        private void PolygonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPolygonElements();
        }

        private void PolygonRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPolygonElements();
        }

        private void ShowWorldPolygonElements()
        {
            PolygonGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPolygonItemsControl, 1);
            WorldPolygonItemsControl.Opacity = 1;
            WorldPolygonItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPolygonPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPolygonPoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click += DeleteWorldPolygonPointButton_Click;
        }

        private void HideWorldPolygonElements()
        {
            PolygonGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPolygonItemsControl, 0);
            WorldPolygonItemsControl.Opacity = 0.5;
            WorldPolygonItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            MoveCheckBox.Checked -= MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPolygonPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPolygonPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPolygonPointButton_Click;
        }

        private void OnWorldPolygonPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygonPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemoveWorldPolygonPointFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectWorldPolygonPointFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddWorldPolygonPointToSelection(clickedObject);
                        //add path if not selected
                        if (!clickedObject.Parent.IsSelected)
                        {
                            AddWorldPolygonToSelectionWithoutPoints(clickedObject.Parent);
                        }
                    }
                }
                else
                {
                    ClearWorldPolygonPointSelection();
                    AddWorldPolygonPointToSelection(clickedObject);
                    //add path if not selected
                    if (!clickedObject.Parent.IsSelected)
                    {
                        AddWorldPolygonToSelectionWithoutPoints(clickedObject.Parent);
                    }
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWorldPolygonClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygon clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    //if already selected and current selected
                    if (clickedObject.IsLastSelected)
                    {
                        RemoveWorldPolygonFromSelection(clickedObject);
                    }
                    //if already selected but not current selected
                    else if (clickedObject.IsSelected)
                    {
                        SelectWorldPolygonFromSelection(clickedObject);
                    }
                    //if not selected
                    else
                    {
                        AddWorldPolygonToSelectionWithPoints(clickedObject);
                    }
                }
                else
                {
                    ClearWorldPolygonSelection();
                    AddWorldPolygonToSelectionWithPoints(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldPolygonSelection();
        }

        private void AddWorldPolygonToSelectionWithPoints(WorldPolygon waypointPath)
        {
            //select path
            if (SelectedWorldPolygon != null)
                SelectedWorldPolygon.IsLastSelected = false;
            waypointPath.IsSelected = waypointPath.IsLastSelected = true;
            SelectedWorldPolygon = waypointPath;
            SelectedWorldPolygons.Add(waypointPath);

            //select points
            foreach (var p in SelectedWorldPolygon.Points)
            {
                SelectedWorldPolygonPoint = null;
                p.IsLastSelected = false;
                if (!p.IsSelected)
                {
                    p.IsSelected = true;
                    SelectedWorldPolygonPoints.Add(p);
                }
            }
        }

        private void AddWorldPolygonToSelectionWithoutPoints(WorldPolygon waypointPath)
        {
            if (SelectedWorldPolygon != null)
                SelectedWorldPolygon.IsLastSelected = false;
            waypointPath.IsSelected = waypointPath.IsLastSelected = true;
            SelectedWorldPolygon = waypointPath;
            SelectedWorldPolygons.Add(waypointPath);
        }

        private void SelectWorldPolygonFromSelection(WorldPolygon waypointPath)
        {
            if (SelectedWorldPolygon != null)
                SelectedWorldPolygon.IsLastSelected = false;
            waypointPath.IsLastSelected = true;
            SelectedWorldPolygon = waypointPath;
        }

        private void RemoveWorldPolygonFromSelection(WorldPolygon waypointPath)
        {
            waypointPath.IsSelected = waypointPath.IsLastSelected = false;
            SelectedWorldPolygon = null;
            SelectedWorldPolygons.Remove(waypointPath);

            //remove points from selection
            foreach (var p in waypointPath.Points)
            {
                if (p.IsSelected)
                {
                    p.IsSelected = false;
                    SelectedWorldPolygonPoints.Remove(p);
                }
                if (p.IsLastSelected)
                {
                    p.IsLastSelected = false;
                    SelectedWorldPolygonPoint = null;
                }

            }
        }

        private void ClearWorldPolygonSelection()
        {
            foreach (var v in SelectedWorldPolygons)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPolygons.Clear();
            SelectedWorldPolygon = null;
            ClearWorldPolygonPointSelection();
        }

        private void AddWorldPolygonPointToSelection(WorldPolygonPoint waypointPathPoint)
        {
            if (SelectedWorldPolygonPoint != null)
                SelectedWorldPolygonPoint.IsLastSelected = false;
            waypointPathPoint.IsSelected = waypointPathPoint.IsLastSelected = true;
            SelectedWorldPolygonPoints.Add(waypointPathPoint);
            SelectedWorldPolygonPoint = waypointPathPoint;
        }

        private void SelectWorldPolygonPointFromSelection(WorldPolygonPoint waypointPathPoint)
        {
            if (SelectedWorldPolygonPoint != null)
                SelectedWorldPolygonPoint.IsLastSelected = false;
            waypointPathPoint.IsLastSelected = true;
            SelectedWorldPolygonPoint = waypointPathPoint;
        }

        private void RemoveWorldPolygonPointFromSelection(WorldPolygonPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = waypointPathPoint.IsLastSelected = false;
            SelectedWorldPolygonPoints.Remove(waypointPathPoint);
            SelectedWorldPolygonPoint = null;
        }

        private void ClearWorldPolygonPointSelection()
        {
            foreach (var v in SelectedWorldPolygonPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygonPoint = null;
        }

        private void MoveWorldPolygonPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldPolygonPoint();
        }

        private void MoveWorldPolygonPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldPolygonPoint();
        }

        private void EnableMoveWorldPolygonPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPolygonPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPolygonPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldPolygonPoints.Count; i++)
            {
                var selectedObject = SelectedWorldPolygonPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPolygonPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedWorldPolygonPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPolygons.Remove(p.Parent);
                }
            }
        }

        private void MapGridOutsideWorldPolygonPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldPolygonPreviewControl, mousePos.X - WorldPolygonPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPolygonPreviewControl, mousePos.Y - WorldPolygonPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WorldPolygonPointPreviewControl, mousePos.X - WorldPolygonPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPolygonPointPreviewControl, mousePos.Y - WorldPolygonPointPreviewControl.ActualHeight / 2);
        }

        private void WorldPolygonPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var polygon = new WorldPolygon(NamedElement.GenerateName("Polygon", Map.WorldPolygons), Map);
            var point = new WorldPolygonPoint(polygon, Canvas.GetLeft(WorldPolygonPreviewControl) + WorldPolygonPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPreviewControl) - WorldPolygonPreviewControl.ActualHeight / 2);
            polygon.Points.Add(point);
            Map.WorldPolygons.Add(polygon);
            AddWorldPolygonToSelectionWithoutPoints(polygon);
            AddWorldPolygonPointToSelection(point);
            AddPointPolygonRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewPolygonRadioButton.IsChecked = false;
        }

        private void WorldPolygonPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                var point = new WorldPolygonPoint(SelectedWorldPolygon, Canvas.GetLeft(WorldPolygonPointPreviewControl) + WorldPolygonPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPointPreviewControl) - WorldPolygonPointPreviewControl.ActualHeight / 2);
                AddWorldPolygonPointToSelection(point);
                SelectedWorldPolygon?.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPointPolygonRadioButton.IsChecked = false;
        }

        private void EditWorldPolygonColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                var cp = new ColorPicker(this, SelectedWorldPolygon.Color);
                if (cp.ShowDialog() == true)
                    SelectedWorldPolygon.Color = cp.NewColor;
            }
        }

        #endregion

        //TODO

        private void PointRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ObjectivePointRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ObjectivePointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void MapTextPointRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MapTextPointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
