using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MTN2.MapData {
    public abstract class Resource {
        private GameLocation Map;
        protected int Width = 2;
        protected int Height = 2;

        public List<Spawn> ResourceList { get; set; }
        public string MapName { get; set; }
        
        public Resource(string MapName) {
            this.MapName = MapName;
            ResourceList = new List<Spawn>();
        }

        public virtual void Initalize() {
            Map = Game1.getLocationFromName(MapName);
        }

        public virtual void SetAmount() {

        }

        public virtual void SetCooldown() {

        }

        public virtual void SpawnItem(int Attempts) {

        }

    }
}
