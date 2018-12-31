using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MTN2.MapData;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace MTN2
{
    /// <summary>
    /// CustomFarmManager class. Performs all the nessecary operations needed to allow a custom farm (or custom farms)
    /// to properly function. It is also built to distinguish between canon farms, or custom farms. Contains a List<T>
    /// of CustomFarm classes and manipulates / gathers the data accordingly.
    /// </summary>
    public class CustomFarmManager {
        protected int LoadedIndex = 0;
        protected int SelectedIndex = 0;
        public List<CustomFarm> FarmList { get; private set; }
        public bool NoDebris { get; set; } = false;
        public bool Canon { get; private set; } = true;

        /// <summary>
        /// Gets the custom farm that the player current has selected
        /// for a new game. Used primarily when the player is creating
        /// said new game.
        /// </summary>
        public CustomFarm SelectedFarm {
            get {
                return FarmList[SelectedIndex];
            }
        }
        /// <summary>
        /// Gets the custom farm that is currently loaded/being played.
        /// </summary>
        public CustomFarm LoadedFarm {
            get {
                return FarmList[LoadedIndex];
            }
        }
        /// <summary>
        /// Gets the coordinates where the player can interact with the 
        /// starting shipping bin.
        /// </summary>
        public Interaction ShippingBinPoints {
            get {
                return LoadedFarm.ShippingBin.PointOfInteraction;
            }
        }

        public Interaction RabbitShrine {
            get {
                return LoadedFarm.RabbitShrine.PointOfInteraction;
            }
        }

        public Point FarmHousePorch {
            get {
                return new Point(LoadedFarm.FarmHouse.PointOfInteraction.X, LoadedFarm.FarmHouse.PointOfInteraction.Y);
            }
        }

        public int FurnitureLayout {
            get {
                return LoadedFarm.FurnitureLayoutFromCanon;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomFarmManager() {
            FarmList = new List<CustomFarm>();
        }

        /// <summary>
        /// Populates the List<CustomFarm> FarmList variable with all the Content Packs
        /// registered to MTN.
        /// </summary>
        /// <param name="Helper">SMAPI's IModHelper, to load in the Content Packs</param>
        /// <param name="Monitor">SMAPI's IMonitor, to print useful information</param>
        public void Populate(IModHelper Helper, IMonitor Monitor) {
            CustomFarm FarmData;
            string IconFile;

            FarmList = new List<CustomFarm>();

            foreach (IContentPack ContentPack in Helper.ContentPacks.GetOwned()) {
                Monitor.Log($"Reading content pack: {ContentPack.Manifest.Name} {ContentPack.Manifest.Version}.");
                FarmData = ContentPack.ReadJsonFile<CustomFarm>("farmType.json");
                
                IconFile = Path.Combine(ContentPack.DirectoryPath, "icon.png");
                if(File.Exists(IconFile)) {
                    FarmData.IconSource = ContentPack.LoadAsset<Texture2D>("icon.png");
                } else {
                    FarmData.IconSource = Helper.Content.Load<Texture2D>(Path.Combine("res", "missingIcon.png"));
                }

                FarmData.ContentPack = ContentPack;
                FarmList.Add(FarmData);
            }

            return;
        }

        /// <summary>
        /// Updates the selected farm. Used during the creation of a new game.
        /// </summary>
        /// <param name="farmName"></param>
        public void UpdateSelectedFarm(string farmName) {
            if (!farmName.StartsWith("MTN_")) {
                Canon = true;
                SelectedIndex = 0;
                return;
            }
            farmName = farmName.Substring(3);
            for (int i = 0; i < FarmList.Count; i++) {
                if (FarmList[i].Name == farmName) {
                    SelectedIndex = i;
                    return;
                }
            }
            SelectedIndex = 0;
            return;
        }

        /// <summary>
        /// Set the Selected Farm as the Loaded Farm. Used when creating a new game,
        /// as a confirmation.
        /// </summary>
        public void LoadCustomFarm() {
            LoadedIndex = SelectedIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whichFarm"></param>
        public void LoadCustomFarm(int whichFarm) {
            if (whichFarm < 5) {
                NoDebris = true;
            }
        }
        
        /// <summary>
        /// Gets the Asset Key of the Base Farm Map. Only does work on the base
        /// farm map of any custom map.
        /// </summary>
        /// <param name="map">The base farm map, loaded.</param>
        /// <returns>The Actual Asset Key</returns>
        public string GetAssetKey(out Map map) {
            if (LoadedFarm.FarmMap.FileType == FileType.raw) {
                map = LoadedFarm.ContentPack.LoadAsset<Map>(LoadedFarm.FarmMap.FileName + ".tbin");
            } else {
                map = null;
            }
            return LoadedFarm.ContentPack.GetActualAssetKey(LoadedFarm.FarmMap.FileName + ((LoadedFarm.FarmMap.FileType == FileType.raw) ? ".tbin" : ".xnb"));
        }

        /// <summary>
        /// Gets the Asset Key of additional maps. Only does work on additional maps,
        /// and not the base farm map of any custom map.
        /// </summary>
        /// <param name="fileName">The filename of the map</param>
        /// <param name="fileType">The file type (xnb or tbin)</param>
        /// <returns>The Actual Asset Key</returns>
        public string GetAssetKey(string fileName, FileType fileType) {
            return LoadedFarm.ContentPack.GetActualAssetKey(fileName + ((fileType == FileType.raw) ? ".tbin" : ".xnb"));
        }

        /// <summary>
        /// Loads a map, based on the given filename. Only does work on additional maps.
        /// Should not be used for base farm map.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The custom map, loaded into memory.</returns>
        public Map LoadMap(string fileName) {
            return LoadedFarm.ContentPack.LoadAsset<Map>(fileName);
        }

        /// <summary>
        /// Gets the X and Y coordinates of the top left point of the Farmhouse in Vector2
        /// Form. Allows offsetting the coordinates. 
        /// </summary>
        /// <param name="OffsetX">The Offset value for the X Coordinate</param>
        /// <param name="OffsetY">The Offset value for the Y Coordinate</param>
        /// <returns>The coordinates in Vector2 form.</returns>
        public Vector2 FarmHouseCoords(float OffsetX = 0, float OffsetY = 0) {
            if (Canon) {
                return FarmHouseCoordsCanon(OffsetX, OffsetY);
            }
            Placement Coordinates = LoadedFarm.FarmHouse.Coordinates;
            return new Vector2((Coordinates.X * 64f) + OffsetX, (Coordinates.Y * 64f) + OffsetY);
        }

        /// <summary>
        /// Gets the original (Canon) farmhouse coordinate values in Vector2 form. Allows
        /// offsetting the coordinates
        /// </summary>
        /// <param name="OffsetX">The Offset value for the X Coordinate</param>
        /// <param name="OffsetY">The Offset value for the Y Coordinate</param>
        /// <returns>The canon coordinates in Vector2 form.</returns>
        protected Vector2 FarmHouseCoordsCanon(float OffsetX, float OffsetY) {
            return new Vector2(3712f + OffsetX, 520f + OffsetY);
        }

        /// <summary>
        /// Computes and returns the Layer Depth value needed to properly render the Farmhouse.
        /// Returns the original (canon) Layer Depth if the farm is not a custom farm.
        /// </summary>
        /// <returns>The proper layer depth. Used in Spritebatch.Draw</returns>
        public float FarmHouseLayerDepth() {
            if (Canon) {
                return 0.075f;
            } else {
                return ((LoadedFarm.FarmHouse.PointOfInteraction.Y - 5 + 3) * 64) / 10000f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GreenHouseCoords() {
            if (Canon) {
                return GreenHouseCoordsCanon();
            }
            Placement Coordinates = LoadedFarm.FarmHouse.Coordinates;
            return new Vector2(Coordinates.X * 64f, Coordinates.Y * 64f);
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

        public Vector2 MailboxNotification(float xOffset, float yOffset, bool Option) {
            if (Canon) {
                return new Vector2((Option) ? 4388f : 4352f, ((Option) ? 928 : 880f) + yOffset);
            }
            Interaction POI = LoadedFarm.MailBox.PointOfInteraction;
            return new Vector2((POI.X * 64f) + xOffset, (POI.Y * 64f) + yOffset);
        }

        public float MailBoxNotifyLayerDepth(bool Option) {
            if (Canon) {
                return (Option) ? 0.11561f : 0.115601f;
            } else {
                return (((LoadedFarm.MailBox.PointOfInteraction.Y + 2) * 64f) / 10000f) + ((Option) ? 0.00041f : 0.000401f);
            }
        }

        public Vector2 GrandpaShrineCoords() {
            if (Canon) {
                return new Vector2(576f, 448f);
            }
            Interaction POI = LoadedFarm.GrandpaShrine.PointOfInteraction;
            return new Vector2(POI.X * 64f, POI.Y * 64f);
        }
    }
}
