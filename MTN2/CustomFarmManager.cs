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
        public bool Canon { get; set; } = true;

        public CustomFarm SelectedFarm {
            get {
                return FarmList[SelectedIndex];
            }
        }

        public CustomFarm LoadedFarm {
            get {
                return FarmList[LoadedIndex];
            }
        }

        public FileType BaseFarmFileType {
            get {
                return FarmList[LoadedIndex].FarmMap.FileType;
            }
        }

        public string BaseFarmFileName {
            get {
                return FarmList[LoadedIndex].FarmMap.FileName;
            }
        }

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

        public void LoadCustomFarm() {
            LoadedIndex = SelectedIndex;
        }

        public void LoadCustomFarm(int whichFarm) {
            if (whichFarm < 5) {
                NoDebris = true;
            }
        }
        
        public string GetAssetKey(out Map map) {
            if (LoadedFarm.FarmMap.FileType == FileType.raw) {
                map = LoadedFarm.ContentPack.LoadAsset<Map>(LoadedFarm.FarmMap.FileName + ".tbin");
            } else {
                map = null;
            }
            return LoadedFarm.ContentPack.GetActualAssetKey(LoadedFarm.FarmMap.FileName + ((LoadedFarm.FarmMap.FileType == FileType.raw) ? ".tbin" : ".xnb"));
        }

        public string GetAssetKey(string fileName, FileType fileType) {
            return LoadedFarm.ContentPack.GetActualAssetKey(fileName + ((fileType == FileType.raw) ? ".tbin" : ".xnb"));
        }

        public Map LoadMap(string fileName) {
            return LoadedFarm.ContentPack.LoadAsset<Map>(fileName);
        }
    }
}
