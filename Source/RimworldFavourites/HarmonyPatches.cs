using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
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

        [HarmonyPatch(typeof(QualityUtility))]
        [HarmonyPatch(nameof(QualityUtility.SendCraftNotification))]
        public static class Patch_QualityUtility_SendCraftNotification
        {

            public static void Postfix(ref Thing thing)
            {
                // Auto favourite produced items that are Masterwork or higher quality
                var qualityComp = thing.TryGetComp<CompQuality>();
                if (qualityComp != null && qualityComp.Quality >= QualityCategory.Masterwork)
                {
                    var favouriteComp = thing.TryGetComp<CompFavouritable>();
                    if (favouriteComp != null)
                        favouriteComp.Favourited = true;
                }
            }

        }

        [HarmonyPatch(typeof(QuestPart_DropPods))]
        [HarmonyPatch(nameof(QuestPart_DropPods.Notify_QuestSignalReceived))]
        public static class Patch_QuestPart_DropPods_Notify_QuestSignalReceived
        {

            public static void Postfix(QuestPart_DropPods __instance, List<Thing> ___tmpThingsToDrop)
            {
                // Auto favourite quest reward items that are not a raw or manufactured resource
                if (!___tmpThingsToDrop.NullOrEmpty())
                {
                    for (int i = 0; i < ___tmpThingsToDrop.Count; i++)
                    {
                        var curThing = ___tmpThingsToDrop[i];
                        if (curThing.TryGetComp<CompFavouritable>() is CompFavouritable favouriteComp && !curThing.HasThingCategory(ThingCategoryDefOf.ResourcesRaw) &&
                            !curThing.HasThingCategory(ThingCategoryDefOf.Manufactured))
                        {
                            favouriteComp.Favourited = true;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RimWorld.PlaySettings))]
        [HarmonyPatch(nameof(RimWorld.PlaySettings.ExposeData))]
        public static class Patch_PlaySettings_ExposeData
        {

            public static void Postfix()
            {
                Scribe_Values.Look(ref PlaySettings.showFavouritesOverlay, "showFavouritesOverlay", true);
            }
        }

        [HarmonyPatch(typeof(RimWorld.PlaySettings))]
        [HarmonyPatch(nameof(RimWorld.PlaySettings.DoPlaySettingsGlobalControls))]
        public static class Patch_PlaySettings_DoPlaySettingsGlobalControls
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

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch(nameof(Pawn.GetGizmos))]
        public static class Patch_Pawn_GetGizmos
        {

            public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
            {
                var lord = __instance.GetLord();
                LordJob_Ritual ritual;

                // Show favourite button on non player pawns and animals too, or pawns doing rituals where drafting is blocked
                if (!__instance.IsColonistPlayerControlled || (lord != null && (ritual = lord.LordJob as LordJob_Ritual) != null) && ritual.BlocksDrafting)
                {
                    var resultList = __result.ToList();

                    var favComp = __instance.TryGetComp<CompFavouritable>();
                    if (favComp != null)
                    {
                        var compGizmos = favComp.CompGetGizmosExtra();
                        foreach (var gizmo in compGizmos)
                        {
                            resultList.Add(gizmo);
                        }
                    }

                    __result = resultList;
                }
                
            }
        }

    }

}
