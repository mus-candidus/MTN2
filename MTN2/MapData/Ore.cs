using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MTN2.MapData {
    public class Ore : Resource {
        private int Width = 1;
        private int Height = 1;

        public Ore() : base("Farm") { }

        public Ore(string MapName) : base(MapName) { }

        public override void AddAtPoint(Vector2 Point, Spawn SItem) {
            throw new NotImplementedException();
        }

        public override void SpawnAll(int Attempts) {
            throw new NotImplementedException();
        }

        public override void SpawnItem(int Attempts, int index) {
            throw new NotImplementedException();
        }
    }
}
