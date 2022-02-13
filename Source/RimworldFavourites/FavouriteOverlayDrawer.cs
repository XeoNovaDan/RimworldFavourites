using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimworldFavourites
{

    [StaticConstructorOnStartup]
    public class FavouriteOverlayDrawer : MapComponent
    {

        public FavouriteOverlayDrawer(Map map) : base(map)
        {
            drawBatch = new DrawBatch();
            overlayDrawRegister = new List<Pair<Thing, CompFavouritable>>();
            overlaysToDraw = new List<Pair<Thing, CompFavouritable>>();
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();

            if (!WorldRendererUtility.WorldRenderedNow && Find.CurrentMap == map)
                DrawAllFavouritedOverlays();
        }

        public void DrawFavouritedOverlay(Pair<Thing, CompFavouritable> p)
        {
            if (!overlaysToDraw.Contains(p))
                overlaysToDraw.Add(p);
        }

        public void DrawAllFavouritedOverlays()
        {

            if (PlaySettings.showFavouritesOverlay)
            {
                // Almost 1:1 structure with the vanilla Overlay Drawer code

                try
                {
                    for (int i = 0; i < overlayDrawRegister.Count; i++)
                    {
                        var curRegister = overlayDrawRegister[i];
                        if (!curRegister.First.Fogged())
                            DrawFavouritedOverlay(curRegister);
                    }
                    for (int i = 0; i < overlaysToDraw.Count; i++)
                    {
                        var p = overlaysToDraw[i];
                        RenderFavouritedOverlay(p);
                    }
                }
                finally
                {
                    overlaysToDraw.Clear();
                }


                drawBatch.Flush();
            }
            
        }

        private void RenderFavouritedOverlay(Pair<Thing, CompFavouritable> p)
        {
            // Render favourited star overlay over the bottom right hand corner of the item on map
            var thing = p.First;
            Vector3 drawPos = thing.DrawPos;
            var rotSize = thing.RotatedSize;
            drawPos.z -= 0.3f * rotSize.z;
            drawPos.x += 0.3f * rotSize.x;
            drawPos.y = BaseAlt + 0.16216215f;
            drawBatch.DrawMesh(MeshPool.plane05, Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one), p.Second.MatToDraw, 0, true);
        }

        public void Register(Thing t, CompFavouritable c, bool checkRegister = true)
        {
            var p = new Pair<Thing, CompFavouritable>(t, c);
            if (!checkRegister || !overlayDrawRegister.Contains(p))
                overlayDrawRegister.Add(p);
        }

        public void Deregister(Thing t, CompFavouritable c, bool checkRegister = true)
        {
            var p = new Pair<Thing, CompFavouritable>(t, c);
            if (!checkRegister || overlayDrawRegister.Contains(p))
                overlayDrawRegister.Remove(p);
        }

        private static readonly float BaseAlt = AltitudeLayer.MetaOverlays.AltitudeFor();

        public static readonly Material FavMatYellow = MaterialPool.MatFrom("UI/Designators/FavouriteStar", ShaderDatabase.MetaOverlay, Color.yellow);
        public static readonly Material FavMatRed = MaterialPool.MatFrom("UI/Designators/FavouriteStar", ShaderDatabase.MetaOverlay, Color.red);
        public static readonly Material FavMatGreen = MaterialPool.MatFrom("UI/Designators/FavouriteStar", ShaderDatabase.MetaOverlay, Color.green);
        public static readonly Material FavMatCyan = MaterialPool.MatFrom("UI/Designators/FavouriteStar", ShaderDatabase.MetaOverlay, Color.cyan);
        public static readonly Material FavMatMagenta = MaterialPool.MatFrom("UI/Designators/FavouriteStar", ShaderDatabase.MetaOverlay, Color.magenta);

        public static readonly Material JunkMatYellow = MaterialPool.MatFrom("UI/Designators/JunkBin", ShaderDatabase.MetaOverlay, Color.yellow);
        public static readonly Material JunkMatRed = MaterialPool.MatFrom("UI/Designators/JunkBin", ShaderDatabase.MetaOverlay, Color.red);
        public static readonly Material JunkMatGreen = MaterialPool.MatFrom("UI/Designators/JunkBin", ShaderDatabase.MetaOverlay, Color.green);
        public static readonly Material JunkMatCyan = MaterialPool.MatFrom("UI/Designators/JunkBin", ShaderDatabase.MetaOverlay, Color.cyan);
        public static readonly Material JunkMatMagenta = MaterialPool.MatFrom("UI/Designators/JunkBin", ShaderDatabase.MetaOverlay, Color.magenta);

        public DrawBatch drawBatch;
        public List<Pair<Thing, CompFavouritable>> overlaysToDraw;
        public List<Pair<Thing, CompFavouritable>> overlayDrawRegister;


    }

}