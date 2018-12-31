using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using MTN2.Locations;
using MTN2.Menus;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MTN2
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        protected HarmonyInstance Harmony;
        protected CustomFarmManager FarmManager;
        protected PatchManager PatchManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModEntry() {
            FarmManager = new CustomFarmManager();
            PatchManager = new PatchManager(FarmManager);
        }

        /// <summary>
        /// Main function / Entry point of MTN. Executed by SMAPI.
        /// </summary>
        /// <param name="helper">Interface of ModHelper. Provides access to various SMAPI tools/methods.</param>
        public override void Entry(IModHelper helper) {
            Monitor.Log("Begin: Harmony Patching", LogLevel.Trace);
            Harmony = HarmonyInstance.Create("MTN.SgtPickles");
            PatchManager.Initialize(helper, Monitor);
            PatchManager.Apply(Harmony);

            //Helper.Events.Display.MenuChanged += NewGameMenu;
            Helper.Events.GameLoop.UpdateTicked += NewGameMenu;
            Helper.Events.GameLoop.GameLaunched += Populate;
            Helper.Events.GameLoop.SaveLoaded += InitialScienceLab;
            Helper.Events.GameLoop.Saving += BeforeSaveScienceLab;
            Helper.Events.GameLoop.Saved += AfterSaveScienceLab;

            Helper.ConsoleCommands.Add("LocationEntry", "Lists (all) the location loaded in the game.\nUsage: LocationEntry <number\n- number: An integer value.\nIf omitted, all locations will be listed.", ListLocation);
            return;
        }

        /// <summary>
        /// Populates the CustomFarmManager with the installed Content Packs registered for MTN.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Populate(object sender, EventArgs e) {
            FarmManager.Populate(Helper, Monitor);
        }

        /// <summary>
        /// Used to replace the vanilla Character Customization Menu with MTN's version. This allows the user to
        /// be able to select custom farms and other options available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameMenu(object sender, EventArgs e) {
            if (Game1.activeClickableMenu is TitleMenu) {
                if (TitleMenu.subMenu is CharacterCustomization) {
                    CharacterCustomization oldMenu = (CharacterCustomization)TitleMenu.subMenu;
                    CharacterCustomizationMTN menu = new CharacterCustomizationMTN(FarmManager, oldMenu.source);
                    TitleMenu.subMenu = menu;
                }
            }
        }

        /// <summary>
        /// Replaces the canon ScienceHouse map with AdvancedScienceHouse. Enables the players to access additional
        /// options from Robin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialScienceLab(object sender, EventArgs e) {
            GameLocation scienceHouse = Game1.getLocationFromName("ScienceHouse"); ;
            int index = Game1.locations.IndexOf(scienceHouse);

            Game1.locations[index] = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", scienceHouse);
            FarmManager.SetScienceIndex(index);
        }

        /// <summary>
        /// Routine call prior to save game being called. Swaps the AdvancedScienceHouse with ScienceHouse. This is done
        /// to bypass Serialization concerns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeSaveScienceLab(object sender, EventArgs e) {
            AdvancedScienceHouse scienceHouse = (AdvancedScienceHouse)Game1.locations[FarmManager.ScienceHouseIndex];
            Game1.locations[FarmManager.ScienceHouseIndex] = scienceHouse.Export();
        }

        /// <summary>
        /// Routine call made after save game was finished. Swaps the ScienceHouse with AdvancedScienceHouse. Re-enables the
        /// player to access additional options from Robin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterSaveScienceLab(object sender, EventArgs e) {
            AdvancedScienceHouse reloadedHouse = new AdvancedScienceHouse(Path.Combine("Maps", "ScienceHouse"), "ScienceHouse", Game1.locations[FarmManager.ScienceHouseIndex]);
            Game1.locations[FarmManager.ScienceHouseIndex] = reloadedHouse;
        }

        private void ListLocation(string command, string[] args) {
            int index;

            if (args.Length < 1) {
                PrintAllLocations();
                return;
            }

            index = int.Parse(args[0]);
            if (index >= Game1.locations.Count) {
                Monitor.Log($"Error: Value must be lower than the number of locations (Current have {Game1.locations.Count} locations).");
            } else {
                Monitor.Log($"Location {index}: {Game1.locations[index].Name} - Type: {Game1.locations[index].ToString()}");
                if (Game1.locations[index].Root == null) {
                    Monitor.Log($"Location Root is null. (This map is disposable)", LogLevel.Error);
                } else {
                    Monitor.Log($"NetRef: {Game1.locations[index].Root} (This map is always active)");
                }
            }
        }
        
        private void PrintAllLocations() {
            for (int i = 0; i < Game1.locations.Count; i++) {
                Monitor.Log($"Location {i}: {Game1.locations[i].Name} - Type: {Game1.locations[i].ToString()}");
            }
            return;
        }
    }
}
