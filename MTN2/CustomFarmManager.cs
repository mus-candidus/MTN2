using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class CustomFarmManager {
        public List<CustomFarm> FarmList { get; private set; }
        public bool NoDebris { get; set; } = false;

        public void Populate(IModHelper Helper, IMonitor Monitor) {
            CustomFarm FarmData;
            string IconFile;

            FarmList = new List<CustomFarm>();

            foreach (IContentPack ContentPack in Helper.GetContentPacks()) {
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
    }
}
