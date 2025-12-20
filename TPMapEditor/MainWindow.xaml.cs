using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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
        private DateTime lastWheelTime = DateTime.MinValue;
        private AppSettings settings;
        [ObservableProperty]
        private WorldObjectType? selectedWorldObjectType;
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
        private WorldPointSet? selectedWorldPointSet;
        [ObservableProperty]
        private ObjectivePoint? selectedObjectivePoint;
        [ObservableProperty]
        private MapTextPoint? selectedMapTextPoint;
        [ObservableProperty]
        private UIElement? selectedElement;

        public IList<WorldObject> SelectedWorldObjects { get; }
        public IList<Player> SelectedPlayers { get; }
        public IList<WaypointPathPoint> SelectedWaypointPathPoints { get; }
        public IList<WaypointPath> SelectedWaypointPaths { get; }
        public IList<WorldPolygonPoint> SelectedWorldPolygonPoints { get; }
        public IList<WorldPolygon> SelectedWorldPolygons { get; }
        public IList<WorldPoint> SelectedWorldPoints { get; }
        public IList<WorldPointSet> SelectedWorldPointSets { get; }
        public IList<ObjectivePoint> SelectedObjectivePoints { get; }
        public IList<MapTextPoint> SelectedMapTextPoints { get; }

        public WorldMap Map { get; }
        
        private void LoadSettings()
        {
            settings = settings.Load();
        }

        public MainWindow()
        {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
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
            SelectedWorldPointSets = new List<WorldPointSet>();
            SelectedObjectivePoints = new List<ObjectivePoint>();
            SelectedMapTextPoints = new List<MapTextPoint>();
            InitializeComponent();
            Title = $"{Title} v{version.Major}.{version.Minor}.{version.Build}";
            WorldObjectRadioButton.IsChecked = true;
            HidePlayerElements();
            HidePathElements();
            HideWorldPolygonElements();
            HideWorldPointSetElements();
            HideObjectivePointElements();
            HideMapTextPointElements();
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
                    var progressDialog = new ProgressDialog(this, "Importing map");
                    Map.Reset();
                    ClearSelections();
                    var _lock = new object();
                    Map.EnableCollectionSynchronization(_lock);
                    Task.Run(() =>
                    {
                        using (var di = new DataImport(ofd.FileName, Map, progressDialog.Progress, progressDialog.ProgressOperation, _lock))
                        {
                            di.ReadMapFileAndAddData();
                        }
                        progressDialog.CanClose = true;
                    });
                    progressDialog.ShowDialog();
                    Map.DisableCollectionSynchronization();
                }
            }
        }

        [RelayCommand]
        private void OnMapExport()
        {
            var sfd = new SaveFileDialog()
            {
                DefaultExt = ".twt",
                Filter = "Map file (.twt)|*.twt",
                Title = "Save your map file",
            };
            if (sfd.ShowDialog(this) == true)
            {
                var progressDialog = new ProgressDialog(this, "Exporting map");
                Task.Run(() =>
                {
                    using (var de = new DataExport(sfd.FileName, Map, progressDialog.Progress, progressDialog.ProgressOperation))
                    {
                        de.CreateMapFileAndWriteData();
                    }
                    progressDialog.CanClose = true;
                });
                progressDialog.ShowDialog();
            }
        }

        [RelayCommand]
        private void OnResetMap()
        {
            if (MessageBox.Show("The current map will be cleared. Continue ?", "Map reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ClearSelections();
                Map.Reset();
            }
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(this, "Map size", Map.Size, Map.ZSize, Map.WorldBuffer);
            if (msd.ShowDialog() == true)
            {
                Map.Size = msd.Size;
                Map.ZSize = msd.ZSize;
                Map.WorldBuffer = msd.WorldBuffer;
            }
        }

        [RelayCommand]
        private void OnWorldInfoEdit()
        {
            new WorldInfoDialog(this, "World info", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnFlagsEdit()
        {
            new FlagDialog(this, "Flags", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnGroupsEdit()
        {
            new CollectionEditorDialog(this, "Groups", Map.Groups, () => new Group(Map)).ShowDialog();
        }

        [RelayCommand]
        private void OnJournalEntriesEdit()
        {
            new JournalEntryDialog(this, "Journal entries", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnMapTextPointsEdit()
        {
            new CollectionEditorDialog(this, "Map text points", Map.MapTextPoints, () => new MapTextPoint(Map, NamedElement.GenerateName("MapTextPoint", Map.MapTextPoints), StringDictionnary.MapTextItems.Keys.FirstOrDefault())).ShowDialog();
            if (SelectedMapTextPoint != null)
            {
                if (!Map.MapTextPoints.Contains(SelectedMapTextPoint))
                {
                    SelectedMapTextPoint = null;
                    if (MapTextPointRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnObjectiveTasksEdit()
        {
            new CollectionEditorDialog(this, "Objective tasks", Map.ObjectiveTasks, () => new ObjectiveTask(Map, NamedElement.GenerateName("ObjectiveTask", Map.ObjectiveTasks), StringDictionnary.ObjectiveTasks.Keys.FirstOrDefault())).ShowDialog();
        }

        [RelayCommand]
        private void OnObjectivePointsEdit()
        {
            new CollectionEditorDialog(this, "Objective points", Map.ObjectivePoints, () => new ObjectivePoint(Map)).ShowDialog();
            if (SelectedObjectivePoint != null)
            {
                if (!Map.ObjectivePoints.Contains(SelectedObjectivePoint))
                {
                    SelectedObjectivePoint = null;
                    if (ObjectivePointRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new CollectionEditorDialog(this, "Players", Map.Players, () => new Player(Map)).ShowDialog();
            if (SelectedPlayer != null)
            {
                if (!Map.Players.Contains(SelectedPlayer))
                {
                    SelectedPlayer = null;
                    if (PlayerRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnPlayerAlliancesEdit()
        {
            if (Map.Players.Count > 1)
                new PlayerAllianceDialog(this, "Player alliances", Map).ShowDialog();
            else
                MessageBox.Show("You need at least 2 players to create alliances.");
        }

        [RelayCommand]
        private void OnSpeechEventsEdit()
        {
            new CollectionEditorDialog(this, "Speech events", Map.SpeechEvents, () => new SpeechEvent(Map, NamedElement.GenerateName("SpeechEvent", Map.SpeechEvents))).ShowDialog();
        }

        [RelayCommand]
        private void OnTeamsEdit()
        {
            new TeamsDialog(this, "Teams", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnTimersEdit()
        {
            new TimerDialog(this, "Timers", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWaypointPathsEdit()
        {
            new CollectionEditorDialog(this, "Waypoint paths", Map.WaypointPaths, () =>
            {
                var wp = new WaypointPath(Map, NamedElement.GenerateName("WaypointPath", Map.WaypointPaths));
                wp.Points.Add(new(wp, 0, 0, 0));
                return wp;
            }).ShowDialog();
            if (SelectedWaypointPath != null)
            {
                if (!Map.WaypointPaths.Contains(SelectedWaypointPath))
                {
                    SelectedWaypointPathPoint = null;
                    SelectedWaypointPath = null;
                    if (WaypointPathRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if (SelectedWaypointPathPoint != null && !SelectedWaypointPath.Points.Contains(SelectedWaypointPathPoint))
                {
                    SelectedWaypointPathPoint = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldCrewsAndArmsEdit()
        {
            new WorldCrewAndArmsDialog(this, "World crews and arms", Map).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new CollectionEditorDialog(this, "World objects", Map.WorldObjects, () => new WorldObject(Map)).ShowDialog();
            if (SelectedWorldObject != null)
            {
                if (!Map.WorldObjects.Contains(SelectedWorldObject))
                {
                    SelectedWorldObject = null;
                    if (WorldObjectRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldPointSetsEdit()
        {
            new CollectionEditorDialog(this, "World point sets", Map.WorldPointSets, () =>
            {
                var wps = new WorldPointSet(Map, NamedElement.GenerateName("WorldPointSet", Map.WorldPointSets));
                wps.Points.Add(new(wps, 0, 0, 0, 0));
                return wps;
            }).ShowDialog();
            if (SelectedWorldPointSet != null)
            {
                if (!Map.WorldPointSets.Contains(SelectedWorldPointSet))
                {
                    SelectedWorldPoint = null;
                    SelectedWorldPointSet = null;
                    if (WorldPointSetRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if(SelectedWorldPoint!=null && !SelectedWorldPointSet.Points.Contains(SelectedWorldPoint))
                {
                    SelectedWorldPoint = null;
                    if (WorldPointSetRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldPolygonsEdit()
        {
            new CollectionEditorDialog(this, "World polygons", Map.WorldPolygons, () =>
            {
                var wp = new WorldPolygon(Map, NamedElement.GenerateName("WorldPolygon", Map.WorldPolygons));
                wp.Points.Add(new(wp, 0, 0));
                return wp;
            }).ShowDialog();
            if (SelectedWorldPolygon != null)
            {
                if (!Map.WorldPolygons.Contains(SelectedWorldPolygon))
                {
                    SelectedWorldPolygonPoint = null;
                    SelectedWorldPolygon = null;
                    if (WorldPolygonRadioButton.IsChecked == true)
                        SelectedElement = null;
                }
                else if (SelectedWorldPolygonPoint != null && !SelectedWorldPolygon.Points.Contains(SelectedWorldPolygonPoint))
                {
                    SelectedWorldPolygonPoint = null;
                }
            }
        }

        [RelayCommand]
        private void OnWorldRulesEdit()
        {
            new CollectionEditorDialog(this, "World rules", Map.WorldRules, () => new WorldRule(Map, NamedElement.GenerateName("WorldRule", Map.WorldRules))).ShowDialog();
        }

        [RelayCommand]
        private void OnAppSettingsEdit()
        {
            var asd = new AppSettingsDialog(this, "Settings", settings);
            asd.ShowDialog();
        }

        [RelayCommand]
        private void OnAboutAppShow()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"TPMapEditor version {v.Major}.{v.Minor}.{v.Build}\nAuthor : Randhomme", "About", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ClearSelections()
        {
            SelectedWorldObjects.Clear();
            SelectedPlayers.Clear();
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPaths.Clear();
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygons.Clear();
            SelectedWorldPoints.Clear();
            SelectedWorldPointSets.Clear();
            SelectedObjectivePoints.Clear();
            SelectedMapTextPoints.Clear();
            SelectedWorldObject = null;
            SelectedPlayer = null;
            SelectedWaypointPathPoint = null;
            SelectedWaypointPath = null;
            SelectedWorldPolygonPoint = null;
            SelectedWorldPolygon = null;
            SelectedWorldPoint = null;
            SelectedWorldPointSet = null;
            SelectedObjectivePoint = null;
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void IncreaseZIndexButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetZIndex(SelectedElement, Panel.GetZIndex(SelectedElement) + 1);
            //if (SelectedElement != null)
            //    SelectedElement.Opacity += 0.1;
        }

        private void DecreaseZIndexButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetZIndex(SelectedElement, Panel.GetZIndex(SelectedElement) - 1);
            //if (SelectedElement != null)
            //    SelectedElement.Opacity -= 0.1;
        }

        //Disable default Alt behaviour to allow rotation
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                (RotateCheckBox.IsChecked == true ||
                (WorldObjectPreviewControl.Visibility == Visibility.Visible) ||
                (PlayerPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointSetPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointPreviewControl.Visibility == Visibility.Visible)))
                e.Handled = true;
        }

        //Scroll horizontally by pressing Shift and using MouseWheel
        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                MapScrollViewer.ScrollToHorizontalOffset(MapScrollViewer.HorizontalOffset + e.Delta);
                e.Handled = true;
            }
        }

        private int GetAcceleratedRotation()
        {
            DateTime now = DateTime.Now;
            double ms = (now - lastWheelTime).TotalMilliseconds;
            lastWheelTime = now;

            if (ms < 25) return 10;
            if (ms < 50) return 5;
            if (ms < 100) return 2;

            return 1;
        }

        //Returns a rotation between -180 and 180
        private double GetRotation(double rotation)
        {
            if (rotation > 180) rotation -= 360;
            else if (rotation < -180) rotation += 360;
            return rotation;
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
            WorldObjectGridRow.Height = new GridLength(1, GridUnitType.Star);
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
            RotateCheckBox.Checked += RotateWorldObjectCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldObjectCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldObject();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldObject_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click += DeleteWorldObjectButton_Click;
            SelectedElement = (UIElement)WorldObjectItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldObject);
        }

        private void HideWotElements()
        {
            WotDataGrid.SelectedItem = null;
            WorldObjectGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldObjectItemsControl, 0);
            WorldObjectItemsControl.Opacity = 0.5;
            WorldObjectItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].IsSelected = false;
            MoveCheckBox.Checked -= MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldObject();
            RotateCheckBox.Checked -= RotateWorldObjectCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotateWorldObjectCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotateWorldObject();
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
            Map.WorldObjects.Add(new(Map, SelectedWorldObjectType!, Canvas.GetLeft(WorldObjectPreviewControl) + WorldObjectPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldObjectPreviewControl) - WorldObjectPreviewControl.ActualHeight / 2, WotSliderRotate.Value));
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldObjectPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WotDataGrid.SelectedItem = null;
        }

        private void WorldObjectPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = WotSliderRotate.Value + (e.Delta > 0 ? step : -step);
                WotSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
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

                if (MoveCheckBox.IsChecked == false)
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
            SelectedElement = (UIElement)WorldObjectItemsControl.ItemContainerGenerator.ContainerFromItem(worldObject);
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
            SelectedElement = null;
        }

        private void ClearWorldObjectSelection()
        {
            foreach (var v in SelectedWorldObjects)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
            SelectedElement = null;
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

        private void RotateWorldObjectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotateWorldObject();
        }

        private void RotateWorldObjectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotateWorldObject();
        }

        private void EnableRotateWorldObject()
        {
            MapGridOutside.MouseWheel += MapGridOutsideWorldObject_MouseWheel;
        }

        private void DisableRotateWorldObject()
        {
            MapGridOutside.MouseWheel -= MapGridOutsideWorldObject_MouseWheel;
        }

        private void MapGridOutsideWorldObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedWorldObjects.Count; i++)
                {
                    var worldObject = SelectedWorldObjects[i];
                    var newRotation = worldObject.ZRotation + (e.Delta > 0 ? step : -step);
                    worldObject.ZRotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        private void DeleteWorldObjectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var worldObject in SelectedWorldObjects)
            {
                Map.WorldObjects.Remove(worldObject);
            }
            SelectedWorldObjects.Clear();
            SelectedWorldObject = null;
            SelectedElement = null;
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
            MapGridOutside.MouseLeftButtonDown += MapGridOutsidePlayer_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click += DeletePlayerPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMovePlayer();
            RotateCheckBox.Checked += RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked += RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotatePlayer();
            MoveCheckBox.Checked += MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked += MovePlayerCheckBox_Unchecked;
            SelectedElement = (UIElement)PlayerItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedPlayer);
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
            RotateCheckBox.Checked -= RotatePlayerCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotatePlayerCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotatePlayer();
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

        private void MapGridOutsidePlayerPreview_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(PlayerPreviewControl, mousePos.X - PlayerPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PlayerPreviewControl, mousePos.Y - PlayerPreviewControl.ActualHeight / 2);
        }

        private void PlayerPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Map.Players.Add(new(Map, NamedElement.GenerateName("Player", Map.Players), Canvas.GetLeft(PlayerPreviewControl) + PlayerPreviewControl.ActualWidth / 2, -Canvas.GetTop(PlayerPreviewControl) - PlayerPreviewControl.ActualHeight / 2, 0, PlayerSliderRotate.Value, Colors.Red));
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void PlayerPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPlayerCheckBox.IsChecked = false;
        }

        private void PlayerPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = PlayerSliderRotate.Value + (e.Delta > 0 ? step : -step);
                PlayerSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
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
            SelectedElement = (UIElement)PlayerItemsControl.ItemContainerGenerator.ContainerFromItem(player);
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
            SelectedElement = null;
        }

        private void ClearPlayerSelection()
        {
            foreach (var v in SelectedPlayers)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
            SelectedElement = null;
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

        private void RotatePlayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotatePlayer();
        }

        private void RotatePlayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotatePlayer();
        }

        private void EnableRotatePlayer()
        {
            MapGridOutside.MouseWheel += MapGridOutsidePlayer_MouseWheel;
        }

        private void DisableRotatePlayer()
        {
            MapGridOutside.MouseWheel -= MapGridOutsidePlayer_MouseWheel;
        }

        private void MapGridOutsidePlayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedPlayers.Count; i++)
                {
                    var player = SelectedPlayers[i];
                    var newRotation = player.Rotation + (e.Delta > 0 ? step : -step);
                    player.Rotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        private void DeletePlayerPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var worldObject in SelectedPlayers)
            {
                Map.Players.Remove(worldObject);
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
            SelectedElement = null;
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
            WaypointPathGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WaypointPathItemsControl, 1);
            WaypointPathItemsControl.Opacity = 1;
            WaypointPathItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWaypointPathPointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWaypointPathPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click += DeleteWaypointPathPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWaypointPathPoint();
            SelectedElement = (UIElement)WaypointPathItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWaypointPath);
        }

        private void HidePathElements()
        {
            WaypointPathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WaypointPathItemsControl, 0);
            WaypointPathItemsControl.Opacity = 0.5;
            WaypointPathItemsControl.IsEnabled = false;
            NewWaypointPathRadioButton.IsChecked = AddWaypointPathPointRadioButton.IsChecked = false;
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

        private void OnWaypointPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPath clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWaypointPath(clickedObject);
                }
                else
                {
                    ClearWaypointPathSelection();
                    SelectAndMakeWaypointPathLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WaypointPathPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWaypointPathPoint(clickedObject);
                }
                else
                {
                    ClearWaypointPathSelection();
                    SelectAndMakeWaypointPathPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWaypointPathPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWaypointPathSelection();
        }

        private void CtrlSelectWaypointPath(WaypointPath waypointPath)
        {
            if (waypointPath.IsLastSelected)
            {
                RemoveWaypointPathFromSelection(waypointPath);
            }
            else
            {
                SelectAndMakeWaypointPathLastSelected(waypointPath);
            }
        }

        private void CtrlSelectWaypointPathPoint(WaypointPathPoint waypointPathPoint)
        {
            if (waypointPathPoint.IsLastSelected)
            {
                RemoveWaypointPathPointFromSelection(waypointPathPoint);
            }
            else
            {
                SelectAndMakeWaypointPathPointLastSelected(waypointPathPoint);
            }
        }

        private void SelectWaypointPath(WaypointPath waypointPath)
        {
            SelectWaypointPathWithoutPoints(waypointPath);
            foreach (var p in waypointPath.Points)
            {
                SelectWaypointPathPoint(p);
                if (p.IsLastSelected)
                {
                    SelectedWaypointPathPoint = null;
                    p.IsLastSelected = false;
                }
            }
        }

        private void SelectWaypointPathWithoutPoints(WaypointPath waypointPath)
        {
            if (!waypointPath.IsSelected)
                AddWaypointPathToSelection(waypointPath);
        }

        private void SelectAndMakeWaypointPathLastSelected(WaypointPath waypointPath)
        {
            SelectWaypointPath(waypointPath);
            MakeWaypointPathLastSelected(waypointPath);
        }

        private void SelectAndMakeWaypointPathLastSelectedWithoutPoints(WaypointPath waypointPath)
        {
            SelectWaypointPathWithoutPoints(waypointPath);
            MakeWaypointPathLastSelected(waypointPath);
        }

        private void SelectWaypointPathPoint(WaypointPathPoint waypointPathPoint)
        {
            if (!waypointPathPoint.IsSelected)
                AddWaypointPathPointToSelection(waypointPathPoint);
            SelectAndMakeWaypointPathLastSelectedWithoutPoints(waypointPathPoint.Parent);
        }

        private void SelectAndMakeWaypointPathPointLastSelected(WaypointPathPoint waypointPathPoint)
        {
            SelectWaypointPathPoint(waypointPathPoint);
            MakeWaypointPathPointLastSelected(waypointPathPoint);
        }

        private void AddWaypointPathToSelection(WaypointPath waypointPath)
        {
            waypointPath.IsSelected = true;
            SelectedWaypointPaths.Add(waypointPath);
        }

        private void AddWaypointPathPointToSelection(WaypointPathPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = true;
            SelectedWaypointPathPoints.Add(waypointPathPoint);
        }

        private void MakeWaypointPathLastSelected(WaypointPath waypointPath)
        {
            if (SelectedWaypointPath != null)
            {
                SelectedWaypointPath.IsLastSelected = false;
            }
            waypointPath.IsLastSelected = true;
            SelectedWaypointPath = waypointPath;
            SelectedElement = (UIElement)WaypointPathItemsControl.ItemContainerGenerator.ContainerFromItem(waypointPath);
        }

        private void MakeWaypointPathPointLastSelected(WaypointPathPoint waypointPathPoint)
        {
            if (SelectedWaypointPathPoint != null)
            {
                SelectedWaypointPathPoint.IsLastSelected = false;
            }
            waypointPathPoint.IsLastSelected = true;
            SelectedWaypointPathPoint = waypointPathPoint;
        }

        private void RemoveWaypointPathFromSelection(WaypointPath waypointPath)
        {
            foreach (var point in waypointPath.Points)
            {
                RemoveWaypointPathPointFromSelection(point);
            }
            waypointPath.IsSelected = waypointPath.IsLastSelected = false;
            SelectedWaypointPaths.Remove(waypointPath);
            SelectedWaypointPath = null;
            SelectedElement = null;
        }

        private void RemoveWaypointPathPointFromSelection(WaypointPathPoint waypointPathPoint)
        {
            waypointPathPoint.IsSelected = false;
            SelectedWaypointPathPoints.Remove(waypointPathPoint);
            if (waypointPathPoint.IsLastSelected)
            {
                waypointPathPoint.IsLastSelected = false;
                SelectedWaypointPathPoint = null;
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
            SelectedElement = null;
            ClearWaypointPathPointSelection();
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
            foreach (var p in SelectedWaypointPathPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if(p.Parent.Points.Count == 0)
                {
                    Map.WaypointPaths.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWaypointPath = null;
                        SelectedElement = null;
                    }
                }
            }
            SelectedWaypointPathPoints.Clear();
            SelectedWaypointPathPoint = null;
        }

        private void MapGridOutsideWaypointPathPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WaypointPathPreviewControl, mousePos.X - WaypointPathPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WaypointPathPreviewControl, mousePos.Y - WaypointPathPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WaypointPathPointPreviewControl, mousePos.X - WaypointPathPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WaypointPathPointPreviewControl, mousePos.Y - WaypointPathPointPreviewControl.ActualHeight / 2);
        }

        private void WaypointPathPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = new WaypointPath(Map, NamedElement.GenerateName("WaypointPath", Map.WaypointPaths));
            var point = new WaypointPathPoint(path, Canvas.GetLeft(WaypointPathPreviewControl) + WaypointPathPreviewControl.ActualWidth / 2, -Canvas.GetTop(WaypointPathPreviewControl) - WaypointPathPreviewControl.ActualHeight / 2, 0);
            path.Points.Add(point);
            Map.WaypointPaths.Add(path);
            ClearWaypointPathSelection();
            SelectAndMakeWaypointPathPointLastSelected(point);
            AddWaypointPathPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NewWaypointPathRadioButton.IsChecked = false;
        }

        private void WaypointPathPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWaypointPath != null)
            {
                var point = new WaypointPathPoint(SelectedWaypointPath, Canvas.GetLeft(WaypointPathPointPreviewControl) + WaypointPathPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WaypointPathPointPreviewControl) - WaypointPathPointPreviewControl.ActualHeight / 2, 0);
                SelectAndMakeWaypointPathPointLastSelected(point);
                SelectedWaypointPath?.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWaypointPathPointRadioButton.IsChecked = false;
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
            WorldPolygonGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPolygonItemsControl, 1);
            WorldPolygonItemsControl.Opacity = 1;
            WorldPolygonItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPolygonPointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click += DeleteWorldPolygonPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPolygonPoint();
            SelectedElement = (UIElement)WorldPolygonItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPolygon);
        }

        private void HideWorldPolygonElements()
        {
            WorldPolygonGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPolygonItemsControl, 0);
            WorldPolygonItemsControl.Opacity = 0.5;
            WorldPolygonItemsControl.IsEnabled = false;
            NewWorldPolygonRadioButton.IsChecked = AddWorldPolygonPointRadioButton.IsChecked = false;
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

        private void OnWorldPolygonClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygon clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPolygon(clickedObject);
                }
                else
                {
                    ClearWorldPolygonSelection();
                    SelectAndMakeWorldPolygonLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWorldPolygonPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPolygonPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPolygonPoint(clickedObject);
                }
                else
                {
                    ClearWorldPolygonSelection();
                    SelectAndMakeWorldPolygonPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldPolygonPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldPolygonSelection();
        }

        private void CtrlSelectWorldPolygon(WorldPolygon worldPolygon)
        {
            if (worldPolygon.IsLastSelected)
            {
                RemoveWorldPolygonFromSelection(worldPolygon);
            }
            else
            {
                SelectAndMakeWorldPolygonLastSelected(worldPolygon);
            }
        }

        private void CtrlSelectWorldPolygonPoint(WorldPolygonPoint worldPolygonPoint)
        {
            if (worldPolygonPoint.IsLastSelected)
            {
                RemoveWorldPolygonPointFromSelection(worldPolygonPoint);
            }
            else
            {
                SelectAndMakeWorldPolygonPointLastSelected(worldPolygonPoint);
            }
        }

        private void SelectWorldPolygon(WorldPolygon worldPolygon)
        {
            SelectWorldPolygonWithoutPoints(worldPolygon);
            foreach (var p in worldPolygon.Points)
            {
                SelectWorldPolygonPoint(p);
                if (p.IsLastSelected)
                {
                    SelectedWorldPolygonPoint = null;
                    p.IsLastSelected = false;
                }
            }
        }

        private void SelectWorldPolygonWithoutPoints(WorldPolygon worldPolygon)
        {
            if (!worldPolygon.IsSelected)
                AddWorldPolygonToSelection(worldPolygon);
        }

        private void SelectAndMakeWorldPolygonLastSelected(WorldPolygon worldPolygon)
        {
            SelectWorldPolygon(worldPolygon);
            MakeWorldPolygonLastSelected(worldPolygon);
        }

        private void SelectAndMakeWorldPolygonLastSelectedWithoutPoints(WorldPolygon worldPolygon)
        {
            SelectWorldPolygonWithoutPoints(worldPolygon);
            MakeWorldPolygonLastSelected(worldPolygon);
        }

        private void SelectWorldPolygonPoint(WorldPolygonPoint worldPolygonPoint)
        {
            if (!worldPolygonPoint.IsSelected)
                AddWorldPolygonPointToSelection(worldPolygonPoint);
            SelectAndMakeWorldPolygonLastSelectedWithoutPoints(worldPolygonPoint.Parent);
        }

        private void SelectAndMakeWorldPolygonPointLastSelected(WorldPolygonPoint worldPolygonPoint)
        {
            SelectWorldPolygonPoint(worldPolygonPoint);
            MakeWorldPolygonPointLastSelected(worldPolygonPoint);
        }

        private void AddWorldPolygonToSelection(WorldPolygon worldPolygon)
        {
            worldPolygon.IsSelected = true;
            SelectedWorldPolygons.Add(worldPolygon);
        }

        private void AddWorldPolygonPointToSelection(WorldPolygonPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = true;
            SelectedWorldPolygonPoints.Add(worldPolygonPoint);
        }

        private void MakeWorldPolygonLastSelected(WorldPolygon worldPolygon)
        {
            if (SelectedWorldPolygon != null)
            {
                SelectedWorldPolygon.IsLastSelected = false;
            }
            worldPolygon.IsLastSelected = true;
            SelectedWorldPolygon = worldPolygon;
            SelectedElement = (UIElement)WorldPolygonItemsControl.ItemContainerGenerator.ContainerFromItem(worldPolygon);
        }

        private void MakeWorldPolygonPointLastSelected(WorldPolygonPoint worldPolygonPoint)
        {
            if (SelectedWorldPolygonPoint != null)
            {
                SelectedWorldPolygonPoint.IsLastSelected = false;
            }
            worldPolygonPoint.IsLastSelected = true;
            SelectedWorldPolygonPoint = worldPolygonPoint;
        }

        private void RemoveWorldPolygonFromSelection(WorldPolygon worldPolygon)
        {
            foreach(var p in worldPolygon.Points)
            {
                RemoveWorldPolygonPointFromSelection(p);
            }
            worldPolygon.IsSelected = worldPolygon.IsLastSelected = false;
            SelectedWorldPolygons.Remove(worldPolygon);
            SelectedWorldPolygon = null;
            SelectedElement = null;
        }

        private void RemoveWorldPolygonPointFromSelection(WorldPolygonPoint worldPolygonPoint)
        {
            worldPolygonPoint.IsSelected = false;
            SelectedWorldPolygonPoints.Remove(worldPolygonPoint);
            if (worldPolygonPoint.IsLastSelected)
            {
                worldPolygonPoint.IsLastSelected = false;
                SelectedWorldPolygonPoint = null;
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
            SelectedElement = null;
            ClearWorldPolygonPointSelection();
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
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWorldPolygon = null;
                        SelectedElement = null;
                    }
                }
            }
            SelectedWorldPolygonPoints.Clear();
            SelectedWorldPolygonPoint = null;
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
            var path = new WorldPolygon(Map, NamedElement.GenerateName("WorldPolygon", Map.WorldPolygons));
            var point = new WorldPolygonPoint(path, Canvas.GetLeft(WorldPolygonPreviewControl) + WorldPolygonPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPreviewControl) - WorldPolygonPreviewControl.ActualHeight / 2);
            path.Points.Add(point);
            Map.WorldPolygons.Add(path);
            ClearWorldPolygonSelection();
            SelectAndMakeWorldPolygonPointLastSelected(point);
            AddWorldPolygonPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewWorldPolygonRadioButton.IsChecked = false;
        }

        private void WorldPolygonPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWorldPolygon != null)
            {
                var point = new WorldPolygonPoint(SelectedWorldPolygon, Canvas.GetLeft(WorldPolygonPointPreviewControl) + WorldPolygonPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPolygonPointPreviewControl) - WorldPolygonPointPreviewControl.ActualHeight / 2);
                SelectAndMakeWorldPolygonPointLastSelected(point);
                SelectedWorldPolygon?.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPolygonPointRadioButton.IsChecked = false;
        }

        #endregion

        #region WorldPointSet

        private void PointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPointSetElements();
        }

        private void PointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPointSetElements();
        }

        private void ShowWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPointSetItemsControl, 1);
            WorldPointSetItemsControl.Opacity = 1;
            WorldPointSetItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPointCheckBox_Unchecked;
            DeleteButton.Click += DeleteWorldPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPoint();
            RotateCheckBox.Checked += RotateWorldPointCheckBox_Checked;
            RotateCheckBox.Unchecked += RotateWorldPointCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                EnableRotateWorldPoint();
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideWorldPointPreview_MouseMove;
            SelectedElement = (UIElement)WorldPointSetItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedWorldPointSet);
        }

        private void HideWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPointSetItemsControl, 0);
            WorldPointSetItemsControl.Opacity = 0.5;
            WorldPointSetItemsControl.IsEnabled = false;
            NewWorldPointSetRadioButton.IsChecked = AddWorldPointSetPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPoint();
            RotateCheckBox.Checked -= RotateWorldPointCheckBox_Checked;
            RotateCheckBox.Unchecked -= RotateWorldPointCheckBox_Unchecked;
            if (RotateCheckBox.IsChecked == true)
                DisableRotateWorldPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPointButton_Click;
        }

        private void OnWorldPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is WorldPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectWorldPoint(clickedObject);
                }
                else
                {
                    ClearWorldPointSetSelection();
                    SelectAndMakeWorldPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearWorldPointSetSelection();
        }

        private void CtrlSelectWorldPoint(WorldPoint worldPoint)
        {
            if (worldPoint.IsLastSelected)
            {
                RemoveWorldPointFromSelection(worldPoint);
            }
            else
            {
                SelectAndMakeWorldPointLastSelected(worldPoint);
            }
        }

        private void SelectWorldPointSetWithoutPoints(WorldPointSet worldPointSet)
        {
            if (!worldPointSet.IsSelected)
                AddWorldPointSetToSelection(worldPointSet);
        }

        private void SelectAndMakeWorldPointSetLastSelectedWithoutPoints(WorldPointSet worldPointSet)
        {
            SelectWorldPointSetWithoutPoints(worldPointSet);
            MakeWorldPointSetLastSelected(worldPointSet);
        }

        private void SelectWorldPoint(WorldPoint worldPoint)
        {
            if (!worldPoint.IsSelected)
                AddWorldPointToSelection(worldPoint);
            SelectAndMakeWorldPointSetLastSelectedWithoutPoints(worldPoint.Parent);
        }

        private void SelectAndMakeWorldPointLastSelected(WorldPoint worldPoint)
        {
            SelectWorldPoint(worldPoint);
            MakeWorldPointLastSelected(worldPoint);
        }

        private void AddWorldPointSetToSelection(WorldPointSet worldPointSet)
        {
            worldPointSet.IsSelected = true;
            SelectedWorldPointSets.Add(worldPointSet);
        }

        private void AddWorldPointToSelection(WorldPoint worldPoint)
        {
            worldPoint.IsSelected = true;
            SelectedWorldPoints.Add(worldPoint);
        }

        private void MakeWorldPointSetLastSelected(WorldPointSet worldPointSet)
        {
            if (SelectedWorldPointSet != null)
            {
                SelectedWorldPointSet.IsLastSelected = false;
            }
            worldPointSet.IsLastSelected = true;
            SelectedWorldPointSet = worldPointSet;
        }

        private void MakeWorldPointLastSelected(WorldPoint worldPoint)
        {
            if (SelectedWorldPoint != null)
            {
                SelectedWorldPoint.IsLastSelected = false;
            }
            worldPoint.IsLastSelected = true;
            SelectedWorldPoint = worldPoint;
            SelectedElement = (UIElement)WorldPointSetItemsControl.ItemContainerGenerator.ContainerFromItem(worldPoint);
        }

        private void RemoveWorldPointFromSelection(WorldPoint worldPoint)
        {
            worldPoint.IsSelected = false;
            SelectedWorldPoints.Remove(worldPoint);
            if (worldPoint.IsLastSelected)
            {
                worldPoint.IsLastSelected = false;
                SelectedWorldPoint = null;
            }
        }

        private void ClearWorldPointSetSelection()
        {
            foreach (var v in SelectedWorldPointSets)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPointSets.Clear();
            SelectedWorldPointSet = null;
            SelectedElement = null;
            ClearWorldPointSelection();
        }

        private void ClearWorldPointSelection()
        {
            foreach (var v in SelectedWorldPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedWorldPoints.Clear();
            SelectedWorldPoint = null;
        }

        private void MoveWorldPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveWorldPoint();
        }

        private void MoveWorldPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveWorldPoint();
        }

        private void EnableMoveWorldPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveWorldPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWorldPoints.Count; i++)
            {
                var selectedObject = SelectedWorldPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteWorldPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedWorldPoints)
            {
                p.Parent.Points.Remove(p);
                //remove path if no more points
                if (p.Parent.Points.Count == 0)
                {
                    Map.WorldPointSets.Remove(p.Parent);
                    if (p.Parent.IsLastSelected)
                    {
                        SelectedWorldPointSet = null;
                    }
                }
            }
            SelectedWorldPoints.Clear();
            SelectedWorldPoint = null;
            SelectedElement = null;
        }

        private void MapGridOutsideWorldPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldPointSetPreviewControl, mousePos.X - WorldPointSetPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPointSetPreviewControl, mousePos.Y - WorldPointSetPreviewControl.ActualHeight / 2);
            Canvas.SetLeft(WorldPointPreviewControl, mousePos.X - WorldPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldPointPreviewControl, mousePos.Y - WorldPointPreviewControl.ActualHeight / 2);
        }

        private void WorldPointSetPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = new WorldPointSet(Map, NamedElement.GenerateName("WorldPointSet", Map.WorldPointSets));
            var point = new WorldPoint(path, Canvas.GetLeft(WorldPointSetPreviewControl) + WorldPointSetPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointSetPreviewControl) - WorldPointSetPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
            path.Points.Add(point);
            Map.WorldPointSets.Add(path);
            ClearWorldPointSetSelection();
            SelectAndMakeWorldPointLastSelected(point);
            AddWorldPointSetPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointSetPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewWorldPointSetRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedWorldPointSet != null)
            {
                var point = new WorldPoint(SelectedWorldPointSet, Canvas.GetLeft(WorldPointPreviewControl) + WorldPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(WorldPointPreviewControl) - WorldPointPreviewControl.ActualHeight / 2, 0, WorldPointSliderRotate.Value);
                SelectAndMakeWorldPointLastSelected(point);
                SelectedWorldPointSet?.Points.Add(point);
            }
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPointSetPointRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                var newValue = WorldPointSliderRotate.Value + (e.Delta > 0 ? step : -step);
                WorldPointSliderRotate.Value = GetRotation(newValue);
                e.Handled = true;
            }
        }

        private void RotateWorldPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableRotateWorldPoint();
        }

        private void RotateWorldPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableRotateWorldPoint();
        }

        private void EnableRotateWorldPoint()
        {
            MapGridOutside.MouseWheel += MapGridOutsideWorldPoint_MouseWheel;
        }

        private void DisableRotateWorldPoint()
        {
            MapGridOutside.MouseWheel -= MapGridOutsideWorldPoint_MouseWheel;
        }

        private void MapGridOutsideWorldPoint_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                for (int i = 0; i < SelectedWorldPoints.Count; i++)
                {
                    var worldPoint = SelectedWorldPoints[i];
                    var newRotation = worldPoint.ZRotation + (e.Delta > 0 ? step : -step);
                    worldPoint.ZRotation = GetRotation(newRotation);
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region ObjectivePoint

        private void ObjectivePointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowObjectivePointElements();
        }

        private void ObjectivePointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideObjectivePointElements();
        }

        private void ShowObjectivePointElements()
        {
            ObjectivePointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(ObjectivePointItemsControl, 1);
            ObjectivePointItemsControl.Opacity = 1;
            ObjectivePointItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveObjectivePointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click += DeleteObjectivePointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveObjectivePoint();
            SelectedElement = (UIElement)ObjectivePointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedObjectivePoint);
        }

        private void HideObjectivePointElements()
        {
            ObjectivePointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(ObjectivePointItemsControl, 0);
            ObjectivePointItemsControl.Opacity = 0.5;
            ObjectivePointItemsControl.IsEnabled = false;
            AddObjectivePointCheckBox.IsChecked = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            MoveCheckBox.Checked -= MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveObjectivePoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click -= DeleteObjectivePointButton_Click;
        }

        private void OnObjectivePointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is ObjectivePoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectObjectivePoint(clickedObject);
                }
                else
                {
                    ClearObjectivePointSelection();
                    SelectAndMakeObjectivePointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearObjectivePointSelection();
        }

        private void CtrlSelectObjectivePoint(ObjectivePoint objectivePoint)
        {
            if (objectivePoint.IsLastSelected)
            {
                RemoveObjectivePointFromSelection(objectivePoint);
            }
            else
            {
                SelectAndMakeObjectivePointLastSelected(objectivePoint);
            }
        }

        private void SelectObjectivePoint(ObjectivePoint objectivePoint)
        {
            if (!objectivePoint.IsSelected)
                AddObjectivePointToSelection(objectivePoint);
        }

        private void SelectAndMakeObjectivePointLastSelected(ObjectivePoint objectivePoint)
        {
            SelectObjectivePoint(objectivePoint);
            MakeObjectivePointLastSelected(objectivePoint);
        }

        private void AddObjectivePointToSelection(ObjectivePoint objectivePoint)
        {
            objectivePoint.IsSelected = true;
            SelectedObjectivePoints.Add(objectivePoint);
        }

        private void MakeObjectivePointLastSelected(ObjectivePoint objectivePoint)
        {
            if (SelectedObjectivePoint != null)
            {
                SelectedObjectivePoint.IsLastSelected = false;
            }
            objectivePoint.IsLastSelected = true;
            SelectedObjectivePoint = objectivePoint;
            SelectedElement = (UIElement)ObjectivePointItemsControl.ItemContainerGenerator.ContainerFromItem(objectivePoint);
        }

        private void RemoveObjectivePointFromSelection(ObjectivePoint objectivePoint)
        {
            objectivePoint.IsSelected = false;
            SelectedObjectivePoints.Remove(objectivePoint);
            if (objectivePoint.IsLastSelected)
            {
                objectivePoint.IsLastSelected = false;
                SelectedObjectivePoint = null;
            }
        }

        private void ClearObjectivePointSelection()
        {
            foreach (var v in SelectedObjectivePoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedObjectivePoints.Clear();
            SelectedObjectivePoint = null;
            SelectedElement = null;
        }

        private void MoveObjectivePointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveObjectivePoint();
        }

        private void MoveObjectivePointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveObjectivePoint();
        }

        private void EnableMoveObjectivePoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveObjectivePoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveObjectivePoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveObjectivePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveObjectivePoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveObjectivePoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedObjectivePoints.Count; i++)
            {
                var selectedObject = SelectedObjectivePoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteObjectivePointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedObjectivePoints)
            {
                Map.ObjectivePoints.Remove(p);
            }
            SelectedObjectivePoints.Clear();
            SelectedObjectivePoint = null;
            SelectedElement = null;
        }

        private void MapGridOutsideObjectivePointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(ObjectivePointPreviewControl, mousePos.X - ObjectivePointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(ObjectivePointPreviewControl, mousePos.Y - ObjectivePointPreviewControl.ActualHeight / 2);
        }

        private void ObjectivePointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = new ObjectivePoint(Map, NamedElement.GenerateName("ObjectivePoint", Map.ObjectivePoints), Canvas.GetLeft(ObjectivePointPreviewControl) + ObjectivePointPreviewControl.ActualWidth / 2, -Canvas.GetTop(ObjectivePointPreviewControl) - ObjectivePointPreviewControl.ActualHeight / 2);
            SelectAndMakeObjectivePointLastSelected(point);
            Map.ObjectivePoints.Add(point);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void ObjectivePointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddObjectivePointCheckBox.IsChecked = false;
        }

        #endregion

        #region MapTextPoint

        private void MapTextPointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowMapTextPointElements();
        }

        private void MapTextPointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideMapTextPointElements();
        }

        private void ShowMapTextPointElements()
        {
            MapTextPointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(MapTextPointItemsControl, 1);
            MapTextPointItemsControl.Opacity = 1;
            MapTextPointItemsControl.IsEnabled = true;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            MoveCheckBox.Checked += MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveMapTextPointCheckBox_Unchecked;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove += MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click += DeleteMapTextPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveMapTextPoint();
            SelectedElement = (UIElement)MapTextPointItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedMapTextPoint);
        }

        private void HideMapTextPointElements()
        {
            MapTextPointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(MapTextPointItemsControl, 0);
            MapTextPointItemsControl.Opacity = 0.5;
            MapTextPointItemsControl.IsEnabled = false;
            AddMapTextPointCheckBox.IsChecked = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            MoveCheckBox.Checked -= MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveMapTextPoint();
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseMove -= MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click -= DeleteMapTextPointButton_Click;
        }

        private void OnMapTextPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element && element.DataContext is MapTextPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (ctrlPressed)
                {
                    CtrlSelectMapTextPoint(clickedObject);
                }
                else
                {
                    ClearMapTextPointSelection();
                    SelectAndMakeMapTextPointLastSelected(clickedObject);
                }

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void MapGridOutsideMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMapTextPointSelection();
        }

        private void CtrlSelectMapTextPoint(MapTextPoint mapTextPoint)
        {
            if (mapTextPoint.IsLastSelected)
            {
                RemoveMapTextPointFromSelection(mapTextPoint);
            }
            else
            {
                SelectAndMakeMapTextPointLastSelected(mapTextPoint);
            }
        }

        private void SelectMapTextPoint(MapTextPoint mapTextPoint)
        {
            if (!mapTextPoint.IsSelected)
                AddMapTextPointToSelection(mapTextPoint);
        }

        private void SelectAndMakeMapTextPointLastSelected(MapTextPoint mapTextPoint)
        {
            SelectMapTextPoint(mapTextPoint);
            MakeMapTextPointLastSelected(mapTextPoint);
        }

        private void AddMapTextPointToSelection(MapTextPoint mapTextPoint)
        {
            mapTextPoint.IsSelected = true;
            SelectedMapTextPoints.Add(mapTextPoint);
        }

        private void MakeMapTextPointLastSelected(MapTextPoint mapTextPoint)
        {
            if (SelectedMapTextPoint != null)
            {
                SelectedMapTextPoint.IsLastSelected = false;
            }
            mapTextPoint.IsLastSelected = true;
            SelectedMapTextPoint = mapTextPoint;
            SelectedElement = (UIElement)MapTextPointItemsControl.ItemContainerGenerator.ContainerFromItem(mapTextPoint);
        }

        private void RemoveMapTextPointFromSelection(MapTextPoint mapTextPoint)
        {
            mapTextPoint.IsSelected = false;
            SelectedMapTextPoints.Remove(mapTextPoint);
            if (mapTextPoint.IsLastSelected)
            {
                mapTextPoint.IsLastSelected = false;
                SelectedMapTextPoint = null;
            }
        }

        private void ClearMapTextPointSelection()
        {
            foreach (var v in SelectedMapTextPoints)
            {
                v.IsSelected = v.IsLastSelected = false;
            }
            SelectedMapTextPoints.Clear();
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void MoveMapTextPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableMoveMapTextPoint();
        }

        private void MoveMapTextPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableMoveMapTextPoint();
        }

        private void EnableMoveMapTextPoint()
        {
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp += MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveMapTextPoint()
        {
            MapGridOutside.MouseLeftButtonDown += MapGridOutsideMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonDown -= MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.MouseLeftButtonUp -= MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            moveActionPoint = e.GetPosition(MapGridInside);
            MapGridOutside.MouseMove += MapGridOutsideMoveMapTextPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideEndMoveMapTextPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            MapGridOutside.MouseMove -= MapGridOutsideMoveMapTextPoint_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMoveMapTextPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedMapTextPoints.Count; i++)
            {
                var selectedObject = SelectedMapTextPoints[i];
                selectedObject.X += x;
                selectedObject.Y -= y;
            }
            moveActionPoint = pos;
        }

        private void DeleteMapTextPointButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in SelectedMapTextPoints)
            {
                Map.MapTextPoints.Remove(p);
            }
            SelectedMapTextPoints.Clear();
            SelectedMapTextPoint = null;
            SelectedElement = null;
        }

        private void MapGridOutsideMapTextPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(MapTextPointPreviewControl, mousePos.X - MapTextPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(MapTextPointPreviewControl, mousePos.Y - MapTextPointPreviewControl.ActualHeight / 2);
        }

        private void MapTextPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var text = StringDictionnary.MapTextItems.Keys.ElementAt(MapTextPointPreviewTextComboBox.SelectedIndex);
            var point = new MapTextPoint(Map, NamedElement.GenerateName("MapTextPoint", Map.MapTextPoints), text, Canvas.GetLeft(MapTextPointPreviewControl) + MapTextPointPreviewControl.ActualWidth / 2, -Canvas.GetTop(MapTextPointPreviewControl) - MapTextPointPreviewControl.ActualHeight / 2);
            SelectAndMakeMapTextPointLastSelected(point);
            Map.MapTextPoints.Add(point);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void MapTextPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddMapTextPointCheckBox.IsChecked = false;
        }

        #endregion
    }
}
