using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace TPMapEditor.Data
{
    public partial class GameHeadersFile : ObservableObject
    {
        public static IList<string> GameHeadersFilesList { get; } = new List<string>();

        [ObservableProperty]
        private string fileName;

        public GameHeadersFile()
        {
            fileName = GameHeadersFilesList.FirstOrDefault() ?? string.Empty;
        }

        public GameHeadersFile(string fileName)
        {
            this.fileName = fileName;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
