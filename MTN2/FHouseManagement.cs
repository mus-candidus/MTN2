using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    internal class FHouseManagement {
        private readonly FarmManagement farmManagement;

        public int FurnitureLayout {
            get {
                return LoadedFarm.FurnitureLayoutFromCanon;
            }
        }

        public FHouseManagement(FarmManagement farmManagement) {
            this.farmManagement = farmManagement;
        }
    }
}
