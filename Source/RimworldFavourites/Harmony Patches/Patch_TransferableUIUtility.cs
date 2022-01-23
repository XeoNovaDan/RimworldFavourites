using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimworldFavourites
{
    public static class Patch_TransferableUIUtility
    {

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
                        DrawFavouritedIcon(new Rect(curX - 24, (rect.height - 24) / 2, 24, 24), favouriteComp.StarColour);
                        curX -= 24; 
                    }
                }
            }

            public static void DrawFavouritedIcon(Rect rect, Color starColour)
            {
                var prevColour = GUI.color;
                GUI.color = starColour;
                GUI.DrawTexture(rect, TexCommand.FavouriteStar);
                GUI.color = prevColour;
                if (Mouse.IsOver(rect))
                    TooltipHandler.TipRegion(rect, "RimworldFavourites.FavouritedIconDesc".Translate());
            }

        }

    }

}
