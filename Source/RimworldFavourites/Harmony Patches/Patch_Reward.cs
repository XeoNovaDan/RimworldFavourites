using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimworldFavourites
{

    public static class Patch_Reward
    {

        [HarmonyPatch(typeof(Reward))]
        [HarmonyPatch(nameof(Reward.Notify_Used))]
        public static class Patch_Notify_Used
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


    }

}
