using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimworldFavourites
{
    public static class Patch_QualityUtility
    {

        [HarmonyPatch(typeof(QualityUtility))]
        [HarmonyPatch(nameof(QualityUtility.SendCraftNotification))]
        public static class Patch_SendCraftNotification
        {

            public static void Postfix(ref Thing thing)
            {
                // Auto favourite produced items that are Masterwork or higher quality
                if (RimworldFavourites.settings.autoFavourite && RimworldFavourites.settings.autoFavouriteManufacturedThings)
                {
                    var qualityComp = thing.TryGetComp<CompQuality>();
                    if (qualityComp != null && RimworldFavourites.settings.autoFavouriteProdtctQualityRange.Includes(qualityComp.Quality))
                    {
                        var favouriteComp = thing.TryGetComp<CompFavouritable>();
                        if (favouriteComp != null)
                            favouriteComp.Favourited = true;
                    }
                }
            }

        }

    }

}
