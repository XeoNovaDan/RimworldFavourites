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

                // Separate items that have are or are not junk, or whose icon colours are different
                if (a != null && b != null)
                {
                    bool compAFav = favCompA.Favourited;
                    bool compBFav = favCompB.Favourited;
                    bool compAJnk = favCompA.Junk;
                    bool compBJnk = favCompB.Junk;

                    if (compAFav != compBFav || compAJnk != compBJnk)
                        __result = false;
                    else if (compAFav && compBFav && favCompA.StarColour != favCompB.StarColour)
                        __result = false;
                    else if (compAJnk && compBJnk && favCompA.BinColour != favCompB.BinColour)
                        __result = false;
                }
            }

        }

    }

}
