using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class JournalEntry : SelectableMapObject
    {
        [ObservableProperty]
        [property: Required]
        private string textStringId, speechEventFileName, pictureTexture;

        public JournalEntry(WorldMap map, string textStringId, string speechEventFileName, string pictureTexture) : base(map)
        {
            this.textStringId = textStringId;
            this.speechEventFileName = speechEventFileName;
            this.pictureTexture = pictureTexture;
        }
    }
}
