﻿using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.StringComparer;

namespace MTN2.Patches.FarmHousePatches
{
    /// <summary>
    /// REASON FOR PATCHING: Adjust exit warp if Farm House was moved.
    /// 
    /// Patches the method FarmHouse.updateMap to accomidate for custom
    /// farm maps with the farm house relocated. Resets the exit warp,
    /// the one players use when leaving the farm housue.
    /// </summary>
    public class updateMapPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public updateMapPatch(ICustomManager customManager) {
            updateMapPatch.customManager = customManager;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has executed.
        /// 
        /// Resets the warps when the Farm House is upgraded to accomidate for custom farm maps, 
        /// assuming the farm house has been relocated.
        /// </summary>
        /// <param name="__instance">The instance of FarmHouse that called updateMap.</param>
        public static void Postfix(FarmHouse __instance) {
            // TO DO: Refactor for custom FarmHouse maps.
            if (customManager.Canon) return;

            if (__instance is Cabin) {
                // TO DO
            } else {
                // Modify the warp point to farm.
                Warp farmWarp = __instance.warps.First(warp => OrdinalIgnoreCase.Equals(warp.TargetName, "Farm"));
                farmWarp.TargetX = customManager.FarmHousePorch.X;
                farmWarp.TargetY = customManager.FarmHousePorch.Y;
            }
        }
    }
}
