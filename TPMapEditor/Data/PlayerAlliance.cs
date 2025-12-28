using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Utils;

namespace TPMapEditor.Data
{
    public partial class PlayerAlliance : CustomObservableValidator
    {
        [ObservableProperty]
        [property: Required]
        private Player player1, player2;

        public PlayerAlliance(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
    }
}
