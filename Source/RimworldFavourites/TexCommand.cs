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

        public static readonly Texture2D Favourited = ContentFinder<Texture2D>.Get("UI/Designators/Favourited");
        public static readonly Texture2D Unfavourited = ContentFinder<Texture2D>.Get("UI/Designators/Unfavourited");
        public static readonly Texture2D ShowFavouritesOverlay = ContentFinder<Texture2D>.Get("UI/Buttons/ShowFavouritesOverlay");

    }
}
