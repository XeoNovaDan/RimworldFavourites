using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace RimworldFavourites
{
    public class RimworldFavouritesSettings : ModSettings
    {

        public bool autoFavourite = true;
        public bool autoFavouriteManufacturedThings = true;
        public bool autoFavouriteConstructedThings = true;
        public QualityRange autoFavouriteProdtctQualityRange = new QualityRange(QualityCategory.Masterwork, QualityCategory.Legendary);
        public bool autoFavouriteQuestRewards = true;
        public bool autoFavouriteQuestRewardRawMaterials = false;
        public bool autoFavouriteQuestRewardPawns = true;
        public bool autoFavouriteQuestRewardPawnsUnrelated = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref autoFavourite, "autoFavourite", true);
            Scribe_Values.Look(ref autoFavouriteManufacturedThings, "autoFavouriteManufacturedThings", true);
            Scribe_Values.Look(ref autoFavouriteConstructedThings, "autoFavouriteConstructedThings", true);
            Scribe_Values.Look(ref autoFavouriteProdtctQualityRange, "autoFavouriteManufacturedQualityRange", new QualityRange(QualityCategory.Masterwork, QualityCategory.Legendary));
            Scribe_Values.Look(ref autoFavouriteQuestRewards, "autoFavouriteQuestRewards", true);
            Scribe_Values.Look(ref autoFavouriteQuestRewardRawMaterials, "autoFavouriteQuestRewardRawMaterials", false);
            Scribe_Values.Look(ref autoFavouriteQuestRewardPawns, "autoFavouriteQuestRewardPawns", true);
            Scribe_Values.Look(ref autoFavouriteQuestRewardPawnsUnrelated, "autoFavouriteQuestRewardPawnsUnrelated", false);
            base.ExposeData();
        }

    }
}
