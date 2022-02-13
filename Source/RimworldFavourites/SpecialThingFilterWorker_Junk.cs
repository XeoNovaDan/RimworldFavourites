using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{
    public class SpecialThingFilterWorker_Junk : SpecialThingFilterWorker
    {

        public override bool Matches(Thing t)
        {
            var favouriteComp = t.TryGetComp<CompFavouritable>();
            if (favouriteComp != null)
                return favouriteComp.Junk;
            return false;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp(typeof(CompFavouritable));
        }

    }
}
