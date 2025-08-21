using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public class DefaultElement : ObservableObject
    {
        public Action? Remove { get; set; }
        public Action? Locate { get; set; }
    }
}
