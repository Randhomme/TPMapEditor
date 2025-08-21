using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TPMapEditor.Data
{
    public partial class PlayerAlliance : ObservableObject
    {
        [ObservableProperty]
        private Player player1, player2;

        public PlayerAlliance(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
    }
}
