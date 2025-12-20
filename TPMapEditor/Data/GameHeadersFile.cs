using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
