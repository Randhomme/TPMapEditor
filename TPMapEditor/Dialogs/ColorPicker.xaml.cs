using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : DialogWindow
    {
        private bool mouseDownedOnColorCanvas = false;
        private bool mouseDownedOnHueSlider = false;
        private short alpha;
        [ObservableProperty]
        private Color newColor;
        public Color CurrentColor { get; set; }
        public ColorPicker(Window owner, string title, Color color, short alpha = -1) : base(owner, title)
        {
            this.alpha = alpha;
            CurrentColor = NewColor = color;
            InitializeComponent();
            if (alpha != -1)
                aGroupBox.Visibility = Visibility.Collapsed;
        }

        private void hueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hexColorTextBox.TextChanged -= hexColorTextBox_TextChanged;
            DisableSlidersEvent();
            changeHueColorFromSlider(e.NewValue);
            GetSelectedColor(Canvas.GetLeft(colorHandle) + 5, Canvas.GetTop(colorHandle) + 5);
            EnableSlidersEvent();
            hexColorTextBox.TextChanged += hexColorTextBox_TextChanged;
        }

        private void colorCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownedOnColorCanvas = true;
            var pos = e.GetPosition(colorCanvas);
            Mouse.Capture(colorCanvas);
            colorCanvas.MouseMove += colorCanvas_MouseMove;
            DisableSlidersEvent();
            hexColorTextBox.TextChanged -= hexColorTextBox_TextChanged;
            GetSelectedColor(pos.X, pos.Y);
        }

        private void colorCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(mouseDownedOnColorCanvas)
            {
                Mouse.Capture(null);
                colorCanvas.MouseMove -= colorCanvas_MouseMove;
                EnableSlidersEvent();
                hexColorTextBox.TextChanged += hexColorTextBox_TextChanged;
                mouseDownedOnColorCanvas = false;
            }
        }

        private void colorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(colorCanvas);
            GetSelectedColor(pos.X, pos.Y);
        }

        private void EnableSlidersEvent()
        {
            rSlider.ValueChanged += rgbaSlider_ValueChanged;
            gSlider.ValueChanged += rgbaSlider_ValueChanged;
            bSlider.ValueChanged += rgbaSlider_ValueChanged;
            aSlider.ValueChanged += rgbaSlider_ValueChanged;
        }

        private void DisableSlidersEvent()
        {
            rSlider.ValueChanged -= rgbaSlider_ValueChanged;
            gSlider.ValueChanged -= rgbaSlider_ValueChanged;
            bSlider.ValueChanged -= rgbaSlider_ValueChanged;
            aSlider.ValueChanged -= rgbaSlider_ValueChanged;
        }

        private void rgbaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NewColor = Color.FromArgb((byte)aSlider.Value, (byte)rSlider.Value, (byte)gSlider.Value, (byte)bSlider.Value);
            changeHueSliderAndCanvasFromColor();
            hexColorTextBox.TextChanged -= hexColorTextBox_TextChanged;
            hexColorTextBox.Text = NewColor.ToString();
            hexColorTextBox.TextChanged += hexColorTextBox_TextChanged;
        }

        private void changeHueSliderAndCanvasFromColor()
        {
            var max = Math.Max(NewColor.R, Math.Max(NewColor.G, NewColor.B));
            var min = Math.Min(NewColor.R, Math.Min(NewColor.G, NewColor.B));
            var delta = (double)max - min;
            var l = max / 255.0;
            var s = 0.0;

            //we change hueSlider if max != min
            if (max != min)
            {
                hueSlider.ValueChanged -= hueSlider_ValueChanged;
                var hue = 0.0;
                //l == 0 if black color, meaning max == min, which is by definition not the case here
                min = (byte)(min / l);
                s = -min / 255.0 + 1;
                if (NewColor.R == max)
                {
                    hue = (NewColor.G - NewColor.B) / delta;
                }
                else if (NewColor.G == max)
                {
                    hue = 2 + (NewColor.B - NewColor.R) / delta;
                }
                else
                {
                    hue = 4 + (NewColor.R - NewColor.G) / delta;
                }
                hue *= 255;
                if (hue < 0) hue += 1530;
                hueSlider.Value = hue;
                changeHueColorFromSlider(hue);
                hueSlider.ValueChanged += hueSlider_ValueChanged;
            }
            Canvas.SetLeft(colorHandle, s * colorCanvas.ActualWidth - 5);
            Canvas.SetTop(colorHandle, (-l + 1) * colorCanvas.ActualHeight - 5);
        }

        private void changeHueColorFromSlider(double value)
        {
            if (value < 256)
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, 255, (byte)value, 0);
            }
            else if (value < 511)
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, (byte)(255 - (value - 255)), 255, 0);
            }
            else if (value < 766)
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, 0, 255, (byte)(value - 510));
            }
            else if (value < 1021)
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, 0, (byte)(255 - (value - 765)), 255);
            }
            else if (value < 1276)
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, (byte)(value - 1020), 0, 255);
            }
            else
            {
                Resources["SelectedHueColor"] = Color.FromArgb(255, 255, 0, (byte)(255 - (value - 1275)));
            }
        }

        private void GetSelectedColor(double px, double py)
        {
            var x = Math.Max(0, Math.Min(px, colorCanvas.ActualWidth));
            var y = Math.Max(0, Math.Min(py, colorCanvas.ActualHeight));
            Canvas.SetLeft(colorHandle, x - 5);
            Canvas.SetTop(colorHandle, y - 5);
            var hueColor = (Color)Resources["SelectedHueColor"];
            var s = x / colorCanvas.ActualWidth;
            var l = -y / colorCanvas.ActualHeight + 1;
            NewColor = Color.FromArgb((byte)aSlider.Value, (byte)((255 - s * (255 - hueColor.R)) * l), (byte)((255 - s * (255 - hueColor.G)) * l), (byte)((255 - s * (255 - hueColor.B)) * l));
            rSlider.Value = NewColor.R;
            gSlider.Value = NewColor.G;
            bSlider.Value = NewColor.B;
            aSlider.Value = NewColor.A;
            hexColorTextBox.Text = NewColor.ToString();
        }

        private void myWindow_Loaded(object sender, RoutedEventArgs e)
        {
            aSlider.Value = alpha == -1 ? CurrentColor.A : alpha;
            rSlider.Value = CurrentColor.R;
            gSlider.Value = CurrentColor.G;
            bSlider.Value = CurrentColor.B;
            hexColorTextBox.Text = CurrentColor.ToString();
        }

        private void colorCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var x = Canvas.GetLeft(colorHandle) + 5;
            var y = Canvas.GetTop(colorHandle) + 5;
            Canvas.SetLeft(colorHandle, x * e.NewSize.Width / e.PreviousSize.Width - 5);
            Canvas.SetTop(colorHandle, y * e.NewSize.Height / e.PreviousSize.Height - 5);
        }

        private void hueSlider_MouseMove(object sender, MouseEventArgs e)
        {
            var slider = (Slider)sender;
            Point position = e.GetPosition(slider);
            double d = 1.0d / slider.ActualWidth * position.X;
            var p = slider.Maximum * d;
            slider.Value = p;
        }

        private void hueSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownedOnHueSlider = true;
            Mouse.Capture(hueSlider);
            hueSlider.MouseMove += hueSlider_MouseMove;
        }

        private void hueSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDownedOnHueSlider)
            {
                Mouse.Capture(null);
                hueSlider.MouseMove -= hueSlider_MouseMove;
                mouseDownedOnHueSlider = false;
            }
        }

        private void hexColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                NewColor = (Color)ColorConverter.ConvertFromString(hexColorTextBox.Text);
                DisableSlidersEvent();
                rSlider.Value = NewColor.R;
                gSlider.Value = NewColor.G;
                bSlider.Value = NewColor.B;
                aSlider.Value = NewColor.A;
                changeHueSliderAndCanvasFromColor();
                EnableSlidersEvent();
                if (alpha != -1)
                    aSlider.Value = alpha;
            }
            catch { }
        }
    }
}
