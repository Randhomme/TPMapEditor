using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Settings;

namespace TPMapEditor.Data
{
    public partial class JournalEntry : ObservableObject
    {
        [ObservableProperty]
        private string textStringId, speechEventFileName, pictureTexture;

        public JournalEntry(string textStringId, string speechEventFileName, string pictureTexture)
        {
            this.textStringId = textStringId;
            this.speechEventFileName = speechEventFileName;
            this.pictureTexture = pictureTexture;
        }
    }
}
