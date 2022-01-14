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
                icon = Favourited ? TexCommand.Favourited : TexCommand.Unfavourited,
                isActive = () => Favourited,
                toggleAction = () =>
                {
                    Favourited = !Favourited;
                }
            };
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref favourited, "favourited");
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
            cachedFavouriteOverlayDrawer.Deregister(parent, false);
        }

        private void UpdateOverlayHandle()
        {
            if (!parent.Spawned)
                return;

            // Register/unregister the item to draw a favourite star over
            cachedFavouriteOverlayDrawer.Deregister(parent);
            
            if (parent.Spawned && Favourited)
                cachedFavouriteOverlayDrawer.Register(parent);
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

        private bool favourited;
        private FavouriteOverlayDrawer cachedFavouriteOverlayDrawer;

    }

}