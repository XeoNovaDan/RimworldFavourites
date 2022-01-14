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
            overlayDrawRegister = new List<Thing>();
            overlaysToDraw = new List<Thing>();
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();

            if (!WorldRendererUtility.WorldRenderedNow && Find.CurrentMap == map)
                DrawAllFavouritedOverlays();
        }

        public void DrawFavouritedOverlay(Thing t)
        {
            if (!overlaysToDraw.Contains(t))
                overlaysToDraw.Add(t);
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
                        if (!curRegister.Fogged())
                            DrawFavouritedOverlay(curRegister);
                    }
                    for (int i = 0; i < overlaysToDraw.Count; i++)
                    {
                        var curThing = overlaysToDraw[i];
                        RenderFavouritedOverlay(curThing);
                    }
                }
                finally
                {
                    overlaysToDraw.Clear();
                }


                drawBatch.Flush();
            }
            
        }

        private void RenderFavouritedOverlay(Thing t)
        {
            // Render favourited star overlay over the bottom right hand corner of the item on map
            Vector3 drawPos = t.DrawPos;
            var rotSize = t.RotatedSize;
            drawPos.z -= 0.3f * rotSize.z;
            drawPos.x += 0.3f * rotSize.x;
            drawPos.y = BaseAlt + 0.16216215f;
            drawBatch.DrawMesh(MeshPool.plane05, Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one), FavouritedMat, 0, true);
        }

        public void Register(Thing t, bool checkRegister = true)
        {
            if (!checkRegister || !overlayDrawRegister.Contains(t))
                overlayDrawRegister.Add(t);
        }

        public void Deregister(Thing t, bool checkRegister = true)
        {
            if (!checkRegister || overlayDrawRegister.Contains(t))
                overlayDrawRegister.Remove(t);
        }

        private static readonly float BaseAlt = AltitudeLayer.MetaOverlays.AltitudeFor();
        private static readonly Material FavouritedMat = MaterialPool.MatFrom("UI/Designators/Favourited", ShaderDatabase.MetaOverlay);

        public DrawBatch drawBatch;
        public List<Thing> overlaysToDraw;
        public List<Thing> overlayDrawRegister;


    }

}