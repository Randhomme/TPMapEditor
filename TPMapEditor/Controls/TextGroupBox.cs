using System.Windows;
using System.Windows.Controls;

namespace TPMapEditor.Controls
{
    /// <summary>
    /// A text box with a header label. It should be used with the corresponding style to actually see the header.
    /// </summary>
    public partial class TextGroupBox : TextBox
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(TextGroupBox), new PropertyMetadata(""));

    }
}
