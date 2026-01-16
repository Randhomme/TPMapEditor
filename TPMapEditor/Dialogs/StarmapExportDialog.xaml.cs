using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TPMapEditor.ViewModel;

namespace TPMapEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for StarmapExportWindow.xaml
    /// </summary>
    public partial class StarmapExportDialog : DialogWindow
    {
        public StarmapExportDialog(Window owner, string title) : base(owner, title)
        {
            InitializeComponent();
        }
    }
}
