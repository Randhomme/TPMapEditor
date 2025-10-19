using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Data
{
    public partial class JournalEntry : NamedElement
    {
        [ObservableProperty]
        private string textStringId, speechEventFileName, pictureTexture;

        public JournalEntry(WorldMap map, string name, string textStringId, string speechEventFileName, string pictureTexture) : base(map, name)
        {
        }

        protected override bool IsNameTaken(string name)
        {
            foreach (var item in map.JournalEntries)
            {
                if (item.Name == name && item != this)
                    return true;
            }
            return false;
        }
    }
}
