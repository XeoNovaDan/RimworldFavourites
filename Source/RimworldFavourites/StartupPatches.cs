using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{

    [StaticConstructorOnStartup]
    public static class StartupPatches
    {

        static StartupPatches()
        {
            // Patch all ThingDefs that have comps to add CompFavouritable
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var curDef = thingDefs[i];
                if (typeof(ThingWithComps).IsAssignableFrom(curDef.thingClass))
                {
                    if (curDef.comps == null)
                        curDef.comps = new List<CompProperties>();
                    curDef.comps.Add(new CompProperties() { compClass=typeof(CompFavouritable)} );
                }
            }
        }

    }

}