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
        private WaypointPathPoint? selectedPathPoint;
        [ObservableProperty]
        private WaypointPath? selectedPath;
        [ObservableProperty]
        private WorldPolygonPoint? selectedPolygonPoint;
        [ObservableProperty]
        private WorldPolygon? selectedPolygon;
        [ObservableProperty]
        private WorldPoint? selectedWorldPoint;
        [ObservableProperty]
        private WorldPointSet? selectedPointSet;
        [ObservableProperty]
        private ObjectivePoint? selectedObjectivePoint;
        [ObservableProperty]
        private MapTextPoint? selectedMapTextPoint;

        public IList<WorldObject> SelectedWots { get; }
        public IList<Player> SelectedPlayers { get; }
        public IList<WaypointPathPoint> SelectedPathPoints { get; }
        public IList<WaypointPath> SelectedPaths { get; }
        public IList<WorldPolygonPoint> SelectedPolygonPoints { get; }
        public IList<WorldPolygon> SelectedPolygons { get; }
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
            SelectedWots = new List<WorldObject>();
            SelectedPlayers = new List<Player>();
            SelectedPathPoints = new List<WaypointPathPoint>();
            SelectedPaths = new List<WaypointPath>();
            SelectedPolygonPoints = new List<WorldPolygonPoint>();
            SelectedPolygons = new List<WorldPolygon>();
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
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new WorldObjectDialog(this, Map).ShowDialog();
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
            //    SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedWot != null)
            //    SelectedWot.BorderBrush = Brushes.Orange;
            //MoveCheckBox.Checked += RadioButton_Checked_5;
            //MoveCheckBox.Unchecked += RadioButton_Unchecked_5;
            //if (MoveCheckBox.IsChecked == true)
            //    MoveWotRadioButtonChecked();
            MapGrid.MouseMove += MapGridWorldObjectPreview_MouseMove;
            //DeleteButton.Click += Button_Click_1;
        }

        private void HideWotElements()
        {
            //wotPreview.Visibility = Visibility.Collapsed;
            WotGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldObjectItemsControl, 0);
            WorldObjectItemsControl.Opacity = 0.5;
            WorldObjectItemsControl.IsEnabled = false;
            //for (int i = 0; i < SelectedWots.Count; i++)
            //    SelectedWots[i].BorderBrush = Brushes.Transparent;
            //MoveCheckBox.Checked -= RadioButton_Checked_5;
            //MoveCheckBox.Unchecked -= RadioButton_Unchecked_5;
            //if (MoveCheckBox.IsChecked == true)
            //    MoveWotRadioButtonUnchecked();
            MapGrid.MouseMove -= MapGridWorldObjectPreview_MouseMove;
            //DeleteButton.Click -= Button_Click_1;
        }

        private void MapGridWorldObjectPreview_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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
            if (sender is FrameworkElement element && element.DataContext is WorldObject clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (!ctrlPressed)
                {
                    foreach (var p in Map.WorldObjects)
                        p.IsSelected = false;
                }

                clickedObject.IsSelected = !clickedObject.IsSelected;
                e.Handled = true;
            }
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
            //for (var i = 0; i < SelectedPlayers.Count; i++)
            //    SelectedPlayers[i].BorderBrush = Brushes.OrangeRed;
            //if (SelectedPlayer != null)
            //    SelectedPlayer.BorderBrush = Brushes.Orange;
            //MoveCheckBox.Checked += RadioButton_Checked_7;
            //MoveCheckBox.Unchecked += RadioButton_Unchecked_7;
            //if (MoveCheckBox.IsChecked == true)
            //    MovePlayerRadioButtonChecked();
            MapGrid.MouseMove += MapGridPlayerPreview_MouseMove;
            //DeleteButton.Click += Button_Click;
        }

        private void HidePlayerElements()
        {
            AddPlayerCheckBox.IsChecked = false;
            PlayerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(PlayerItemsControl, 0);
            PlayerItemsControl.Opacity = 0.5;
            PlayerItemsControl.IsEnabled = false;
            //for (var i = 0; i < SelectedPlayers.Count; i++)
            //    SelectedPlayers[i].BorderBrush = Brushes.Transparent;
            //MoveCheckBox.Checked -= RadioButton_Checked_7;
            //MoveCheckBox.Unchecked -= RadioButton_Unchecked_7;
            //if (MoveCheckBox.IsChecked == true)
            //    MovePlayerRadioButtonUnchecked();
            MapGrid.MouseMove -= MapGridPlayerPreview_MouseMove;
            //DeleteButton.Click -= Button_Click;
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Player clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (!ctrlPressed)
                {
                    foreach (var p in Map.Players)
                        p.IsSelected = false;
                }

                clickedObject.IsSelected = !clickedObject.IsSelected;
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

        private void MapGridPlayerPreview_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

        #endregion

        #region WaypointPath

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is WaypointPathPoint clickedObject)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (!ctrlPressed)
                {
                    foreach (var p in clickedObject.Parent.Points)
                        p.IsSelected = false;
                    SelectedPathPoint = clickedObject;
                }

                clickedObject.IsSelected = !clickedObject.IsSelected;
                e.Handled = true;
            }
        }

        private void DeleteWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPathPoint?.Parent.Points.Remove(SelectedPathPoint);
        }

        #endregion

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
            //MoveCheckBox.Checked += RadioButton_Checked_5;
            //MoveCheckBox.Unchecked += RadioButton_Unchecked_5;
            //if (MoveCheckBox.IsChecked == true)
            //    MoveWotRadioButtonChecked();
            //MapGrid.MouseMove += WotCanvas_MouseMove;
            //DeleteButton.Click += Button_Click_1;
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
            //MoveCheckBox.Checked -= RadioButton_Checked_5;
            //MoveCheckBox.Unchecked -= RadioButton_Unchecked_5;
            //if (MoveCheckBox.IsChecked == true)
            //    MoveWotRadioButtonUnchecked();
            //MapGrid.MouseMove -= WotCanvas_MouseMove;
            //DeleteButton.Click -= Button_Click_1;
        }

        private void PolygonRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PolygonRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

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
