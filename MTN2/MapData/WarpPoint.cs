using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public struct WarpPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public WarpPoint(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }
    }
}
