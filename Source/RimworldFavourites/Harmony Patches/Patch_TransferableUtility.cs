using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimworldFavourites
{
    public static class Patch_TransferableUtility
    {

        [HarmonyPatch(typeof(TransferableUtility))]
        [HarmonyPatch(nameof(TransferableUtility.TransferAsOne))]
        public static class Patch_TransferableUtility_TransferAsOne
        {

            public static void Postfix(ref bool __result, Thing a, Thing b)
            {
                var favCompA = a.TryGetComp<CompFavouritable>();
                var favCompB = b.TryGetComp<CompFavouritable>();
                if (a != null && b != null && (favCompA.Favourited != favCompB.Favourited || favCompA.Junk != favCompB.Junk))
                    __result = false;
            }

        }

    }

}
