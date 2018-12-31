using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
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
            Helper.Events.GameLoop.GameLaunched += Populate;
            Helper.Events.Display.MenuChanged += NewGameMenu;
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
    }
}
