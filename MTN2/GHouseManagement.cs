using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    internal class GHouseManagement {
        private readonly FarmManagement farmManagement;
        public List<CustomGreenHouse> GreenHouseList { get; private set; }

        public int GreenHouseEntryX {
            get {
                if (Canon || LoadedFarm.CustomGreenhouse == null) return 10;
                return LoadedFarm.CustomGreenhouse.Enterance.PointOfInteraction.X;
            }
        }

        public int GreenHouseEntryY {
            get {
                if (Canon || LoadedFarm.CustomGreenhouse == null) return 23;
                return LoadedFarm.CustomGreenhouse.Enterance.PointOfInteraction.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="farmManagement"></param>
        public GHouseManagement(FarmManagement farmManagement) {
            this.farmManagement = farmManagement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="monitor"></param>
        public void Populate(IContentPack contentPack, IMonitor monitor) {
            CustomGreenHouse GreenHouseData = new CustomGreenHouse();

            if (ProcessContentPack(contentPack, out GreenHouseData)) {
                monitor.Log($"\t + Contains a custom greenhouse.", LogLevel.Trace);
                //Version control?
                //Validate?
                GreenHouseData.ContentPack = contentPack;
                GreenHouseList.Add(GreenHouseData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customFarm"></param>
        public void LinkToFarm(CustomFarm customFarm) {
            if (customFarm.StartingGreenHouse == null) return;
            for (int i = 0; i < GreenHouseList.Count; i++) {
                if (GreenHouseList[i].Name == customFarm.StartingGreenHouse) {
                    customFarm.CustomGreenhouse = GreenHouseList[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentPack"></param>
        /// <param name="greenHouse"></param>
        /// <returns></returns>
        private bool ProcessContentPack(IContentPack contentPack, out CustomGreenHouse greenHouse) {
            Dictionary<string, object> Extra;
            if (contentPack.Manifest.ExtraFields != null && contentPack.Manifest.ExtraFields.ContainsKey("ContentPackType")) {
                Extra = (Dictionary<string, object>)ObjectToDictionaryHelper.ToDictionary(contentPack.Manifest.ExtraFields["ContentPackType"]);
                if (Extra.ContainsKey("Greenhouse") && bool.Parse(Extra["Greenhouse"].ToString())) {
                    greenHouse = contentPack.ReadJsonFile<CustomGreenHouse>("greenHouseType.json");
                    return true;
                }
            }
            greenHouse = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GreenHouseCoords() {
            if (Canon || LoadedFarm.GreenHouse == null) {
                return GreenHouseCoordsCanon();
            }
            Placement? Coordinates = LoadedFarm.GreenHouse.Coordinates;
            return new Vector2(Coordinates.Value.X * 64f, Coordinates.Value.Y * 64f);
        }

        protected Vector2 GreenHouseCoordsCanon() {
            return new Vector2(1600f, 384f);
        }

        public float GreenHouseLayerDepth() {
            if (Canon) {
                return 0.0704f;
            } else {
                return ((LoadedFarm.GreenHouse.PointOfInteraction.Y - 7 + 2) * 64f) / 10000f;
            }
        }
    }
}
