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
                        DrawColourSelectFloatMenu();

                    else
                        Favourited = !Favourited;
                }
            };
        }

        private void DrawColourSelectFloatMenu()
        {
            var options = new List<FloatMenuOption>();
            options.Add(new FloatMenuOption("RimworldFavourites.Yellow".Translate(), () => ChangeStarColour(Color.yellow), TexCommand.FavouriteStar, Color.yellow));
            options.Add(new FloatMenuOption("RimworldFavourites.Red".Translate(), () => ChangeStarColour(Color.red), TexCommand.FavouriteStar, Color.red));
            options.Add(new FloatMenuOption("RimworldFavourites.Green".Translate(), () => ChangeStarColour(Color.green), TexCommand.FavouriteStar, Color.green));
            options.Add(new FloatMenuOption("RimworldFavourites.Cyan".Translate(), () => ChangeStarColour(Color.cyan), TexCommand.FavouriteStar, Color.cyan));
            options.Add(new FloatMenuOption("RimworldFavourites.Magenta".Translate(), () => ChangeStarColour(Color.magenta), TexCommand.FavouriteStar, Color.magenta));
            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static void ChangeStarColour(Color colour)
        {
            // Change the star colour of all selected items, otherwise only a single colour gets changed
            var selectedItems = Find.Selector.SelectedObjectsListForReading;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var thing = selectedItems[i] as Thing;
                if (thing != null && thing.TryGetComp<CompFavouritable>() is CompFavouritable favouriteComp)
                    favouriteComp.StarColour = colour;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref favourited, "favourited");
            Scribe_Values.Look(ref starColour, "starColour", Color.yellow);
        }

        public override string TransformLabel(string label)
        {
            // If the item's in a ThingHolder that isn't a minified thing e.g. equipped by a pawn, start the label with "FAV -"
            if (!parent.Spawned && (!(ParentHolder is MinifiedThing) || !((MinifiedThing)ParentHolder).Spawned) && Favourited)
                return "RimworldFavourites.FavouritedShortUpper".Translate() + " - " + label.CapitalizeFirst();
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
            cachedFavouriteOverlayDrawer.Deregister(parent, this, false);
        }

        private void UpdateOverlayHandle()
        {
            if (!parent.Spawned)
                return;

            // Register/unregister the item to draw a favourite star over
            cachedFavouriteOverlayDrawer.Deregister(parent, this);
            
            if (parent.Spawned && Favourited)
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

        public Color StarColour
        {
            get
            {
                if (Favourited)
                    return starColour;

                return Color.grey;
            }
            set
            {
                starColour = value;
                cachedFavouriteMat = null;
            }
        }

        public Material FavouritedMaterial
        {
            get
            {
                if (cachedFavouriteMat == null)
                {
                    if (starColour == Color.yellow)
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatYellow;
                    if (starColour == Color.red)
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatRed;
                    else if (starColour == Color.green)
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatGreen;
                    else if (starColour == Color.cyan)
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatCyan;
                    else if (starColour == Color.magenta)
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatMagenta;
                    else
                    {
                        Log.Warning("Invalid colour for favourited material. Defaulting to yellow...");
                        cachedFavouriteMat = FavouriteOverlayDrawer.FavMatYellow;
                    }
                }
                return cachedFavouriteMat;
            }
        }

        private bool favourited;
        private Color starColour = Color.yellow;
        private FavouriteOverlayDrawer cachedFavouriteOverlayDrawer;
        private Material? cachedFavouriteMat;

    }

}