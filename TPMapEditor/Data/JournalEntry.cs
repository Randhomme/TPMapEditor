using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPMapEditor.Settings;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public partial class JournalEntry : CustomObservableValidator
    {
        [ObservableProperty]
        [property: Required]
        private string textStringId, speechEventFileName, pictureTexture;

        public JournalEntry(string textStringId, string speechEventFileName, string pictureTexture)
        {
            this.textStringId = textStringId;
            this.speechEventFileName = speechEventFileName;
            this.pictureTexture = pictureTexture;
        }
    }
}
