using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{
    [StaticConstructorOnStartup]
    public static class TexCommand
    {

        public static readonly Texture2D FavouriteStar = ContentFinder<Texture2D>.Get("UI/Designators/FavouriteStar");
        public static readonly Texture2D JunkBin = ContentFinder<Texture2D>.Get("UI/Designators/JunkBin");
        public static readonly Texture2D ShowFavouritesOverlay = ContentFinder<Texture2D>.Get("UI/Buttons/ShowFavouritesOverlay");

    }
}
