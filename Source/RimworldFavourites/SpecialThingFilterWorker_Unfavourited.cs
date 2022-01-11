using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{
    public class SpecialThingFilterWorker_Unfavourited : SpecialThingFilterWorker
    {

        public override bool Matches(Thing t)
        {
            var favouriteComp = t.TryGetComp<CompFavouritable>();
            if (favouriteComp != null)
                return !favouriteComp.Favourited;
            return true;
        }

        public override bool AlwaysMatches(ThingDef def)
        {
            return !def.HasComp(typeof(CompFavouritable));
        }

    }
}
