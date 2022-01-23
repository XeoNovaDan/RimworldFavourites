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
        public static RimworldFavouritesSettings settings;

        public RimworldFavourites(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("XeoNovaDan.RimworldFavourites");
            settings = GetSettings<RimworldFavouritesSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var drawSettings = new Listing_Standard();
            drawSettings.Begin(inRect.LeftHalf());
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavourite".Translate(), ref settings.autoFavourite, "RimworldFavourites.Settings_AutoFavourite_ToolTip".Translate());
            drawSettings.GapLine();

            #region Favourite Toggles
            // Auto favourite manufactured things + quality range
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteManufacturedThings".Translate(), ref settings.autoFavouriteManufacturedThings, "RimworldFavourites.Settings_AutoFavouriteManufacturedThings_ToolTip".Translate());
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteConstructedThings".Translate(), ref settings.autoFavouriteConstructedThings, "RimworldFavourites.Settings_AutoFavouriteConstructedThings_ToolTip".Translate());
            Widgets.QualityRange(drawSettings.GetRect(Widgets.RangeControlCompactHeight), 1342, ref settings.autoFavouriteProductQualityRange);
            drawSettings.GapLine();

            // Auto favourite quest rewards
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteQuestRewards".Translate(), ref settings.autoFavouriteQuestRewards, "RimworldFavourites.Settings_AutoFavouriteQuestRewards_ToolTip".Translate());
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteQuestRewardRawMaterials".Translate(), ref settings.autoFavouriteQuestRewardRawMaterials, "RimworldFavourites.Settings_AutoFavouriteQuestRewardRawMaterials_ToolTip".Translate());
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteQuestRewardPawns".Translate(), ref settings.autoFavouriteQuestRewardPawns, "RimworldFavourites.Settings_AutoFavouriteQuestRewardPawns_ToolTip".Translate());
            drawSettings.CheckboxLabeled("RimworldFavourites.Settings_AutoFavouriteQuestRewardPawnsUnrelated".Translate(), ref settings.autoFavouriteQuestRewardPawnsUnrelated, "RimworldFavourites.Settings_AutoFavouriteQuestRewardPawnsUnrelated_ToolTip".Translate());
            #endregion

            drawSettings.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RimworldFavourites.SettingsCategoryTitle".Translate();
        }

    }
}
