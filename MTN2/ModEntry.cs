using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;

namespace MTN2
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {

        private List<Patch> Patches;
        protected HarmonyInstance Harmony;
        protected CustomFarmManager FarmManager;

        public ModEntry() {
            FarmManager = new CustomFarmManager();
        }

        /// <summary>
        /// Main function / Entry point of MTN. Executed by SMAPI.
        /// </summary>
        /// <param name="helper">Interface of ModHelper. Provides access to various SMAPI tools/methods.</param>
        public override void Entry(IModHelper helper) {
            Monitor.Log("Begin: Harmony Patching", LogLevel.Trace);
            Harmony = HarmonyInstance.Create("MTN.SgtPickles");


            return;
        }

        private void Populate(object sender, EventArgs e) {
            FarmManager.Populate(Helper, Monitor);
        }
    }
}
