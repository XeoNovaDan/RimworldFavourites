using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{

    public class CompFavouritable : ThingComp
    {

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // Favourite button
            yield return new Command_Toggle
            {
                defaultLabel = "RimworldFavourites.Favourite".Translate(),
                defaultDesc = "RimworldFavourites.FavouriteDesc".Translate(),
                icon = TexCommand.FavouriteStar,
                defaultIconColor = StarColour,
                isActive = () => Favourited,
                toggleAction = () =>
                {
                    // Right click - Draw float menu with list of options to choose the colour of the favourited star
                    if (Event.current.button == 1)
                        DrawColourSelectFloatMenu(StarColour, TexCommand.FavouriteStar);

                    else
                        Favourited = !Favourited;
                }
            };

            // Junk button
            yield return new Command_Toggle
            {
                defaultLabel = "RimworldFavourites.Junk".Translate(),
                defaultDesc = "RimworldFavourites.JunkDesc".Translate(),
                icon = TexCommand.JunkBin,
                defaultIconColor = BinColour,
                isActive = () => Junk,
                toggleAction = () =>
                {
                    // Right click - Draw float menu with list of options to choose the colour of the favourited star
                    if (Event.current.button == 1)
                        DrawColourSelectFloatMenu(BinColour, TexCommand.JunkBin);

                    else
                        Junk = !Junk;
                }
            };
        }

        private void DrawColourSelectFloatMenu(Color colourToChange, Texture2D tex)
        {
            var options = new List<FloatMenuOption>();
            options.Add(new FloatMenuOption("RimworldFavourites.Yellow".Translate(), () => ChangeIconColour(ref colourToChange, Color.yellow), tex, Color.yellow));
            options.Add(new FloatMenuOption("RimworldFavourites.Red".Translate(), () => ChangeIconColour(ref colourToChange, Color.red), tex, Color.red));
            options.Add(new FloatMenuOption("RimworldFavourites.Green".Translate(), () => ChangeIconColour(ref colourToChange, Color.green), tex, Color.green));
            options.Add(new FloatMenuOption("RimworldFavourites.Cyan".Translate(), () => ChangeIconColour(ref colourToChange, Color.cyan), tex, Color.cyan));
            options.Add(new FloatMenuOption("RimworldFavourites.Magenta".Translate(), () => ChangeIconColour(ref colourToChange, Color.magenta), tex, Color.magenta));
            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static void ChangeIconColour(ref Color colourToChange, Color colour)
        {
            // Change the star/bin colour of all selected items, otherwise only a single colour gets changed
            var selectedItems = Find.Selector.SelectedObjectsListForReading;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var thing = selectedItems[i] as Thing;
                if (thing != null && thing.TryGetComp<CompFavouritable>() is CompFavouritable)
                    colourToChange = colour;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref favourited, "favourited");
            Scribe_Values.Look(ref junk, "junk");
            Scribe_Values.Look(ref starColour, "starColour");
            Scribe_Values.Look(ref binColour, "binColour");
        }

        public override string TransformLabel(string label)
        {
            // If the item's in a ThingHolder that isn't a minified thing e.g. equipped by a pawn, start the label with "FAV -" or "JNK -"
            if (!parent.Spawned && (!(ParentHolder is MinifiedThing) || !((MinifiedThing)ParentHolder).Spawned))
            {
                if (Favourited)
                    return "RimworldFavourites.FavouritedShortUpper".Translate() + " - " + label.CapitalizeFirst();
                else if (Junk)
                    return "RimworldFavourites.JunkShortUpper".Translate() + " - " + label.CapitalizeFirst();
            }
            return label;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            // Try and register the item as a favourite to render when spawned
            base.PostSpawnSetup(respawningAfterLoad);
            cachedFavouriteOverlayDrawer = parent.Map.GetComponent<FavouriteOverlayDrawer>();
            UpdateOverlayHandle();
        }

        public override void PostDeSpawn(Map map)
        {
            // Force deregister an item when despawned e.g. furniture minified, to prevent weird duplicate star overlays being rendered
            base.PostDeSpawn(map);
            cachedFavouriteOverlayDrawer.Deregister(parent, this, false);
        }

        private void UpdateOverlayHandle()
        {
            if (!parent.Spawned)
                return;

            // Register/unregister the item to draw a favourite star over
            cachedFavouriteOverlayDrawer.Deregister(parent, this);
            
            if (parent.Spawned && (Favourited || Junk))
                cachedFavouriteOverlayDrawer.Register(parent, this);
        }

        private MinifiedThing MinifiedParent => parent as MinifiedThing;

        public bool Favourited
        {
            get
            {
                // Take minified things into account
                if (MinifiedParent != null)
                {
                    var favouriteComp = MinifiedParent.InnerThing.TryGetComp<CompFavouritable>();

                    // This should never not be null to be honest as InnerThing would have CompNullifiable thus should be autopatched as it's a ThingWithComps, paranoid nullcheck
                    if (favouriteComp != null)
                        return favouriteComp.Favourited;
                }

                return favourited;
            }
            set
            {
                // An item cannot be favourite and junk at the same time
                if (value)
                {
                    Junk = false;
                    cachedMatToDraw = null;
                }

                // Take minified things into account
                if (MinifiedParent != null)
                {
                    var favouriteComp = MinifiedParent.InnerThing.TryGetComp<CompFavouritable>();
                    if (favouriteComp != null)
                        favouriteComp.Favourited = value;
                }

                else
                    favourited = value;

                UpdateOverlayHandle();
            }
        }

        public bool Junk
        {
            get
            {

                // Take minified things into account
                if (MinifiedParent != null)
                {
                    var favouriteComp = MinifiedParent.InnerThing.TryGetComp<CompFavouritable>();

                    // This should never not be null to be honest as InnerThing would have CompNullifiable thus should be autopatched as it's a ThingWithComps, paranoid nullcheck
                    if (favouriteComp != null)
                        return favouriteComp.Junk;
                }

                return junk;
            }
            set
            {
                // An item cannot be junk and favourite at the same time
                if (value)
                {
                    Favourited = false;
                    cachedMatToDraw = null;
                }

                // Take minified things into account
                if (MinifiedParent != null)
                {
                    var favouriteComp = MinifiedParent.InnerThing.TryGetComp<CompFavouritable>();
                    if (favouriteComp != null)
                        favouriteComp.Junk = value;
                }

                else
                    junk = value;

                UpdateOverlayHandle();
            }
        }

        public Color StarColour
        {
            get
            {
                if (Favourited)
                {
                    if (starColour == default)
                        return Color.yellow;
                    return starColour;
                }

                return Color.grey;
            }
            set
            {
                starColour = value;
                cachedMatToDraw = null;
            }
        }

        public Color BinColour
        {
            get
            {
                if (Junk)
                {
                    if (binColour == default)
                        return Color.red;
                    return binColour;
                }
                    

                return Color.grey;
            }
            set
            {
                binColour = value;
                cachedJunkMat = null;
            }
        }

        public Material MatToDraw
        {
            get
            {
                if (cachedMatToDraw == null)
                {
                    if (Favourited)
                    {
                        if (StarColour == Color.yellow)
                            cachedMatToDraw = FavouriteOverlayDrawer.FavMatYellow;
                        else if (StarColour == Color.red)
                            cachedMatToDraw = FavouriteOverlayDrawer.FavMatRed;
                        else if (StarColour == Color.green)
                            cachedMatToDraw = FavouriteOverlayDrawer.FavMatGreen;
                        else if (StarColour == Color.cyan)
                            cachedMatToDraw = FavouriteOverlayDrawer.FavMatCyan;
                        else if (StarColour == Color.magenta)
                            cachedMatToDraw = FavouriteOverlayDrawer.FavMatMagenta;
                        else
                            throw new NotImplementedException("No FavMat for StarColour");
                    }
                    else if (Junk)
                    {
                        if (BinColour == Color.yellow)
                            cachedMatToDraw = FavouriteOverlayDrawer.JunkMatYellow;
                        if (BinColour == Color.red)
                            cachedMatToDraw = FavouriteOverlayDrawer.JunkMatRed;
                        else if (BinColour == Color.green)
                            cachedMatToDraw = FavouriteOverlayDrawer.JunkMatGreen;
                        else if (BinColour == Color.cyan)
                            cachedMatToDraw = FavouriteOverlayDrawer.JunkMatCyan;
                        else if (BinColour == Color.magenta)
                            cachedMatToDraw = FavouriteOverlayDrawer.JunkMatMagenta;
                        else
                            throw new NotImplementedException("No JunkMat for BinColour");
                    }
                    else
                        throw new NotImplementedException("Tried to get MatToDraw but not Favourited or Junk");
                }
                return cachedMatToDraw;
            }
        }

        public Material JunkMaterial
        {
            get
            {
                if (cachedJunkMat == null)
                {
                    if (binColour == Color.yellow)
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatYellow;
                    if (binColour == Color.red)
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatRed;
                    else if (binColour == Color.green)
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatGreen;
                    else if (binColour == Color.cyan)
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatCyan;
                    else if (binColour == Color.magenta)
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatMagenta;
                    else
                        cachedJunkMat = FavouriteOverlayDrawer.FavMatRed;
                }
                return cachedJunkMat;
            }
        }

        private bool favourited;
        private bool junk;
        private Color starColour;
        private Color binColour;
        private FavouriteOverlayDrawer cachedFavouriteOverlayDrawer;
        private Material? cachedMatToDraw;
        private Material? cachedJunkMat;

    }

}