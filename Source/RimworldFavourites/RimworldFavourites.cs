using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace RimworldFavourites
{
    public class RimworldFavourites : Mod
    {

        public static Harmony harmonyInstance;

        public RimworldFavourites(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("XeoNovaDan.RimworldFavourites");
        }

    }
}
