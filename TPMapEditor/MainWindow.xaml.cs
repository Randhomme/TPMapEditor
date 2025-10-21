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
        private WotControl? selectedWot;
        [ObservableProperty]
        private PlayerControl? selectedPlayer;
        [ObservableProperty]
        private PathPointControl? selectedPathPoint;
        [ObservableProperty]
        private PathControl? selectedPath;
        [ObservableProperty]
        private PolygonPointControl? selectedPolygonPoint;
        [ObservableProperty]
        private PolygonControl? selectedPolygon;
        [ObservableProperty]
        private WorldPointControl? selectedWorldPoint;
        [ObservableProperty]
        private ObjectivePointControl? selectedObjectivePoint;
        public List<WotControl> SelectedWots { get; }
        public List<PlayerControl> SelectedPlayers { get; }
        public List<PathPointControl> SelectedPathPoints { get; }
        public List<PolygonPointControl> SelectedPolygonPoints { get; }
        public List<WorldPointControl> SelectedWorldPoints { get; }
        public List<ObjectivePointControl> SelectedObjectivePoints { get; }
        public WorldMap Map { get; }
        public WotGridItem? WotGridSelectedItem { get; set; }
        
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
            SelectedWots = new List<WotControl>();
            SelectedPlayers = new List<PlayerControl>();
            SelectedPathPoints = new List<PathPointControl>();
            SelectedPolygonPoints = new List<PolygonPointControl>();
            SelectedWorldPoints = new List<WorldPointControl>();
            SelectedObjectivePoints = new List<ObjectivePointControl>();
            InitializeComponent();
            wotPreview.Visibility = pathPointPreview.Visibility = polygonPointPreview.Visibility = Visibility.Hidden;
            wotRadioButton.IsChecked = true;
            HidePlayerElements();
            HidePathElements();
            HidePolygonElements();
            HidePointElements();
            this.selectedPlayerX.Minimum = this.selectedPlayerY.Minimum = -Map.Size / 2 - 150;
            this.selectedPlayerX.Maximum = this.selectedPlayerY.Maximum = Map.Size / 2 + 150;
        }

        #region MenuCommand

        [RelayCommand]
        private void OnWorldInfoEdit()
        {
            new WorldInfoDialog(this, Map).ShowDialog();
        }

        [RelayCommand]
        private void OnMapSizeEdit()
        {
            var msd = new MapSizeDialog(this, Map.Size);
            if (msd.ShowDialog() == true)
            {
                Map.Size = msd.Size;
                this.selectedPlayerX.Minimum = this.selectedPlayerY.Minimum = -Map.Size / 2 - 150;
                this.selectedPlayerX.Maximum = this.selectedPlayerY.Maximum = Map.Size / 2 + 150;
            }
        }

        [RelayCommand]
        private void OnPlayersEdit()
        {
            new PlayerDialog(this, Map, () => CreatePlayer(0,0,0)).ShowDialog();
        }

        [RelayCommand]
        private void OnWorldObjectsEdit()
        {
            new WorldObjectDialog(this, Map, () => CreateWorldObject(0, 0, 0)).ShowDialog();
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

        #endregion

        //wot radio button checked
        private void WotRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWotElements();
        }

        //wot radio button unchecked
        private void WotRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWotElements();
        }

        //player radio button checked
        private void PlayerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPlayerElements();
        }

        //player radio button unchecked
        private void PlayerRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePlayerElements();
        }

        //waypoint path radio button checked
        private void PathRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPathElements();
        }

        //waypoint path radio button unchecked
        private void PathRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePathElements();
        }

        //world polygon radio button checked
        private void PolygonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPolygonElements();
        }

        //world polygon radio button unchecked
        private void PolygonRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePolygonElements();
        }

        //world point radio button checked
        private void PointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowPointElements();
        }

        //world point radio button unchecked
        private void PointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HidePointElements();
        }

        //objective point radio button checked
        private void ObjectivePointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowObjectivePointElements();
        }

        //objective point radio button unchecked
        private void ObjectivePointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideObjectivePointElements();
        }

        private void HideWotElements()
        {
            wotPreview.Visibility = Visibility.Collapsed;
            wotDataGrid.SelectedItems.Clear();
            wotGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(wotCanvas, 0);
            wotCanvas.Opacity = 0.5;
            wotCanvas.IsEnabled = false;
            for (int i = 0; i < SelectedWots.Count; i++)
                SelectedWots[i].BorderBrush = Brushes.Transparent;
            //SelectedWot = null;
            moveCheckBox.Checked -= RadioButton_Checked_5;
            moveCheckBox.Unchecked -= RadioButton_Unchecked_5;
            if (moveCheckBox.IsChecked == true)
                MoveWotRadioButtonUnchecked();
            mapGrid.MouseMove -= WotCanvas_MouseMove;
            deleteButton.Click -= Button_Click_1;
        }

        private void ShowWotElements()
        {
            wotGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(wotCanvas, 1);
            wotCanvas.Opacity = 1;
            wotCanvas.IsEnabled = true;
            for (int i = 0; i < SelectedWots.Count; i++)
                SelectedWots[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedWot != null)
                SelectedWot.BorderBrush = Brushes.Orange;
            moveCheckBox.Checked += RadioButton_Checked_5;
            moveCheckBox.Unchecked += RadioButton_Unchecked_5;
            if (moveCheckBox.IsChecked == true)
                MoveWotRadioButtonChecked();
            mapGrid.MouseMove += WotCanvas_MouseMove;
            deleteButton.Click += Button_Click_1;
        }

        private void HidePlayerElements()
        {
            addPlayerCheckBox.IsChecked = false;
            playerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(playerCanvas, 0);
            playerCanvas.Opacity = 0.5;
            playerCanvas.IsEnabled = false;
            for (var i = 0; i < SelectedPlayers.Count; i++)
                SelectedPlayers[i].BorderBrush = Brushes.Transparent;
            moveCheckBox.Checked -= RadioButton_Checked_7;
            moveCheckBox.Unchecked -= RadioButton_Unchecked_7;
            if (moveCheckBox.IsChecked == true)
                MovePlayerRadioButtonUnchecked();
            mapGrid.MouseMove -= PlayerCanvas_MouseMove;
            deleteButton.Click -= Button_Click;
        }

        private void ShowPlayerElements()
        {
            playerGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(playerCanvas, 1);
            playerCanvas.Opacity = 1;
            playerCanvas.IsEnabled = true;
            for (var i = 0; i < SelectedPlayers.Count; i++)
                SelectedPlayers[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedPlayer != null)
                SelectedPlayer.BorderBrush = Brushes.Orange;
            moveCheckBox.Checked += RadioButton_Checked_7;
            moveCheckBox.Unchecked += RadioButton_Unchecked_7;
            if (moveCheckBox.IsChecked == true)
                MovePlayerRadioButtonChecked();
            mapGrid.MouseMove += PlayerCanvas_MouseMove;
            deleteButton.Click += Button_Click;
        }

        private void HidePathElements()
        {
            addPointPathRadioButton.IsChecked = newPathRadioButton.IsChecked = false;
            pathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(pathCanvas, 0);
            pathCanvas.Opacity = 0.5;
            pathCanvas.IsEnabled = false;
            for (int i = 0; i < SelectedPathPoints.Count; i++)
                SelectedPathPoints[i].BorderBrush = Brushes.Transparent;
            if (SelectedPath != null)
                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
            if (moveCheckBox.IsChecked == true)
                MovePathRadioButtonUnchecked();
            moveCheckBox.Checked -= RadioButton_Checked_9;
            moveCheckBox.Unchecked -= RadioButton_Unchecked_9;
            mapGrid.MouseMove -= PathCanvas_MouseMove;
            deleteButton.Click -= Button_Click_2;
        }

        private void ShowPathElements()
        {
            pathGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(pathCanvas, 1);
            pathCanvas.Opacity = 1;
            pathCanvas.IsEnabled = true;
            for (int i = 0; i < SelectedPathPoints.Count; i++)
                SelectedPathPoints[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedPath != null)
                SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
            if (SelectedPathPoint != null)
                SelectedPathPoint.BorderBrush = Brushes.Orange;
            if (moveCheckBox.IsChecked == true)
                MovePathRadioButtonChecked();
            moveCheckBox.Checked += RadioButton_Checked_9;
            moveCheckBox.Unchecked += RadioButton_Unchecked_9;
            mapGrid.MouseMove += PathCanvas_MouseMove;
            deleteButton.Click += Button_Click_2;
        }

        private void HidePolygonElements()
        {
            addPointPolygonRadioButton.IsChecked = newPolygonRadioButton.IsChecked = false;
            polygonGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(polygonCanvas, 0);
            polygonCanvas.Opacity = 0.5;
            polygonCanvas.IsEnabled = false;
            for (int i = 0; i < SelectedPolygonPoints.Count; i++)
                SelectedPolygonPoints[i].BorderBrush = Brushes.Transparent;
            if (SelectedPolygon != null)
                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
            if (moveCheckBox.IsChecked == true)
                MovePolygonRadioButtonUnchecked();
            moveCheckBox.Checked -= RadioButton_Checked_12;
            moveCheckBox.Unchecked -= RadioButton_Unchecked_12;
            mapGrid.MouseMove -= PolygonCanvas_MouseMove;
            deleteButton.Click -= Button_Click_3;
        }

        private void ShowPolygonElements()
        {
            polygonGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(polygonCanvas, 1);
            polygonCanvas.Opacity = 1;
            polygonCanvas.IsEnabled = true;
            for (int i = 0; i < SelectedPolygonPoints.Count; i++)
                SelectedPolygonPoints[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedPolygon != null)
                SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
            if (SelectedPolygonPoint != null)
                SelectedPolygonPoint.BorderBrush = Brushes.Orange;
            if (moveCheckBox.IsChecked == true)
                MovePolygonRadioButtonChecked();
            moveCheckBox.Checked += RadioButton_Checked_12;
            moveCheckBox.Unchecked += RadioButton_Unchecked_12;
            mapGrid.MouseMove += PolygonCanvas_MouseMove;
            deleteButton.Click += Button_Click_3;
        }

        private void HidePointElements()
        {
            addWorldPointCheckBox.IsChecked = false;
            pointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(pointCanvas, 0);
            pointCanvas.Opacity = 0.5;
            pointCanvas.IsEnabled = false;
            for (var i = 0; i < SelectedWorldPoints.Count; i++)
                SelectedWorldPoints[i].BorderBrush = Brushes.Transparent;
            if (moveCheckBox.IsChecked == true)
                MoveWorldPointCheckBoxUnchecked();
            moveCheckBox.Checked -= MoveCheckBox_Checked_WorldPoint;
            moveCheckBox.Unchecked -= MoveCheckBox_Unchecked_WorldPoint;
            mapGrid.MouseMove -= PointCanvas_MouseMove;
            deleteButton.Click -= DeleteButton_Click_WorldPoint;
        }

        private void ShowPointElements()
        {
            pointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(pointCanvas, 1);
            pointCanvas.Opacity = 1;
            pointCanvas.IsEnabled = true;
            for (int i = 0; i < SelectedWorldPoints.Count; i++)
                SelectedWorldPoints[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedWorldPoint != null)
                SelectedWorldPoint.BorderBrush = Brushes.Orange;
            if (moveCheckBox.IsChecked == true)
                MoveWorldPointCheckBoxChecked();
            moveCheckBox.Checked += MoveCheckBox_Checked_WorldPoint;
            moveCheckBox.Unchecked += MoveCheckBox_Unchecked_WorldPoint;
            mapGrid.MouseMove += PointCanvas_MouseMove;
            deleteButton.Click += DeleteButton_Click_WorldPoint;
        }

        private void HideObjectivePointElements()
        {
            addObjectivePointCheckBox.IsChecked = false;
            objectivePointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(objectivePointCanvas, 0);
            objectivePointCanvas.Opacity = 0.5;
            objectivePointCanvas.IsEnabled = false;
            for (var i = 0; i < SelectedObjectivePoints.Count; i++)
                SelectedObjectivePoints[i].BorderBrush = Brushes.Transparent;
            if (moveCheckBox.IsChecked == true)
                MoveObjectivePointCheckBoxUnchecked();
            moveCheckBox.Checked -= MoveCheckBox_Checked_ObjectivePoint;
            moveCheckBox.Unchecked -= MoveCheckBox_Unchecked_ObjectivePoint;
            mapGrid.MouseMove -= ObjectivePointCanvas_MouseMove;
            deleteButton.Click -= DeleteButton_Click_ObjectivePoint;
        }

        private void ShowObjectivePointElements()
        {
            objectivePointGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(objectivePointCanvas, 1);
            objectivePointCanvas.Opacity = 1;
            objectivePointCanvas.IsEnabled = true;
            for (int i = 0; i < SelectedObjectivePoints.Count; i++)
                SelectedObjectivePoints[i].BorderBrush = Brushes.OrangeRed;
            if (SelectedObjectivePoint != null)
                SelectedObjectivePoint.BorderBrush = Brushes.Orange;
            if (moveCheckBox.IsChecked == true)
                MoveObjectivePointCheckBoxChecked();
            moveCheckBox.Checked += MoveCheckBox_Checked_ObjectivePoint;
            moveCheckBox.Unchecked += MoveCheckBox_Unchecked_ObjectivePoint;
            mapGrid.MouseMove += ObjectivePointCanvas_MouseMove;
            deleteButton.Click += DeleteButton_Click_ObjectivePoint;
        }

        private void MapGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (wotRadioButton.IsChecked == true)
            {
                ClearWotSelection();
            }
            else if (playerRadioButton.IsChecked == true)
            {
                ClearPlayerSelection();
            }
            else if (pathRadioButton.IsChecked == true)
            {
                ClearPathPointSelection();
            }
            else if (polygonRadioButton.IsChecked == true)
            {
                ClearPolygonPointSelection();
            }
            else if (pointRadioButton.IsChecked == true)
            {
                ClearWorldPointSelection();
            }
            else if (objectivePointRadioButton.IsChecked == true)
            {
                ClearObjectivePointSelection();
            }
        }

        private void ClearWotSelection()
        {
            for(int i = 0; i < SelectedWots.Count; i++)
            {
                SelectedWots[i].BorderBrush = Brushes.Transparent;
            }
            SelectedWots.Clear();
            SelectedWot = null;
        }

        private void ClearPlayerSelection()
        {
            for(int i = 0; i < SelectedPlayers.Count; i++)
            {
                SelectedPlayers[i].BorderBrush = Brushes.Transparent;
            }
            SelectedPlayers.Clear();
            SelectedPlayer = null;
        }

        private void ClearPathPointSelection()
        {
            for (int i = 0; i < SelectedPathPoints.Count; i++)
            {
                SelectedPathPoints[i].BorderBrush = Brushes.Transparent;
            }
            if (SelectedPath != null)
                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
            SelectedPathPoints.Clear();
            SelectedPathPoint = null;
            SelectedPath = null;
        }

        private void ClearPolygonPointSelection()
        {
            for (int i = 0; i < SelectedPolygonPoints.Count; i++)
            {
                SelectedPolygonPoints[i].BorderBrush = Brushes.Transparent;
            }
            if (SelectedPolygon != null)
                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
            SelectedPolygonPoints.Clear();
            SelectedPolygonPoint = null;
            SelectedPolygon = null;
        }

        private void ClearWorldPointSelection()
        {
            for (int i = 0; i < SelectedWorldPoints.Count; i++)
            {
                SelectedWorldPoints[i].BorderBrush = Brushes.Transparent;
            }
            SelectedWorldPoints.Clear();
            SelectedWorldPoint = null;
        }

        private void ClearObjectivePointSelection()
        {
            for (int i = 0; i < SelectedObjectivePoints.Count; i++)
            {
                SelectedObjectivePoints[i].BorderBrush = Brushes.Transparent;
            }
            SelectedObjectivePoints.Clear();
            SelectedObjectivePoint = null;
        }

        #region WorldObject

        private void WotDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WotGridSelectedItem != null && wotRadioButton.IsChecked == true)
            {
                wotPreviewImage.Source = WotGridSelectedItem.Image;
                wotPreview.Visibility = Visibility.Visible;

            }
        }

        //wot canvas mouse move
        private void WotCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(wotCanvas);
            Canvas.SetLeft(wotPreview, mousePos.X - wotPreview.ActualWidth / 2);
            Canvas.SetTop(wotPreview, mousePos.Y - wotPreview.ActualHeight / 2);
        }

        private void AddWotControlToSelection(WotControl wotControl)
        {
            if (SelectedWot != null)
                SelectedWot.BorderBrush = Brushes.OrangeRed;
            SelectedWot = wotControl;
            SelectedWots.Add(SelectedWot);
            SelectedWot.BorderBrush = Brushes.Orange;
        }

        private void RemoveWotControlFromSelection(WotControl wotControl)
        {
            wotControl.BorderBrush = Brushes.Transparent;
            SelectedWots.Remove(wotControl);
            SelectedWot = null;
        }

        private void SelectWotControlFromSelection(WotControl wotControl)
        {
            if (SelectedWot != null)
                SelectedWot.BorderBrush = Brushes.OrangeRed;
            SelectedWot = wotControl;
            SelectedWot.BorderBrush = Brushes.Orange;
        }

        private void CreateWorldObject(float x, float y, float zRotation)
        {
            var wotControl = new WotControl
            (
                new(WotGridSelectedItem ?? WotGridItem.WotTypes.FirstOrDefault(), x, y, zRotation)
            );
            wotControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (wotControl.BorderBrush == Brushes.Transparent)
                        {
                            AddWotControlToSelection(wotControl);
                        }
                        //if already selected and current selected
                        else if (wotControl.BorderBrush == Brushes.Orange)
                        {
                            RemoveWotControlFromSelection(wotControl);
                        }
                        //if selected but not last selected
                        else
                        {
                            SelectWotControlFromSelection(wotControl);
                        }
                    }
                    else
                    {
                        ClearWotSelection();
                        AddWotControlToSelection(wotControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            wotControl.WorldObject.Remove = () =>
            {
                wotCanvas.Children.Remove(wotControl);
                if (SelectedWot != null && SelectedWot.WorldObject == wotControl.WorldObject)
                    SelectedWot = null;
            };
            Map.WorldObjects.Add(wotControl.WorldObject);
            wotCanvas.Children.Add(wotControl);
        }

        //wot preview mouse left
        private void WotPreview_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CreateWorldObject((float)(Canvas.GetLeft(wotPreview) + wotPreview.ActualWidth / 2), (float)(Canvas.GetTop(wotPreview) + wotPreview.ActualHeight / 2), (float)sliderRotate.Value);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        //wot preview mouse right
        private void WotPreview_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wotPreview.Visibility = Visibility.Hidden;
            wotDataGrid.SelectedItems.Clear();
        }

        //move wot action radio button checked
        private void RadioButton_Checked_5(object sender, RoutedEventArgs e)
        {
            MoveWotRadioButtonChecked();
        }

        private void MoveWotRadioButtonChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_2;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_2;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        //move wot action radio button unchecked
        private void RadioButton_Unchecked_5(object sender, RoutedEventArgs e)
        {
            MoveWotRadioButtonUnchecked();
        }

        private void MoveWotRadioButtonUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_2;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_2;
            mapGrid.Cursor = Cursors.Arrow;
        }

        //delete selected wot action button click
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < SelectedWots.Count; i++)
            {
                var selectedWot = SelectedWots[i];
                selectedWot.WorldObject.Group = null;
                wotCanvas.Children.Remove(selectedWot);
                Map.WorldObjects.Remove(selectedWot.WorldObject);
            }
            SelectedWot = null;
        }

        private void MapGrid_MouseLeftButtonDown_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(wotCanvas);
            mapGrid.MouseMove += MapGrid_MouseMoveWot;
        }

        private void MapGrid_MouseLeftButtonUp_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMoveWot;
        }

        private void MapGrid_MouseMoveWot(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(wotCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move wot selection
            for (var i = 0; i < SelectedWots.Count; i++)
            {
                var selectedWot = SelectedWots[i];
                Canvas.SetLeft(selectedWot, Canvas.GetLeft(selectedWot) + a);
                Canvas.SetTop(selectedWot, Canvas.GetTop(selectedWot) + b);
            }
            moveActionPoint = pos;
        }

        //lower z-index of selected wot control
        private void LowerZ_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < SelectedWots.Count; i++)
            {
                var selectedWot = SelectedWots[i];
                Canvas.SetZIndex(selectedWot, Canvas.GetZIndex(selectedWot) - 1);
            }
        }

        #endregion WorldObject

        #region Player

        //player canvas mouse move
        private void PlayerCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(playerCanvas);
            Canvas.SetLeft(playerPreview, mousePos.X - playerPreview.ActualWidth / 2);
            Canvas.SetTop(playerPreview, mousePos.Y - playerPreview.ActualHeight / 2);
        }

        //player preview mouse left
        private void PlayerPreview_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CreatePlayer((float)(Canvas.GetLeft(playerPreview) + playerPreview.ActualWidth / 2), (float)(Canvas.GetTop(playerPreview) + playerPreview.ActualHeight / 2), (float)sliderRotate.Value);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void AddPlayerControlToSelection(PlayerControl playerControl)
        {
            if (SelectedPlayer != null)
                SelectedPlayer.BorderBrush = Brushes.OrangeRed;
            SelectedPlayer = playerControl;
            SelectedPlayers.Add(SelectedPlayer);
            SelectedPlayer.BorderBrush = Brushes.Orange;
        }

        private void RemovePlayerControlFromSelection(PlayerControl playerControl)
        {
            playerControl.BorderBrush = Brushes.Transparent;
            SelectedPlayers.Remove(playerControl);
            SelectedPlayer = null;
        }

        private void SelectPlayerControlFromSelection(PlayerControl playerControl)
        {
            if (SelectedPlayer != null)
                SelectedPlayer.BorderBrush = Brushes.OrangeRed;
            SelectedPlayer = playerControl;
            SelectedPlayer.BorderBrush = Brushes.Orange;
        }

        private void CreatePlayer(float x, float y, float rotation)
        {
            var playerControl = new PlayerControl
            (
                new(NamedElement.GenerateName("Player", Map.Players), Map, x, y, 0, rotation, Colors.Red),
                playerPreviewImage.Source
            );
            playerControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (playerControl.BorderBrush == Brushes.Transparent)
                        {
                            AddPlayerControlToSelection(playerControl);
                        }
                        //if already selected and current selected
                        else if (playerControl.BorderBrush == Brushes.Orange)
                        {
                            RemovePlayerControlFromSelection(playerControl);
                        }
                        //if selected but not last selected
                        else
                        {
                            SelectPlayerControlFromSelection(playerControl);
                        }
                    }
                    else
                    {
                        ClearPlayerSelection();
                        AddPlayerControlToSelection(playerControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            playerControl.Player.Remove = () =>
            {
                playerCanvas.Children.Remove(playerControl);
                if (SelectedPlayer != null && SelectedPlayer.Player == playerControl.Player)
                    SelectedPlayer = null;
            };
            Map.Players.Add(playerControl.Player);
            playerCanvas.Children.Add(playerControl);
        }

        //player preview mouse right
        private void PlayerPreview_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //playerPreview.Visibility = Visibility.Hidden;
            addPlayerCheckBox.IsChecked = false;
        }

        private void EditPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlayer != null)
            {
                var cp = new ColorPicker(this, SelectedPlayer.Player.Color);
                if (cp.ShowDialog() == true)
                    SelectedPlayer.Player.Color = cp.NewColor;
            }
        }

        //move player action radio button checked
        private void RadioButton_Checked_7(object sender, RoutedEventArgs e)
        {
            MovePlayerRadioButtonChecked();
        }

        private void MovePlayerRadioButtonChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_Player;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_Player;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        //move player action radio button unchecked
        private void RadioButton_Unchecked_7(object sender, RoutedEventArgs e)
        {
            MovePlayerRadioButtonUnchecked();
        }

        private void MovePlayerRadioButtonUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_Player;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_Player;
            mapGrid.Cursor = Cursors.Arrow;
        }

        private void MapGrid_MouseLeftButtonDown_Player(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(playerCanvas);
            mapGrid.MouseMove += MapGrid_MouseMovePlayer;
        }

        private void MapGrid_MouseLeftButtonUp_Player(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMovePlayer;
        }

        private void MapGrid_MouseMovePlayer(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(playerCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move player selection
            for (var i = 0; i < SelectedPlayers.Count; i++)
            {
                var selectedPlayer = SelectedPlayers[i];
                Canvas.SetLeft(selectedPlayer, Canvas.GetLeft(selectedPlayer) + a);
                Canvas.SetTop(selectedPlayer, Canvas.GetTop(selectedPlayer) + b);
            }
            moveActionPoint = pos;
        }

        //delete selected players action button click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < SelectedPlayers.Count; i++)
            {
                var selectedPlayer = SelectedPlayers[i];
                selectedPlayer.Player.Remove?.Invoke();
                Map.Players.Remove(selectedPlayer.Player);
            }
            SelectedPlayer = null;
        }

        #endregion Player

        #region Path

        //edit path color
        private void EditWaypointPathColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPath != null)
            {
                var cp = new ColorPicker(this, SelectedPath.WaypointPath.Color, 255);
                if (cp.ShowDialog() == true)
                    SelectedPath.WaypointPath.Color = cp.NewColor;
            }
        }

        //new path radio button checked
        private void RadioButton_Checked_6(object sender, RoutedEventArgs e)
        {
            pathPointPreview.Visibility = Visibility.Visible;
            pathPointPreview.MouseLeftButtonDown += PathPointPreview_MouseLeftButtonDown;
        }

        //new path radio button unchecked
        private void RadioButton_Unchecked_6(object sender, RoutedEventArgs e)
        {
            pathPointPreview.Visibility = Visibility.Collapsed;
            pathPointPreview.MouseLeftButtonDown -= PathPointPreview_MouseLeftButtonDown;
        }

        //add point path radio button checked
        private void RadioButton_Checked_8(object sender, RoutedEventArgs e)
        {
            pathPointPreview.Visibility = Visibility.Visible;
            pathPointPreview.MouseLeftButtonDown += PathPointPreview_MouseLeftButtonDown_1;
        }

        //add point path radio button unchecked
        private void RadioButton_Unchecked_8(object sender, RoutedEventArgs e)
        {
            pathPointPreview.Visibility = Visibility.Collapsed;
            pathPointPreview.MouseLeftButtonDown -= PathPointPreview_MouseLeftButtonDown_1;
        }

        //path canvas mouse move
        private void PathCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(pathCanvas);
            Canvas.SetLeft(pathPointPreview, mousePos.X - pathPointPreview.ActualWidth / 2);
            Canvas.SetTop(pathPointPreview, mousePos.Y - pathPointPreview.ActualHeight / 2);
        }

        //new path point preview mouse left
        private void PathPointPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var waypointPath = new WaypointPath(NamedElement.GenerateName("Path", Map.WaypointPaths), Map);
            Map.WaypointPaths.Add(waypointPath);

            var path = new System.Windows.Shapes.Path
            {
                Stroke = new SolidColorBrush(),
                StrokeThickness = 7
            };
            BindingOperations.SetBinding(path.Stroke, SolidColorBrush.ColorProperty, new Binding("Color") { Source = waypointPath });

            var outlinePath = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Transparent,
                StrokeThickness = 17
            };

            var pathGeometry = new PathGeometry();
            path.Data = outlinePath.Data = pathGeometry;

            var pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);

            var point = new Point3((float)(Canvas.GetLeft(pathPointPreview) + pathPointPreview.ActualWidth / 2), (float)(Canvas.GetTop(pathPointPreview) + pathPointPreview.ActualHeight / 2), 0);
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = point });

            var pathControl = new PathControl(path, outlinePath, pathFigure, waypointPath);
            path.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (outlinePath.Stroke == Brushes.Transparent)
                        {
                            if (SelectedPath != null)
                                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
                            SelectedPath = pathControl;
                            SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
                            for(int i = 0; i < SelectedPath.PathPointControls.Count; i++)
                            {
                                var tempPointControl = SelectedPath.PathPointControls[i];
                                tempPointControl.BorderBrush = Brushes.OrangeRed;
                                if (!SelectedPathPoints.Contains(tempPointControl))
                                    SelectedPathPoints.Add(tempPointControl);
                            }
                            if (SelectedPathPoint != null)
                            {
                                SelectedPathPoint.BorderBrush = Brushes.OrangeRed;
                                SelectedPathPoint = null;
                            }
                        }
                        //if already selected and current selected
                        else
                        {
                            outlinePath.Stroke = Brushes.Transparent;
                            for(int i=0;i<pathControl.PathPointControls.Count;i++)
                            {
                                var pathPointControl = pathControl.PathPointControls[i];
                                pathPointControl.BorderBrush = Brushes.Transparent;
                                SelectedPathPoints.Remove(pathPointControl);
                            }
                            SelectedPath = null;
                            if (SelectedPathPoint != null && SelectedPathPoint.BorderBrush == Brushes.Transparent)
                                SelectedPathPoint = null;
                        }
                    }
                    else
                    {
                        ClearPathPointSelection();
                        SelectedPath = pathControl;
                        SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
                        for (int i = 0; i < SelectedPath.PathPointControls.Count; i++)
                        {
                            var tempPointControl = SelectedPath.PathPointControls[i];
                            tempPointControl.BorderBrush = Brushes.OrangeRed;
                            if (!SelectedPathPoints.Contains(tempPointControl))
                                SelectedPathPoints.Add(tempPointControl);
                        }
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            pathCanvas.Children.Add(outlinePath);
            pathCanvas.Children.Add(path);
            if (SelectedPath != null)
                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
            SelectedPath = pathControl;
            SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
            if (SelectedPathPoint != null)
                SelectedPathPoint.BorderBrush = Brushes.OrangeRed;
            CreatePathPointControl(pathControl, waypointPath, point);
            addPointPathRadioButton.IsChecked = true;
            e.Handled = true;
        }

        //add path point preview mouse left
        private void PathPointPreview_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (SelectedPath != null)
            {
                var lineSegment = new LineSegment();
                var point = new Point3((float)(Canvas.GetLeft(pathPointPreview) + pathPointPreview.ActualWidth / 2), (float)(Canvas.GetTop(pathPointPreview) + pathPointPreview.ActualHeight / 2), 0);
                BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, new Binding("Point") { Source = point });
                SelectedPath.PathFigure.Segments.Add(lineSegment);
                CreatePathPointControl(SelectedPath, SelectedPath.WaypointPath, point);
                e.Handled = true;
            }
        }

        private void CreatePathPointControl(PathControl currentPathControl, WaypointPath path, Point3 point)
        {
            //var currentPathControl = SelectedPath;
            var pathPointControl = new PathPointControl(currentPathControl, path, point);
            pathPointControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (pathPointControl.BorderBrush == Brushes.Transparent)
                        {
                            if (SelectedPath != null)
                                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
                            if (SelectedPathPoint != null)
                                SelectedPathPoint.BorderBrush = Brushes.OrangeRed;
                            SelectedPathPoint = pathPointControl;
                            SelectedPath = currentPathControl;
                            if (SelectedPath != null)
                                SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
                            SelectedPathPoints.Add(SelectedPathPoint);
                            SelectedPathPoint.BorderBrush = Brushes.Orange;
                        }
                        //if already selected and current selected
                        else if (pathPointControl.BorderBrush == Brushes.Orange)
                        {
                            if (SelectedPath != null)
                            {
                                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
                                SelectedPath = null;
                            }
                            pathPointControl.BorderBrush = Brushes.Transparent;
                            SelectedPathPoints.Remove(pathPointControl);
                            SelectedPathPoint = null;
                        }
                        //if selected but not last selected
                        else
                        {
                            if (SelectedPath != null)
                                SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
                            if (SelectedPathPoint != null)
                                SelectedPathPoint.BorderBrush = Brushes.OrangeRed;
                            SelectedPathPoint = pathPointControl;
                            SelectedPath = currentPathControl;
                            if (SelectedPath != null)
                                SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
                            SelectedPathPoint.BorderBrush = Brushes.Orange;
                        }
                    }
                    else
                    {
                        ClearPathPointSelection();
                        SelectedPathPoint = pathPointControl;
                        SelectedPath = currentPathControl;
                        if (SelectedPath != null)
                            SelectedPath.OutlinePath.Stroke = Brushes.OrangeRed;
                        SelectedPathPoint.BorderBrush = Brushes.Orange;
                        SelectedPathPoints.Add(pathPointControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            path.Points.Add(point);
            SelectedPath?.PathPointControls.Add(pathPointControl);
            pathCanvas.Children.Add(pathPointControl);
        }

        //path point preview mouse right
        private void PathPointPreview_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            newPathRadioButton.IsChecked = addPointPathRadioButton.IsChecked = false;
        }

        //move path action radio button checked
        private void RadioButton_Checked_9(object sender, RoutedEventArgs e)
        {
            MovePathRadioButtonChecked();
        }

        //move path action radio button unchecked
        private void RadioButton_Unchecked_9(object sender, RoutedEventArgs e)
        {
            MovePathRadioButtonUnchecked();
        }

        private void MovePathRadioButtonChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_3;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_3;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        private void MovePathRadioButtonUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_3;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_3;
            mapGrid.Cursor = Cursors.Arrow;
        }

        private void MapGrid_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(pathCanvas);
            mapGrid.MouseMove += MapGrid_MouseMovePathPoint;
        }

        private void MapGrid_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMovePathPoint;
        }

        private void MapGrid_MouseMovePathPoint(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(pathCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move path point selection
            for (var i = 0; i < SelectedPathPoints.Count; i++)
            {
                var selectedPathPoint = SelectedPathPoints[i];
                Canvas.SetLeft(selectedPathPoint, Canvas.GetLeft(selectedPathPoint) + a);
                Canvas.SetTop(selectedPathPoint, Canvas.GetTop(selectedPathPoint) + b);
            }
            moveActionPoint = pos;
        }

        //delete path points
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < SelectedPathPoints.Count; i++)
            {
                var selectedPathPoint = SelectedPathPoints[i];
                var index = selectedPathPoint.PathControl.PathPointControls.FindIndex((control) =>
                {
                    return selectedPathPoint == control;
                });
                //not start point
                if (index > 0)
                {
                    selectedPathPoint.PathControl.PathFigure.Segments.RemoveAt(index - 1);
                    selectedPathPoint.PathControl.PathPointControls.RemoveAt(index);
                }
                //start point
                else if (index == 0)
                {
                    //if one point left, remove the waypointPath from Map and path from pathCanvas
                    if(selectedPathPoint.PathControl.WaypointPath.Points.Count == 1 && selectedPathPoint.PathControl.PathPointControls.Count == 1)
                    {
                        Map.WaypointPaths.Remove(selectedPathPoint.PathControl.WaypointPath);
                        pathCanvas.Children.Remove(selectedPathPoint.PathControl.Path);
                        pathCanvas.Children.Remove(selectedPathPoint.PathControl.OutlinePath);
                    }
                    //at least 2 points left
                    else
                    {
                        var nextPathPointControl = selectedPathPoint.PathControl.PathPointControls[1];
                        BindingOperations.SetBinding(selectedPathPoint.PathControl.PathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = nextPathPointControl.Point });
                        selectedPathPoint.PathControl.PathFigure.Segments.RemoveAt(index);
                        selectedPathPoint.PathControl.PathPointControls.RemoveAt(index);
                    }
                }
                selectedPathPoint.PathControl.WaypointPath.Points.Remove(selectedPathPoint.Point);
                pathCanvas.Children.Remove(selectedPathPoint);
                if (SelectedPath != null)
                {
                    SelectedPath.OutlinePath.Stroke = Brushes.Transparent;
                    SelectedPath = null;
                }
                SelectedPathPoint = null;
            }
            SelectedPathPoints.Clear();
        }

        #endregion Path

        #region Polygon

        //edit polygon color
        private void EditWorldPolygonColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPolygon != null)
            {
                var cp = new ColorPicker(this, SelectedPolygon.WorldPolygon.Color, 255);
                if (cp.ShowDialog() == true)
                    SelectedPolygon.WorldPolygon.Color = cp.NewColor;
            }
        }

        //new polygon radio button checked
        private void RadioButton_Checked_10(object sender, RoutedEventArgs e)
        {
            polygonPointPreview.Visibility = Visibility.Visible;
            polygonPointPreview.MouseLeftButtonDown += PolygonPointPreview_MouseLeftButtonDown;
        }

        //new polygon radio button unchecked
        private void RadioButton_Unchecked_10(object sender, RoutedEventArgs e)
        {
            polygonPointPreview.Visibility = Visibility.Collapsed;
            polygonPointPreview.MouseLeftButtonDown -= PolygonPointPreview_MouseLeftButtonDown;
        }

        //add polygon path radio button checked
        private void RadioButton_Checked_11(object sender, RoutedEventArgs e)
        {
            polygonPointPreview.Visibility = Visibility.Visible;
            polygonPointPreview.MouseLeftButtonDown += PolygonPointPreview_MouseLeftButtonDown_1;
        }

        //add polygon path radio button unchecked
        private void RadioButton_Unchecked_11(object sender, RoutedEventArgs e)
        {
            polygonPointPreview.Visibility = Visibility.Collapsed;
            polygonPointPreview.MouseLeftButtonDown -= PolygonPointPreview_MouseLeftButtonDown_1;
        }

        //polygon canvas mouse move
        private void PolygonCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(polygonCanvas);
            Canvas.SetLeft(polygonPointPreview, mousePos.X - polygonPointPreview.ActualWidth / 2);
            Canvas.SetTop(polygonPointPreview, mousePos.Y - polygonPointPreview.ActualHeight / 2);
        }

        //new polygon point preview mouse left
        private void PolygonPointPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var worldPolygon = new WorldPolygon(NamedElement.GenerateName("Polygon", Map.WorldPolygons), Map);
            Map.WorldPolygons.Add(worldPolygon);

            var path = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(),
                Stroke = new SolidColorBrush(),
                StrokeThickness = 7
            };
            BindingOperations.SetBinding(path.Stroke, SolidColorBrush.ColorProperty, new Binding("Color") { Source = worldPolygon });
            BindingOperations.SetBinding(path.Fill, SolidColorBrush.ColorProperty, new Binding("Color") { Source = worldPolygon, Converter = new FillColorConverter() });

            var outlinePath = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Transparent,
                StrokeThickness = 17
            };

            var pathGeometry = new PathGeometry();
            path.Data = outlinePath.Data = pathGeometry;

            var pathFigure = new PathFigure() { IsClosed = true };
            pathGeometry.Figures.Add(pathFigure);

            var point = new Point2((float)(Canvas.GetLeft(polygonPointPreview) + polygonPointPreview.ActualWidth / 2), (float)(Canvas.GetTop(polygonPointPreview) + polygonPointPreview.ActualHeight / 2));
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = point });

            var polygonControl = new PolygonControl(path, outlinePath, pathFigure, worldPolygon);
            path.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (outlinePath.Stroke == Brushes.Transparent)
                        {
                            if (SelectedPolygon != null)
                                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
                            SelectedPolygon = polygonControl;
                            SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
                            for (int i = 0; i < SelectedPolygon.PolygonPointControls.Count; i++)
                            {
                                var tempPointControl = SelectedPolygon.PolygonPointControls[i];
                                tempPointControl.BorderBrush = Brushes.OrangeRed;
                                if (!SelectedPolygonPoints.Contains(tempPointControl))
                                    SelectedPolygonPoints.Add(tempPointControl);
                            }
                            if (SelectedPolygonPoint != null)
                            {
                                SelectedPolygonPoint.BorderBrush = Brushes.OrangeRed;
                                SelectedPolygonPoint = null;
                            }
                        }
                        //if already selected and current selected
                        else
                        {
                            outlinePath.Stroke = Brushes.Transparent;
                            for (int i = 0; i < polygonControl.PolygonPointControls.Count; i++)
                            {
                                var pathPointControl = polygonControl.PolygonPointControls[i];
                                pathPointControl.BorderBrush = Brushes.Transparent;
                                SelectedPolygonPoints.Remove(pathPointControl);
                            }
                            SelectedPolygon = null;
                            if (SelectedPolygonPoint != null && SelectedPolygonPoint.BorderBrush == Brushes.Transparent)
                                SelectedPolygonPoint = null;
                        }
                    }
                    else
                    {
                        ClearPolygonPointSelection();
                        SelectedPolygon = polygonControl;
                        SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
                        for (int i = 0; i < SelectedPolygon.PolygonPointControls.Count; i++)
                        {
                            var tempPointControl = SelectedPolygon.PolygonPointControls[i];
                            tempPointControl.BorderBrush = Brushes.OrangeRed;
                            if (!SelectedPolygonPoints.Contains(tempPointControl))
                                SelectedPolygonPoints.Add(tempPointControl);
                        }
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            polygonCanvas.Children.Add(outlinePath);
            polygonCanvas.Children.Add(path);
            if (SelectedPolygon != null)
                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
            SelectedPolygon = polygonControl;
            SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
            if (SelectedPolygonPoint != null)
                SelectedPolygonPoint.BorderBrush = Brushes.OrangeRed;
            CreatePolygonPointControl(polygonControl, worldPolygon, point);
            addPointPolygonRadioButton.IsChecked = true;
            e.Handled = true;
        }

        //add point polygon point preview mouse left
        private void PolygonPointPreview_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (SelectedPolygon != null)
            {
                var lineSegment = new LineSegment();
                var point = new Point2((float)(Canvas.GetLeft(polygonPointPreview) + polygonPointPreview.ActualWidth / 2), (float)(Canvas.GetTop(polygonPointPreview) + polygonPointPreview.ActualHeight / 2));
                BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty, new Binding("Point") { Source = point });
                SelectedPolygon.PathFigure.Segments.Add(lineSegment);
                CreatePolygonPointControl(SelectedPolygon, SelectedPolygon.WorldPolygon, point);
                e.Handled = true;
            }
        }

        private void CreatePolygonPointControl(PolygonControl currentPolygonControl, WorldPolygon polygon, Point2 point)
        {
            var polygonPointControl = new PolygonPointControl(currentPolygonControl, polygon, point);
            polygonPointControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (polygonPointControl.BorderBrush == Brushes.Transparent)
                        {
                            if (SelectedPolygon != null)
                                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
                            if (SelectedPolygonPoint != null)
                                SelectedPolygonPoint.BorderBrush = Brushes.OrangeRed;
                            SelectedPolygonPoint = polygonPointControl;
                            SelectedPolygon = currentPolygonControl;
                            if (SelectedPolygon != null)
                                SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
                            SelectedPolygonPoints.Add(SelectedPolygonPoint);
                            SelectedPolygonPoint.BorderBrush = Brushes.Orange;
                        }
                        //if already selected and current selected
                        else if (polygonPointControl.BorderBrush == Brushes.Orange)
                        {
                            if (SelectedPolygon != null)
                            {
                                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
                                SelectedPolygon = null;
                            }
                            polygonPointControl.BorderBrush = Brushes.Transparent;
                            SelectedPolygonPoints.Remove(polygonPointControl);
                            SelectedPolygonPoint = null;
                        }
                        //if selected but not last selected
                        else
                        {
                            if (SelectedPolygon != null)
                                SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
                            if (SelectedPolygonPoint != null)
                                SelectedPolygonPoint.BorderBrush = Brushes.OrangeRed;
                            SelectedPolygonPoint = polygonPointControl;
                            SelectedPolygon = currentPolygonControl;
                            if (SelectedPolygon != null)
                                SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
                            SelectedPolygonPoint.BorderBrush = Brushes.Orange;
                        }
                    }
                    else
                    {
                        ClearPolygonPointSelection();
                        SelectedPolygonPoint = polygonPointControl;
                        SelectedPolygon = currentPolygonControl;
                        if (SelectedPolygon != null)
                            SelectedPolygon.OutlinePath.Stroke = Brushes.OrangeRed;
                        SelectedPolygonPoint.BorderBrush = Brushes.Orange;
                        SelectedPolygonPoints.Add(polygonPointControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            polygon.Points.Add(point);
            SelectedPolygon?.PolygonPointControls.Add(polygonPointControl);
            polygonCanvas.Children.Add(polygonPointControl);
        }

        //polygon point preview mouse right
        private void PolygonPointPreview_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            newPolygonRadioButton.IsChecked = addPointPolygonRadioButton.IsChecked = false;
        }

        //move polygon action radio button checked
        private void RadioButton_Checked_12(object sender, RoutedEventArgs e)
        {
            MovePolygonRadioButtonChecked();
        }

        //move polygon action radio button unchecked
        private void RadioButton_Unchecked_12(object sender, RoutedEventArgs e)
        {
            MovePolygonRadioButtonUnchecked();
        }

        private void MovePolygonRadioButtonChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_4;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_4;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        private void MovePolygonRadioButtonUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_4;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_4;
            mapGrid.Cursor = Cursors.Arrow;
        }

        private void MapGrid_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(polygonCanvas);
            mapGrid.MouseMove += MapGrid_MouseMovePolygonPoint;
        }

        private void MapGrid_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMovePolygonPoint;
        }

        private void MapGrid_MouseMovePolygonPoint(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(polygonCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move polygon point selection
            for (var i = 0; i < SelectedPolygonPoints.Count; i++)
            {
                var selectedPolygonPoint = SelectedPolygonPoints[i];
                Canvas.SetLeft(selectedPolygonPoint, Canvas.GetLeft(selectedPolygonPoint) + a);
                Canvas.SetTop(selectedPolygonPoint, Canvas.GetTop(selectedPolygonPoint) + b);
            }
            moveActionPoint = pos;
        }

        //delete polygon points
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < SelectedPolygonPoints.Count; i++)
            {
                var selectedPolygonPoint = SelectedPolygonPoints[i];
                var index = selectedPolygonPoint.PolygonControl.PolygonPointControls.FindIndex((control) =>
                {
                    return selectedPolygonPoint == control;
                });
                //not start point
                if (index > 0)
                {
                    selectedPolygonPoint.PolygonControl.PathFigure.Segments.RemoveAt(index - 1);
                    selectedPolygonPoint.PolygonControl.PolygonPointControls.RemoveAt(index);
                }
                //start point
                else if (index == 0)
                {
                    //if one point left, remove the waypointPath from Map and path from pathCanvas
                    if (selectedPolygonPoint.PolygonControl.WorldPolygon.Points.Count == 1 && selectedPolygonPoint.PolygonControl.PolygonPointControls.Count == 1)
                    {
                        Map.WorldPolygons.Remove(selectedPolygonPoint.PolygonControl.WorldPolygon);
                        pathCanvas.Children.Remove(selectedPolygonPoint.PolygonControl.Path);
                        pathCanvas.Children.Remove(selectedPolygonPoint.PolygonControl.OutlinePath);
                    }
                    //at least 2 points left
                    else
                    {
                        var nextPolygonPointControl = selectedPolygonPoint.PolygonControl.PolygonPointControls[1];
                        BindingOperations.SetBinding(selectedPolygonPoint.PolygonControl.PathFigure, PathFigure.StartPointProperty, new Binding("Point") { Source = nextPolygonPointControl.Point });
                        selectedPolygonPoint.PolygonControl.PathFigure.Segments.RemoveAt(index);
                        selectedPolygonPoint.PolygonControl.PolygonPointControls.RemoveAt(index);
                    }
                }
                selectedPolygonPoint.PolygonControl.WorldPolygon.Points.Remove(selectedPolygonPoint.Point);
                polygonCanvas.Children.Remove(selectedPolygonPoint);
                if (SelectedPolygon != null)
                {
                    SelectedPolygon.OutlinePath.Stroke = Brushes.Transparent;
                    SelectedPolygon = null;
                }
                SelectedPolygonPoint = null;
            }
            SelectedPolygonPoints.Clear();
        }

        #endregion Polygon

        #region WorldPoint

        //point canvas mouse move
        private void PointCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(pointCanvas);
            Canvas.SetLeft(worldPointPreview, mousePos.X - worldPointPreview.ActualWidth / 2);
            Canvas.SetTop(worldPointPreview, mousePos.Y - worldPointPreview.ActualHeight / 2);
        }

        // world point preview mouse right
        private void WorldPointPreview_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            addWorldPointCheckBox.IsChecked = false;
        }

        private void AddWorldPointControlToSelection(WorldPointControl worldPointControl)
        {
            if (SelectedWorldPoint != null)
                SelectedWorldPoint.BorderBrush = Brushes.OrangeRed;
            SelectedWorldPoint = worldPointControl;
            SelectedWorldPoints.Add(worldPointControl);
            SelectedWorldPoint.BorderBrush = Brushes.Orange;
        }

        private void RemoveWorldPointControlFromSelection(WorldPointControl worldPointControl)
        {
            worldPointControl.BorderBrush = Brushes.Transparent;
            SelectedWorldPoints.Remove(worldPointControl);
            SelectedWorldPoint = null;
        }

        private void SelectWorldPointControlFromSelection(WorldPointControl worldPointControl)
        {
            if (SelectedWorldPoint != null)
                SelectedWorldPoint.BorderBrush = Brushes.OrangeRed;
            SelectedWorldPoint = worldPointControl;
            SelectedWorldPoint.BorderBrush = Brushes.Orange;
        }

        // world point preview mouse left
        private void WorldPointPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var worldPointControl = new WorldPointControl
            (
                new(NamedElement.GenerateName("Point", Map.WorldPoints), Map, (float)(Canvas.GetLeft(worldPointPreview) + worldPointPreview.ActualWidth / 2), (float)(Canvas.GetTop(worldPointPreview) + worldPointPreview.ActualHeight / 2), (float)sliderRotate.Value)
            );
            worldPointControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (worldPointControl.BorderBrush == Brushes.Transparent)
                        {
                            AddWorldPointControlToSelection(worldPointControl);
                        }
                        //if already selected and current selected
                        else if (worldPointControl.BorderBrush == Brushes.Orange)
                        {
                            RemoveWorldPointControlFromSelection(worldPointControl);
                        }
                        //if selected but not last selected
                        else
                        {
                            SelectWorldPointControlFromSelection(worldPointControl);
                        }
                    }
                    else
                    {
                        ClearWorldPointSelection();
                        AddWorldPointControlToSelection(worldPointControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            Map.WorldPoints.Add(worldPointControl.WorldPoint);
            pointCanvas.Children.Add(worldPointControl);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        //move world point action radio button checked
        private void MoveCheckBox_Checked_WorldPoint(object sender, RoutedEventArgs e)
        {
            MoveWorldPointCheckBoxChecked();
        }

        //move world point action radio button unchecked
        private void MoveCheckBox_Unchecked_WorldPoint(object sender, RoutedEventArgs e)
        {
            MoveWorldPointCheckBoxUnchecked();
        }

        private void MoveWorldPointCheckBoxChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_MoveWorldPoint;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_MoveWorldPoint;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        private void MoveWorldPointCheckBoxUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_MoveWorldPoint;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_MoveWorldPoint;
            mapGrid.Cursor = Cursors.Arrow;
        }

        //begin move world point
        private void MapGrid_MouseLeftButtonDown_MoveWorldPoint(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(pointCanvas);
            mapGrid.MouseMove += MapGrid_MouseMoveWorldPoint;
        }

        //end move world point
        private void MapGrid_MouseLeftButtonUp_MoveWorldPoint(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMoveWorldPoint;
        }

        private void MapGrid_MouseMoveWorldPoint(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(pointCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move world point selection
            for (var i = 0; i < SelectedWorldPoints.Count; i++)
            {
                var selectedWorldPoint = SelectedWorldPoints[i];
                Canvas.SetLeft(selectedWorldPoint, Canvas.GetLeft(selectedWorldPoint) + a);
                Canvas.SetTop(selectedWorldPoint, Canvas.GetTop(selectedWorldPoint) + b);
            }
            moveActionPoint = pos;
        }

        // delete world points
        private void DeleteButton_Click_WorldPoint(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < SelectedWorldPoints.Count; i++)
            {
                var selectedPointControl = SelectedWorldPoints[i];
                pointCanvas.Children.Remove(selectedPointControl);
                Map.WorldPoints.Remove(selectedPointControl.WorldPoint);
            }
            SelectedWorldPoint = null;
        }

        //edit path color
        private void EditWorldPointColor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorldPoint != null)
            {
                var cp = new ColorPicker(this, SelectedWorldPoint.WorldPoint.Color, 255);
                if (cp.ShowDialog() == true)
                    SelectedWorldPoint.WorldPoint.Color = cp.NewColor;
            }
        }

        #endregion WorldPoint

        #region ObjectivePoint

        //objective canvas mouse move
        private void ObjectivePointCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePos = e.GetPosition(objectivePointCanvas);
            Canvas.SetLeft(objectivePointPreview, mousePos.X - objectivePointPreview.ActualWidth / 2);
            Canvas.SetTop(objectivePointPreview, mousePos.Y - objectivePointPreview.ActualHeight / 2);
        }

        //objective point preview mouse right
        private void ObjectivePointPreview_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            addObjectivePointCheckBox.IsChecked = false;
        }

        private void AddObjectivePointControlToSelection(ObjectivePointControl objectivePointControl)
        {
            if (SelectedObjectivePoint != null)
                SelectedObjectivePoint.BorderBrush = Brushes.OrangeRed;
            SelectedObjectivePoint = objectivePointControl;
            SelectedObjectivePoints.Add(objectivePointControl);
            SelectedObjectivePoint.BorderBrush = Brushes.Orange;
        }

        private void RemoveObjectivePointControlFromSelection(ObjectivePointControl objectivePointControl)
        {
            objectivePointControl.BorderBrush = Brushes.Transparent;
            SelectedObjectivePoints.Remove(objectivePointControl);
            SelectedObjectivePoint = null;
        }

        private void SelectObjectivePointControlFromSelection(ObjectivePointControl objectivePointControl)
        {
            if (SelectedObjectivePoint != null)
                SelectedObjectivePoint.BorderBrush = Brushes.OrangeRed;
            SelectedObjectivePoint = objectivePointControl;
            SelectedObjectivePoint.BorderBrush = Brushes.Orange;
        }

        //objective point preview mouse left
        private void ObjectivePointPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var objectivePointControl = new ObjectivePointControl
            (
                new(Map, NamedElement.GenerateName("ObjectivePoint", Map.ObjectivePoints), (float)(Canvas.GetLeft(objectivePointPreview) + objectivePointPreview.ActualWidth / 2), (float)(Canvas.GetTop(objectivePointPreview) + objectivePointPreview.ActualHeight / 2))
            );
            objectivePointControl.MouseLeftButtonDown += (s, e1) =>
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        //if not selected yet
                        if (objectivePointControl.BorderBrush == Brushes.Transparent)
                        {
                            AddObjectivePointControlToSelection(objectivePointControl);
                        }
                        //if already selected and current selected
                        else if (objectivePointControl.BorderBrush == Brushes.Orange)
                        {
                            RemoveObjectivePointControlFromSelection(objectivePointControl);
                        }
                        //if selected but not last selected
                        else
                        {
                            SelectObjectivePointControlFromSelection(objectivePointControl);
                        }
                    }
                    else
                    {
                        ClearObjectivePointSelection();
                        AddObjectivePointControlToSelection(objectivePointControl);
                    }
                    if (moveCheckBox.IsChecked == false)
                        e1.Handled = true;
                }
            };
            Map.ObjectivePoints.Add(objectivePointControl.ObjectivePoint);
            objectivePointCanvas.Children.Add(objectivePointControl);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        //move world point action radio button checked
        private void MoveCheckBox_Checked_ObjectivePoint(object sender, RoutedEventArgs e)
        {
            MoveObjectivePointCheckBoxChecked();
        }

        //move world point action radio button unchecked
        private void MoveCheckBox_Unchecked_ObjectivePoint(object sender, RoutedEventArgs e)
        {
            MoveObjectivePointCheckBoxUnchecked();
        }

        private void MoveObjectivePointCheckBoxChecked()
        {
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown_MoveObjectivePoint;
            mapGrid.MouseLeftButtonUp += MapGrid_MouseLeftButtonUp_MoveObjectivePoint;
            mapGrid.Cursor = Cursors.SizeAll;
        }

        private void MoveObjectivePointCheckBoxUnchecked()
        {
            mapGrid.MouseLeftButtonDown += MapGrid_MouseLeftButtonDown;
            mapGrid.MouseLeftButtonDown -= MapGrid_MouseLeftButtonDown_MoveObjectivePoint;
            mapGrid.MouseLeftButtonUp -= MapGrid_MouseLeftButtonUp_MoveObjectivePoint;
            mapGrid.Cursor = Cursors.Arrow;
        }

        //begin move objective point
        private void MapGrid_MouseLeftButtonDown_MoveObjectivePoint(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(mapGrid);
            moveActionPoint = e.GetPosition(objectivePointCanvas);
            mapGrid.MouseMove += MapGrid_MouseMoveObjectivePoint;
        }

        //end move objective point
        private void MapGrid_MouseLeftButtonUp_MoveObjectivePoint(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mapGrid.MouseMove -= MapGrid_MouseMoveObjectivePoint;
        }

        //move objective point (while mouse left down)
        private void MapGrid_MouseMoveObjectivePoint(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(objectivePointCanvas);
            var a = pos.X - moveActionPoint.X;
            var b = pos.Y - moveActionPoint.Y;
            //move objective point selection
            for (var i = 0; i < SelectedObjectivePoints.Count; i++)
            {
                var selectedObjectivePoint = SelectedObjectivePoints[i];
                Canvas.SetLeft(selectedObjectivePoint, Canvas.GetLeft(selectedObjectivePoint) + a);
                Canvas.SetTop(selectedObjectivePoint, Canvas.GetTop(selectedObjectivePoint) + b);
            }
            moveActionPoint = pos;
        }

        //delete objective points
        private void DeleteButton_Click_ObjectivePoint(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < SelectedObjectivePoints.Count; i++)
            {
                var selectedObjectivePointControl = SelectedObjectivePoints[i];
                objectivePointCanvas.Children.Remove(selectedObjectivePointControl);
                Map.ObjectivePoints.Remove(selectedObjectivePointControl.ObjectivePoint);
            }
            SelectedObjectivePoint = null;
        }

        #endregion ObjectivePoint

    }
}
