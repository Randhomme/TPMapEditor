using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using TPMapEditor.Interfaces.Implementations;

namespace TPMapEditor.Data
{
    public partial class PlayerAlliance : SelectableMapObject
    {
        [ObservableProperty]
        [property: Required]
        private Player player1, player2;

        public IEnumerable<Player> SelectablePlayers { get; }

        public PlayerAlliance(WorldMap map, IEnumerable<Player> selectablePlayers, Player player1, Player player2) : base(map)
        {
            this.SelectablePlayers = selectablePlayers;
            this.player1 = player1;
            this.player2 = player2;
        }
    }
}
