using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimworldFavourites
{

    public static class Patch_PlaySettings
    {

        [HarmonyPatch(typeof(RimWorld.PlaySettings))]
        [HarmonyPatch(nameof(RimWorld.PlaySettings.ExposeData))]
        public static class Patch_ExposeData
        {

            public static void Postfix()
            {
                Scribe_Values.Look(ref PlaySettings.showFavouritesOverlay, "showFavouritesOverlay", true);
            }
        }

        [HarmonyPatch(typeof(RimWorld.PlaySettings))]
        [HarmonyPatch(nameof(RimWorld.PlaySettings.DoPlaySettingsGlobalControls))]
        public static class Patch_DoPlaySettingsGlobalControls
        {

            public static void Postfix(ref WidgetRow row, bool worldView)
            {
                // Add option in bottom right to toggle favourites overlay
                if (!worldView)
                {
                    row.ToggleableIcon(ref PlaySettings.showFavouritesOverlay, TexCommand.ShowFavouritesOverlay, "RimworldFavourites.ShowFavouritesOverlayButton".Translate());
                }
            }
        }

    }

}
