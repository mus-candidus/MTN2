using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MTN2.Compatibility;
using MTN2.MapData;
using StardewModdingAPI;
using StardewValley.Locations;

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
    /// CustomManager class. Performs all the nessecary operations needed to allow a custom maps (custom farms, greenhouse, etc)
    /// to properly function. It is also built to distinguish between canon, or custom. Contains a List<T>
    /// of custom classes and manipulates / gathers the data accordingly.
    /// </summary>
    internal class CustomManager : ICustomManager {
        public FarmManagement FarmManager { get; private set; }
        public GHouseManagement GreenhouseManager { get; private set; }
        public FHouseManagement HouseManager { get; private set; }
        
        public bool NoDebris { get; set; } = false;
        public bool Canon { get; private set; } = true;
        public int ScienceHouseIndex { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomManager() {
            FarmManager = new FarmManagement();
            GreenhouseManager = new GHouseManagement(FarmManager);
            HouseManager = new FHouseManagement(FarmManager);
        }

        /// <summary>
        /// Populates the List<CustomFarm> FarmList variable with all the Content Packs
        /// registered to MTN.
        /// </summary>
        /// <param name="helper">SMAPI's IModHelper, to load in the Content Packs</param>
        /// <param name="monitor">SMAPI's IMonitor, to print useful information</param>
        public void Populate(IModHelper helper, IMonitor monitor) {

            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned()) {
                monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}.");
                FarmManager.Populate(helper, contentPack, monitor);
                GreenhouseManager.Populate(contentPack, monitor);
            }

            return;
        }

        /// <summary>
        /// Updates the selected farm. Used during the creation of a new game.
        /// </summary>
        /// <param name="farmName"></param>
        public void UpdateSelectedFarm(string farmName) {
            Canon = FarmManager.Update(farmName);
            return;
        }

        /// <summary>
        /// Set the Selected Farm as the Loaded Farm. Used when creating a new game,
        /// as a confirmation.
        /// </summary>
        public void LoadCustomFarm() {
            Canon = FarmManager.Load();
            if (!Canon) GreenhouseManager.LinkToFarm(FarmManager.LoadedFarm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whichFarm"></param>
        public void LoadCustomFarm(int whichFarm) {
            Canon = FarmManager.Load(whichFarm);
            if (!Canon) GreenhouseManager.LinkToFarm(FarmManager.LoadedFarm);
        }
        
        /// <summary>
        /// Gets the Asset Key of the Base Farm Map. Only does work on the base
        /// farm map of any custom map.
        /// </summary>
        /// <param name="map">The base farm map, loaded.</param>
        /// <returns>The Actual Asset Key</returns>
        public string GetAssetKey(out Map map, string type) {
            if (type == "Greenhouse") {
                if (LoadedFarm.CustomGreenhouse.GreenhouseMap.FileType == FileType.raw || LoadedFarm.CustomGreenhouse.GreenhouseMap.FileType == FileType.tbin) {
                    map = LoadedFarm.CustomGreenhouse.ContentPack.LoadAsset<Map>(LoadedFarm.CustomGreenhouse.GreenhouseMap.FileName + ".tbin");
                } else {
                    map = null;
                }
                return LoadedFarm.CustomGreenhouse.ContentPack.GetActualAssetKey(LoadedFarm.CustomGreenhouse.GreenhouseMap.FileName + ((LoadedFarm.CustomGreenhouse.GreenhouseMap.FileType == FileType.raw) ? ".tbin" : ".xnb"));
            }
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
            if (Canon || LoadedFarm.FarmHouse == null) {
                return FarmHouseCoordsCanon(OffsetX, OffsetY);
            }
            Placement? Coordinates = LoadedFarm.FarmHouse.Coordinates;
            return new Vector2((Coordinates.Value.X * 64f) + OffsetX, (Coordinates.Value.Y * 64f) + OffsetY);
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



        public Vector2 MailboxNotification(float xOffset, float yOffset, bool Option) {
            if (Canon || LoadedFarm.MailBox == null) {
                return new Vector2((Option) ? 4388f : 4352f, ((Option) ? 928f : 880f) + yOffset);
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

        public void SetScienceIndex(int index) {
            ScienceHouseIndex = index;
        }

        public void Reset() {
            SelectedIndex = 0;
            LoadedIndex = -1;
            Canon = true;
        }
    }
}
