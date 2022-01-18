using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

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
                if (RimworldFavourites.settings.autoFavourite && RimworldFavourites.settings.autoFavouriteManufacturedThings)
                {
                    var qualityComp = thing.TryGetComp<CompQuality>();
                    if (qualityComp != null && RimworldFavourites.settings.autoFavouriteManufacturedQualityRange.Includes(qualityComp.Quality))
                    {
                        var favouriteComp = thing.TryGetComp<CompFavouritable>();
                        if (favouriteComp != null)
                            favouriteComp.Favourited = true;
                    }
                }
            }

        }

        [HarmonyPatch(typeof(Reward))]
        [HarmonyPatch(nameof(Reward.Notify_Used))]
        public static class Patch_Reward_Notify_Used
        {

            public static void Postfix(Reward __instance)
            {
                var modSettings = RimworldFavourites.settings;
                if (modSettings.autoFavourite && modSettings.autoFavouriteQuestRewards)
                {
                    if (__instance is Reward_Items itemReward)
                    {
                        var items = itemReward.items;
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            if (item.TryGetComp<CompFavouritable>() is CompFavouritable favouritable && (modSettings.autoFavouriteQuestRewardRawMaterials ||
                                (!item.HasThingCategory(ThingCategoryDefOf.ResourcesRaw) && !item.HasThingCategory(ThingCategoryDefOf.Manufactured))))
                                favouritable.Favourited = true;
                        }
                    }
                    else if (__instance is Reward_Pawn pawnReward && modSettings.autoFavouriteQuestRewardPawns)
                    {
                        var pawn = pawnReward.pawn;

                        bool favouritePawn = modSettings.autoFavouriteQuestRewardPawnsUnrelated;

                        if (!favouritePawn && pawn.relations is Pawn_RelationsTracker relations)
                        {
                            var allPawnsList = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners;

                            var directRelations = relations.DirectRelations;

                            for (int i = 0; i < directRelations.Count; i++)
                            {
                                var curRelation = directRelations[i];
                                if (allPawnsList.Contains(curRelation.otherPawn))
                                {
                                    favouritePawn = true;
                                    break;
                                }
                            }
                        }

                        if (favouritePawn && pawn.TryGetComp<CompFavouritable>() is CompFavouritable favouritable)
                            favouritable.Favourited = true;

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
