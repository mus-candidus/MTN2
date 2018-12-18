using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public class MapFile {
        public string Location { get; set; }
        public string FileName { get; set; }
        public string MapType { get; set; } = "GameLocation";
        public string DisplayName { get; set; } = "Untitled";

        public MapFile(string Location, string FileName) {
            this.Location = Location;
            this.FileName = FileName;
            MapType = "GameLocation";
            DisplayName = "Untitled";
        }

        public MapFile(string Location, string FileName, string MapType, string DisplayName) {
            this.Location = Location;
            this.FileName = FileName;
            this.MapType = MapType;
            this.DisplayName = DisplayName;
        }
    }
}
