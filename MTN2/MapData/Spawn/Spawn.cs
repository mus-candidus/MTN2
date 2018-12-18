using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData {
    public class Spawn {
        public string ItemName { get; set; }
        public SpawningSeason Seasons { get; set; }
        public SpawnType Boundary { get; set; }

        public Area AreaBinding { get; set; }
        public int TileBindiing { get; set; }

        public float Chance { get; set; } = 0.70f;

        public Modifier RainModifier { get; set; }
        public Modifier NewMonthModifier { get; set; }
        public Modifier NewYearModifier { get; set; }

        public int AmountMin { get; set; }
        public int AmountMax { get; set; }

        public int CooldownMin { get; set; }
        public int CooldownMax { get; set; }
        public int DaysLeft { get; set; }
    }
}
