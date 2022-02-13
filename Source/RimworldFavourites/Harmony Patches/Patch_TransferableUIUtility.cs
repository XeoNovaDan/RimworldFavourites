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
                    if (favouriteComp != null)
                    {
                        bool favourited = favouriteComp.Favourited;
                        bool junk = favouriteComp.Junk;
                        if (favourited || junk)
                        {
                            // 24 - Same as BondIconWidth
                            var iconRect = new Rect(curX - 24, (rect.height - 24) / 2, 24, 24);
                            if (favourited)
                                DrawTradeIcon(iconRect, TexCommand.FavouriteStar, favouriteComp.StarColour, "RimworldFavourites.FavouritedIconDesc".Translate());
                            else if (junk)
                                DrawTradeIcon(iconRect, TexCommand.JunkBin, favouriteComp.BinColour, "RimworldFavourites.JunkIconDesc".Translate());
                            else
                                throw new NotImplementedException($"Tried to draw trade icon for {trad} but {thing} not Favourited or Junk");
                            curX -= 24;
                        }
                    }
                }
            }

            public static void DrawTradeIcon(Rect rect, Texture2D tex, Color colour, string toolTip)
            {
                var prevColour = GUI.color;
                GUI.color = colour;
                GUI.DrawTexture(rect, tex);
                GUI.color = prevColour;
                if (Mouse.IsOver(rect))
                    TooltipHandler.TipRegion(rect, toolTip);
            }

        }

    }

}
