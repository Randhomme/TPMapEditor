using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TPMapEditor.Controls
{
    public class SplitToggleButton : ToggleButton
    {
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

        private bool _menuWasOpen;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_DropDownButton") is Button dropButton)
            {
                dropButton.Click += (s, e) =>
                {
                    DropDownMenu.PlacementTarget = this;
                    DropDownMenu.Placement = PlacementMode.Bottom;
                    DropDownMenu.IsOpen = !_menuWasOpen;
                };

                if (DropDownMenu != null)
                {
                    DropDownMenu.Opened += (s, e) =>
                    {
                        _menuWasOpen = true;
                    };
                    DropDownMenu.Closed += (s, e) =>
                    {
                        _menuWasOpen = false;
                    };
                }
            }
        }
    }
}
