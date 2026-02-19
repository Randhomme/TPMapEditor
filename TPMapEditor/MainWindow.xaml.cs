using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using TPMapEditor.ViewModel;

namespace TPMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        private readonly MainViewModel vm;
        private Point selectActionPoint;
        private Point moveActionPoint;
        private DateTime lastWheelTime = DateTime.MinValue;
        private Canvas? currentCanvas;

        public MainWindow()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Title = $"{Title} v{version.Major}.{version.Minor}.{version.Build}";
            vm = (MainViewModel)DataContext;
        }

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
            vm.SaveSettings();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            vm.LoadSettings();
            #if !DEBUG //Don't check for updates in debug mode
            var local = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            var latest = await GetLatestGitHubVersionAsync();
            if (latest != null && latest > local)
            {
                if (MessageBox.Show(
                    $"A new update is available (v{latest}).\n" +
                    "Do you want to check it out ?",
                    "Update available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                ) == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/Randhomme/TPMapEditor/releases/latest",
                        UseShellExecute = true
                    });
                }
            }
            #endif
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
            double newAbsoluteX = absoluteX * scale;
            double newAbsoluteY = absoluteY * scale;
            double targetX = newAbsoluteX - mousePos.X;
            double targetY = newAbsoluteY - mousePos.Y;
            vm.Zoom = newScale;
            MapScrollViewer.ScrollToHorizontalOffset(targetX);
            MapScrollViewer.ScrollToVerticalOffset(targetY);

            e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Disable default Alt behaviour to allow rotation
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                (RotateCheckBox.IsChecked == true ||
                (WorldObjectPreviewControl.Visibility == Visibility.Visible) ||
                (PlayerPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointSetPreviewControl.Visibility == Visibility.Visible) ||
                (WorldPointPreviewControl.Visibility == Visibility.Visible)))
                e.Handled = true;

            //Keyboard shortcuts
            if (!IsTextInputActive() && vm.TryExecuteKBShortcutCommand(e.Key, Keyboard.Modifiers))
                e.Handled = true;
        }

        //Scroll horizontally by pressing Shift and using MouseWheel
        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                MapScrollViewer.ScrollToHorizontalOffset(MapScrollViewer.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void MapScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            MapScrollViewer.Focus();
        }

        private void MapGridOutsideSelect_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(PreviewCanvas);
            if (selectActionPoint.X < pos.X)
            {
                Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
                SelectionRectangle.Width = pos.X - selectActionPoint.X;
            }
            else
            {
                Canvas.SetLeft(SelectionRectangle, pos.X);
                SelectionRectangle.Width = selectActionPoint.X - pos.X;
            }

            if (selectActionPoint.Y < pos.Y)
            {
                Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
                SelectionRectangle.Height = pos.Y - selectActionPoint.Y;
            }
            else
            {
                Canvas.SetTop(SelectionRectangle, pos.Y);
                SelectionRectangle.Height = selectActionPoint.Y - pos.Y;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HidePlayerElements();
            HideWaypointPathElements();
            HideWorldPolygonElements();
            HideWorldPointSetElements();
            HideObjectivePointElements();
            HideMapTextPointElements();
            WorldObjectRadioButton.IsChecked = true;
        }

        private void RotateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MapGridOutside.MouseWheel += MapGridOutsideRotation_MouseWheel;
        }

        private void RotateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MapGridOutside.MouseWheel -= MapGridOutsideRotation_MouseWheel;
        }

        private void MapGridOutsideRotation_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                var step = GetAcceleratedRotation();
                step = e.Delta > 0 ? step : -step;
                vm.RotateTransformSelection(step);
                e.Handled = true;
            }
        }

        #endregion

        #region UtilsMethods

        /// <summary>
        /// Check for a new release
        /// </summary>
        /// <returns></returns>
        private async Task<Version?> GetLatestGitHubVersionAsync()
        {
            try
            {
                using var http = new HttpClient();

                // GitHub demande obligatoirement un User-Agent
                http.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp");

                var url = "https://api.github.com/repos/Randhomme/TPMapEditor/releases/latest";

                var json = await http.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);

                // Récupère "tag_name" => "v1.4.2"
                var tag = doc.RootElement.GetProperty("tag_name").GetString();

                if (tag == null)
                    return null;

                // Supprime le "v"
                tag = tag.TrimStart('v');

                return Version.TryParse(tag, out var version) ? version : null;
            }
            catch { return null; }
        }

        /// <summary>
        /// Returns a rotation between -180 and 180
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private double GetRotation(double rotation)
        {
            if (rotation > 180) rotation -= 360;
            else if (rotation < -180) rotation += 360;
            return rotation;
        }

        /// <summary>
        /// Allows a smooth rotation using mouse wheel
        /// </summary>
        /// <returns></returns>
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

        private bool IsTextInputActive()
        {
            DependencyObject? current = Keyboard.FocusedElement as DependencyObject;

            if (current is TextBoxBase textBoxBase)
                return textBoxBase.IsSelectionActive;

            return false;
        }

        private T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is T tChild)
                    return tChild;

                T? childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        #endregion

        #region WorldObject

        private void WorldObjectRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldObjectElements();
        }

        private void WorldObjectRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldObjectElements();
        }

        private void ShowWorldObjectElements()
        {
            WorldObjectGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldObjectItemsControl, 1);
            WorldObjectItemsControl.Opacity = 1;
            WorldObjectItemsControl.IsEnabled = true;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldObject();
            MapGridOutside.MouseMove += MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click += DeleteWorldObjectButton_Click;
            currentCanvas = FindVisualChild<Canvas>(WorldObjectItemsControl);
            vm.ActivateWorldObjects();
        }

        private void HideWorldObjectElements()
        {
            WotDataGrid.SelectedItem = null;
            WorldObjectGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldObjectItemsControl, 0);
            WorldObjectItemsControl.Opacity = 0.5;
            WorldObjectItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveWorldObjectCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldObjectCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldObject();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldObjectPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldObjectButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                vm.ClearWorldObjectSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectWorldObjectsInRect(rect);
                e.Handled = true;
            }
            else if (WorldObjectPreviewControl.Visibility != Visibility.Visible)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnWorldObjectClicked(s, e);
            }
        }

        private void OnWorldObjectClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectWorldObject(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldObject()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldObject_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldObject_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldObject_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldObject_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldObject_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteWorldObjectButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedWorldObjectsFromMap();
        }

        private void MapGridOutsideWorldObjectPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(WorldObjectPreviewControl, mousePos.X - WorldObjectPreviewControl.ActualWidth / 2);
            Canvas.SetTop(WorldObjectPreviewControl, mousePos.Y - WorldObjectPreviewControl.ActualHeight / 2);
        }

        private void WorldObjectPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(WorldObjectPreviewControl) + WorldObjectPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WorldObjectPreviewControl) - WorldObjectPreviewControl.ActualHeight / 2;
            var zRotation = WotSliderRotate.Value;
            vm.CreateWorldObject(x, y, 0, zRotation);
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

        private void WorldObjectVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldObjectsVisibility(true);
        }

        private void WorldObjectVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldObjectsVisibility(false);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked += MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMovePlayer();
            MapGridOutside.MouseMove += MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click += DeletePlayerButton_Click;
            currentCanvas = FindVisualChild<Canvas>(PlayerItemsControl);
            vm.ActivatePlayers();
        }

        private void HidePlayerElements()
        {
            WotDataGrid.SelectedItem = null;
            PlayerGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(PlayerItemsControl, 0);
            PlayerItemsControl.Opacity = 0.5;
            PlayerItemsControl.IsEnabled = false;
            AddPlayerCheckBox.IsChecked = false;
            MoveCheckBox.Checked -= MovePlayerCheckBox_Checked;
            MoveCheckBox.Unchecked -= MovePlayerCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMovePlayer();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsidePlayerPreview_MouseMove;
            DeleteButton.Click -= DeletePlayerButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsidePlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                vm.ClearPlayerSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsidePlayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.InverseZoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.InverseZoom)
            {
                vm.SelectPlayersInRect(rect);
                e.Handled = true;
            }
            else if (AddPlayerCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnPlayerClicked(s, e);
            }
        }

        private void OnPlayerClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectPlayer(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMovePlayer()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsidePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsidePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMovePlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMovePlayer_MouseMove;
        }

        private void MapGridOutsideEndMovePlayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMovePlayer_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMovePlayer_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedPlayersFromMap();
        }

        private void MapGridOutsidePlayerPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(PlayerPreviewControl, mousePos.X - PlayerPreviewControl.ActualWidth / 2);
            Canvas.SetTop(PlayerPreviewControl, mousePos.Y - PlayerPreviewControl.ActualHeight / 2);
        }

        private void PlayerPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(PlayerPreviewControl) + PlayerPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(PlayerPreviewControl) - PlayerPreviewControl.ActualHeight / 2;
            var z = 0;
            var rotation = PlayerSliderRotate.Value;
            vm.CreatePlayer(x, y, z, rotation);
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

        private void PlayerVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllPlayersVisibility(true);
        }

        private void PlayerVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllPlayersVisibility(false);
        }

        #endregion

        #region WaypointPath

        private void WaypointPathRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWaypointPathElements();
        }

        private void WaypointPathRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWaypointPathElements();
        }

        private void ShowWaypointPathElements()
        {
            WaypointPathGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WaypointPathItemsControl, 1);
            WaypointPathItemsControl.Opacity = 1;
            WaypointPathItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWaypointPathPointCheckBox_Unchecked;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove += MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click += DeleteWaypointPathPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWaypointPathPoint();
            currentCanvas = FindVisualChild<Canvas>(WaypointPathItemsControl);
            vm.ActivateWaypointPaths();
        }

        private void HideWaypointPathElements()
        {
            WaypointPathGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WaypointPathItemsControl, 0);
            WaypointPathItemsControl.Opacity = 0.5;
            WaypointPathItemsControl.IsEnabled = false;
            NewWaypointPathRadioButton.IsChecked = AddWaypointPathPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWaypointPathPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWaypointPathPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWaypointPathPoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWaypointPathPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWaypointPathPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && NewWaypointPathRadioButton.IsChecked == false && AddWaypointPathPointRadioButton.IsChecked == false)
            {
                vm.ClearWaypointPathSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectWaypointPathPointsInRect(rect);
                e.Handled = true;
            }
            else if (AddWaypointPathPointRadioButton.IsChecked == false && NewWaypointPathRadioButton.IsChecked == false)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null && SelectCheckBox.IsChecked == true)
                {
                    if (s is FrameworkElement element)
                    {
                        bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                        vm.SelectWaypointPathOrWaypointPathPoint(element.DataContext, ctrlPressed);
                    }
                }
            }
        }

        private void OnWaypointPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectWaypointPath(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWaypointPathPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectWaypointPathPoint(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWaypointPathPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWaypointPathPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveWaypointPathPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWaypointPathPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWaypointPathPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWaypointPathPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteWaypointPathPointButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedWaypointPathPointsFromMap();
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

            var x = Canvas.GetLeft(WaypointPathPreviewControl) + WaypointPathPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WaypointPathPreviewControl) - WaypointPathPreviewControl.ActualHeight / 2;
            vm.CreateWaypointPath(x, y);
            AddWaypointPathPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewWaypointPathRadioButton.IsChecked = false;
        }

        private void WaypointPathPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(WaypointPathPointPreviewControl) + WaypointPathPointPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WaypointPathPointPreviewControl) - WaypointPathPointPreviewControl.ActualHeight / 2;
            vm.AddWaypointPathPointToSelectedWaypointPath(x, y);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WaypointPathPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWaypointPathPointRadioButton.IsChecked = false;
        }

        private void WaypointPathVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWaypointPathsVisibility(true);
        }

        private void WaypointPathVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWaypointPathsVisibility(true);
        }

        #endregion

        #region WorldPolygon

        private void WorldPolygonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPolygonElements();
        }

        private void WorldPolygonRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPolygonElements();
        }

        private void ShowWorldPolygonElements()
        {
            WorldPolygonGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPolygonItemsControl, 1);
            WorldPolygonItemsControl.Opacity = 1;
            WorldPolygonItemsControl.IsEnabled = true;
            MoveCheckBox.Checked += MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPolygonPointCheckBox_Unchecked;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove += MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click += DeleteWorldPolygonPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPolygonPoint();
            currentCanvas = FindVisualChild<Canvas>(WorldPolygonItemsControl);
            vm.ActivateWorldPolygons();
        }

        private void HideWorldPolygonElements()
        {
            WorldPolygonGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPolygonItemsControl, 0);
            WorldPolygonItemsControl.Opacity = 0.5;
            WorldPolygonItemsControl.IsEnabled = false;
            NewWorldPolygonRadioButton.IsChecked = AddWorldPolygonPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWorldPolygonPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPolygonPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPolygonPoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPolygonPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPolygonPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && NewWorldPolygonRadioButton.IsChecked == false && AddWorldPolygonPointRadioButton.IsChecked == false)
            {
                vm.ClearWorldPolygonSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectWorldPolygonPointsInRect(rect);
                e.Handled = true;
            }
            else if (AddWorldPolygonPointRadioButton.IsChecked != true && NewWorldPolygonRadioButton.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null && SelectCheckBox.IsChecked == true)
                {
                    if (s is FrameworkElement element)
                    {
                        bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                        vm.SelectWorldPolygonOrWorldPolygonPoint(element.DataContext, ctrlPressed);
                    }
                }
                    
            }
        }

        private void OnWorldPolygonClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                vm.SelectWorldPolygon(element.DataContext, ctrlPressed);
                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
        }

        private void OnWorldPolygonPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                vm.SelectWorldPolygonPoint(element.DataContext, ctrlPressed);
                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPolygonPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPolygonPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPolygonPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPolygonPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldPolygonPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPolygonPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPolygonPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteWorldPolygonPointButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedWorldPolygonPointsFromMap();
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
            var x = Canvas.GetLeft(WorldPolygonPreviewControl) + WorldPolygonPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WorldPolygonPreviewControl) - WorldPolygonPreviewControl.ActualHeight / 2;
            vm.CreateWorldPolygon(x, y);
            AddWorldPolygonPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewWorldPolygonRadioButton.IsChecked = false;
        }

        private void WorldPolygonPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(WorldPolygonPointPreviewControl) + WorldPolygonPointPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WorldPolygonPointPreviewControl) - WorldPolygonPointPreviewControl.ActualHeight / 2;
            vm.AddWorldPolygonPointToSelectedWorldPolygon(x, y);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPolygonPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPolygonPointRadioButton.IsChecked = false;
        }

        private void WorldPolygonVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldPolygonsVisibility(true);
        }

        private void WorldPolygonVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldPolygonsVisibility(false);
        }

        #endregion

        #region WorldPointSet

        private void WorldPointRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowWorldPointSetElements();
        }

        private void WorldPointRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            HideWorldPointSetElements();
        }

        private void ShowWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = new GridLength(1, GridUnitType.Star);
            Panel.SetZIndex(WorldPointSetItemsControl, 1);
            WorldPointSetItemsControl.Opacity = 1;
            WorldPointSetItemsControl.IsEnabled = true;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveWorldPointCheckBox_Unchecked;
            DeleteButton.Click += DeleteWorldPointButton_Click;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveWorldPoint();
            MapGridOutside.MouseMove += MapGridOutsideWorldPointPreview_MouseMove;
            currentCanvas = FindVisualChild<Canvas>(WorldPointSetItemsControl);
            vm.ActivateWorldPointSets();
        }

        private void HideWorldPointSetElements()
        {
            WorldPointSetGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(WorldPointSetItemsControl, 0);
            WorldPointSetItemsControl.Opacity = 0.5;
            WorldPointSetItemsControl.IsEnabled = false;
            AddWorldPointSetRadioButton.IsChecked = AddWorldPointSetPointRadioButton.IsChecked = false;
            MoveCheckBox.Checked -= MoveWorldPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveWorldPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveWorldPoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideWorldPointPreview_MouseMove;
            DeleteButton.Click -= DeleteWorldPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && AddWorldPointSetRadioButton.IsChecked == false && AddWorldPointSetPointRadioButton.IsChecked == false)
            {
                vm.ClearWorldPointSetSelection();
            }
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectWorldPointsInRect(rect);
                e.Handled = true;
            }
            else if (AddWorldPointSetPointRadioButton.IsChecked != true && AddWorldPointSetRadioButton.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnWorldPointClicked(s, e);
            }
        }

        private void OnWorldPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                vm.SelectWorldPoint(element.DataContext, ctrlPressed);
                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveWorldPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveWorldPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveWorldPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveWorldPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveWorldPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveWorldPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteWorldPointButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedWorldPointsFromMap();
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
            var x = Canvas.GetLeft(WorldPointSetPreviewControl) + WorldPointSetPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WorldPointSetPreviewControl) - WorldPointSetPreviewControl.ActualHeight / 2;
            var zRotation = WorldPointSliderRotate.Value;
            vm.CreateWorldPointSet(x, y, 0, zRotation);
            AddWorldPointSetPointRadioButton.IsChecked = true;
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void WorldPointSetPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddWorldPointSetRadioButton.IsChecked = false;
        }

        private void WorldPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(WorldPointPreviewControl) + WorldPointPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(WorldPointPreviewControl) - WorldPointPreviewControl.ActualHeight / 2;
            var zRotation = WorldPointSliderRotate.Value;
            vm.AddWorldPointToSelectedWorldPointSet(x, y, 0, zRotation);
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

        private void WorldPointSetVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldPointSetsVisibility(true);
        }

        private void WorldPointSetVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllWorldPointSetsVisibility(false);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveObjectivePoint();
            MapGridOutside.MouseMove += MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click += DeleteObjectivePointButton_Click;
            currentCanvas = FindVisualChild<Canvas>(ObjectivePointItemsControl);
            vm.ActivateObjectivePoints();
        }

        private void HideObjectivePointElements()
        {
            WotDataGrid.SelectedItem = null;
            ObjectivePointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(ObjectivePointItemsControl, 0);
            ObjectivePointItemsControl.Opacity = 0.5;
            ObjectivePointItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveObjectivePointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveObjectivePointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveObjectivePoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideObjectivePointPreview_MouseMove;
            DeleteButton.Click -= DeleteObjectivePointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                vm.ClearObjectivePointSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectObjectivePointsInRect(rect);
                e.Handled = true;
            }
            else if(AddObjectivePointCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnObjectivePointClicked(s, e);
            }
        }

        private void OnObjectivePointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectObjectivePoint(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveObjectivePoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveObjectivePoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveObjectivePoint_MouseMove;
        }

        private void MapGridOutsideEndMoveObjectivePoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveObjectivePoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveObjectivePoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteObjectivePointButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedObjectivePointsFromMap();
        }

        private void MapGridOutsideObjectivePointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(ObjectivePointPreviewControl, mousePos.X - ObjectivePointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(ObjectivePointPreviewControl, mousePos.Y - ObjectivePointPreviewControl.ActualHeight / 2);
        }

        private void ObjectivePointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(ObjectivePointPreviewControl) + ObjectivePointPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(ObjectivePointPreviewControl) - ObjectivePointPreviewControl.ActualHeight / 2;
            vm.CreateObjectivePoint(x, y);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void ObjectivePointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddObjectivePointCheckBox.IsChecked = false;
        }

        private void ObjectivePointVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllObjectivePointsVisibility(true);
        }

        private void ObjectivePointVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllObjectivePointsVisibility(false);
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
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MoveCheckBox.Checked += MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked += MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                EnableMoveMapTextPoint();
            MapGridOutside.MouseMove += MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click += DeleteMapTextPointButton_Click;
            currentCanvas = FindVisualChild<Canvas>(MapTextPointItemsControl);
            vm.ActivateMapTextPoints();
        }

        private void HideMapTextPointElements()
        {
            WotDataGrid.SelectedItem = null;
            MapTextPointGridRow.Height = GridLength.Auto;
            Panel.SetZIndex(MapTextPointItemsControl, 0);
            MapTextPointItemsControl.Opacity = 0.5;
            MapTextPointItemsControl.IsEnabled = false;
            MoveCheckBox.Checked -= MoveMapTextPointCheckBox_Checked;
            MoveCheckBox.Unchecked -= MoveMapTextPointCheckBox_Unchecked;
            if (MoveCheckBox.IsChecked == true)
                DisableMoveMapTextPoint();
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.MouseMove -= MapGridOutsideMapTextPointPreview_MouseMove;
            DeleteButton.Click -= DeleteMapTextPointButton_Click;
            currentCanvas = null;
        }

        private void MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                vm.ClearMapTextPointSelection();
            selectActionPoint = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(SelectionRectangle, selectActionPoint.X);
            Canvas.SetTop(SelectionRectangle, selectActionPoint.Y);
            MapGridOutside.MouseMove += MapGridOutsideSelect_MouseMove;
            e.Handled = true;
        }

        private void MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideSelect_MouseMove;
            Mouse.Capture(null);
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
            var pos = e.GetPosition(PreviewCanvas);
            var rect = new Rect(selectActionPoint, pos);
            if (rect.Width >= SystemParameters.MinimumHorizontalDragDistance / vm.Zoom || rect.Height >= SystemParameters.MinimumVerticalDragDistance / vm.Zoom)
            {
                vm.SelectMapTextPointsInRect(rect);
                e.Handled = true;
            }
            else if (AddMapTextPointCheckBox.IsChecked != true)
            {
                var s = currentCanvas?.InputHitTest(pos);
                if (s != null)
                    OnObjectivePointClicked(s, e);
            }
        }

        private void OnMapTextPointClicked(object sender, MouseButtonEventArgs e)
        {
            if (SelectCheckBox.IsChecked == true && sender is FrameworkElement element)
            {
                bool ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                vm.SelectMapTextPoint(element.DataContext, ctrlPressed);

                if (MoveCheckBox.IsChecked == false)
                    e.Handled = true;
            }
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
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.SizeAll;
        }

        private void DisableMoveMapTextPoint()
        {
            MapGridOutside.PreviewMouseLeftButtonDown += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp += MapGridOutsideMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.PreviewMouseLeftButtonDown -= MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown;
            MapGridOutside.PreviewMouseLeftButtonUp -= MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp;
            MapGridOutside.Cursor = Cursors.Arrow;
        }

        private void MapGridOutsideBeginMoveMapTextPoint_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveActionPoint = e.GetPosition(MapGridInside);
            vm.InitTranslateTransformCommand();
            MapGridOutside.MouseMove += MapGridOutsideMoveMapTextPoint_MouseMove;
        }

        private void MapGridOutsideEndMoveMapTextPoint_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapGridOutside.MouseMove -= MapGridOutsideMoveMapTextPoint_MouseMove;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void MapGridOutsideMoveMapTextPoint_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(MapGridOutside);
            var pos = e.GetPosition(MapGridInside);
            var x = pos.X - moveActionPoint.X;
            var y = moveActionPoint.Y - pos.Y; // y position is upside down
            vm.TranslateTransformSelection(x, y);
            moveActionPoint = pos;
        }

        private void DeleteMapTextPointButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveSelectedMapTextPointsFromMap();
        }

        private void MapGridOutsideMapTextPointPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(PreviewCanvas);
            Canvas.SetLeft(MapTextPointPreviewControl, mousePos.X - MapTextPointPreviewControl.ActualWidth / 2);
            Canvas.SetTop(MapTextPointPreviewControl, mousePos.Y - MapTextPointPreviewControl.ActualHeight / 2);
        }

        private void MapTextPointPreviewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = Canvas.GetLeft(MapTextPointPreviewControl) + MapTextPointPreviewControl.ActualWidth / 2;
            var y = -Canvas.GetTop(MapTextPointPreviewControl) - MapTextPointPreviewControl.ActualHeight / 2;
            vm.CreateMapTextPoint(x, y);
            e.Handled = true; // to not trigger the mapGrid MouseLeftButtonDown event
        }

        private void MapTextPointPreviewControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddMapTextPointCheckBox.IsChecked = false;
        }

        private void MapTextPointVisibilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllMapTextPointsVisibility(true);
        }

        private void MapTextPointVisibilityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            vm?.SetAllMapTextPointsVisibility(false);
        }

        #endregion
    }
}
