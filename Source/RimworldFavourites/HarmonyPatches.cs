using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace RimworldFavourites
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {

        static HarmonyPatches()
        {
            RimworldFavourites.harmonyInstance.PatchAll();
        }

        [HarmonyPatch(typeof(TransferableUIUtility))]
        [HarmonyPatch(nameof(TransferableUIUtility.DoExtraAnimalIcons))]
        public static class Patch_TransferableUIUtility_DoExtraAnimalIcons
        {

            public static void Postfix(Transferable trad, Rect rect, ref float curX)
            {
                // Add in favourited icon on trade menu
                var thing = trad.AnyThing as ThingWithComps;
                if (thing != null)
                {
                    var favouriteComp = thing.TryGetComp<CompFavouritable>();
                    if (favouriteComp != null && favouriteComp.Favourited)
                    {
                        // 24 - Same as BondIconWidth
                        DrawFavouritedIcon(new Rect(curX - 24, (rect.height - 24) / 2, 24, 24));
                        curX -= 24; 
                    }
                }
            }

            public static void DrawFavouritedIcon(Rect rect)
            {
                GUI.DrawTexture(rect, TexCommand.Favourited);
                if (Mouse.IsOver(rect))
                    TooltipHandler.TipRegion(rect, "RimworldFavourites.FavouritedIconDesc".Translate());
            }

        }

        [HarmonyPatch(typeof(GenRecipe))]
        [HarmonyPatch("PostProcessProduct")]
        public static class Patch_GenRecipe_PostProcessProduct
        {

            public static void Postfix(ref Thing __result)
            {
                // Auto favourite produced items that are Masterwork or higher quality
                var qualityComp = __result.TryGetComp<CompQuality>();
                if (qualityComp != null && qualityComp.Quality >= QualityCategory.Masterwork)
                {
                    var favouriteComp = __result.TryGetComp<CompFavouritable>();
                    if (favouriteComp != null)
                        favouriteComp.Favourited = true;
                }
            }

        }

        [HarmonyPatch(typeof(Reward_Items))]
        [HarmonyPatch(nameof(Reward_Items.InitFromValue))]
        public static class Patch_Reward_Items_InitFromValue
        {

            public static void Postfix(Reward_Items __instance)
            {
                // Auto favourite quest rewards if not a raw or manufactured resource
                var items = __instance.ItemsListForReading;
                if (!items.NullOrEmpty())
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (!item.HasThingCategory(ThingCategoryDefOf.ResourcesRaw) && !item.HasThingCategory(ThingCategoryDefOf.Manufactured))
                        {
                            var favouriteComp = item.TryGetComp<CompFavouritable>();
                            if (favouriteComp != null)
                                favouriteComp.Favourited = true;
                        }
                    }
                }
            }

        }

    }
}
