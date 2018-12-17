using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class CustomFarm {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string Icon { get; set; }
        public float Version { get; set; }

        [JsonIgnore]
        public Texture2D IconSource { get; set; }
        [JsonIgnore]
        public IContentPack ContentPack { get; set; }

        //Multiplayer
        public int CabinCapacity { get; set; } = 3;
        public bool AllowClose { get; set; } = true;
        public bool AllowSeperate { get; set; } = true;
    }
}
