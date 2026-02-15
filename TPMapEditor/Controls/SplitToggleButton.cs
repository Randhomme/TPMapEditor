using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TPMapEditor.Controls
{
    public class SplitToggleButton : ToggleButton
    {
        private bool _menuWasOpen;

        public static readonly DependencyProperty DropDownMenuProperty =
        DependencyProperty.Register(
            nameof(DropDownMenu),
            typeof(ContextMenu),
            typeof(SplitToggleButton),
            new PropertyMetadata(null));

        public ContextMenu DropDownMenu
        {
            get => (ContextMenu)GetValue(DropDownMenuProperty);
            set => SetValue(DropDownMenuProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_DropDownButton") is Button dropButton)
            {
                dropButton.PreviewMouseLeftButtonDown -= DropButton_PreviewMouseLeftButtonDown;
                dropButton.PreviewMouseLeftButtonDown += DropButton_PreviewMouseLeftButtonDown;

                if (DropDownMenu != null)
                {
                    DropDownMenu.Opened -= DropDownMenu_Opened;
                    DropDownMenu.Closed -= DropDownMenu_Closed;
                    DropDownMenu.Opened += DropDownMenu_Opened;
                    DropDownMenu.Closed += DropDownMenu_Closed;
                }
            }
        }

        private void DropButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            DropDownMenu.PlacementTarget = this;
            DropDownMenu.Placement = PlacementMode.Bottom;
            DropDownMenu.IsOpen = !_menuWasOpen;
        }

        private void DropDownMenu_Opened(object sender, RoutedEventArgs e)
        {
            _menuWasOpen = true;
        }
        private void DropDownMenu_Closed(object sender, RoutedEventArgs e)
        {
            _menuWasOpen = false;
        }
    }
}
