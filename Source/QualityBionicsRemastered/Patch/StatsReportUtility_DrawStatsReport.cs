using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionicsRemastered;
using QualityBionicsRemastered.Core;
using RimWorld;
using UnityEngine;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved stats report patch that displays quality-adjusted efficiency without modifying shared definitions.
/// </summary>
[HarmonyPatch(typeof(StatsReportUtility), "DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) })]
public static class StatsReportUtility_DrawStatsReport
{
    [HarmonyPrefix]
    private static bool Prefix(Rect rect, Thing thing)
    {
        try
        {
            // Only intercept for quality bionics
            if (!thing.def.isTechHediff || !thing.TryGetQuality(out var quality))
                return true;

            var relevantRecipes = DefDatabase<RecipeDef>.AllDefs
                .Where(recipe => recipe.addsHediff != null && recipe.IsIngredient(thing.def))
                .Where(recipe => QualityBionicsManager.IsQualityBionic(recipe.addsHediff))
                .ToList();

            if (!relevantRecipes.Any())
                return true;

            // Create a custom stats display for quality bionics
            DrawCustomStatsReport(rect, thing, quality, relevantRecipes);
            return false; // Skip original method
        }
        catch (Exception ex)
        {
            QualityBionicsMod.Warning($"Error in StatsReportUtility_DrawStatsReport: {ex.Message}");
            return true; // Fall back to original method
        }
    }

    private static void DrawCustomStatsReport(Rect rect, Thing thing, QualityCategory quality, List<RecipeDef> relevantRecipes)
    {
        GUI.BeginGroup(rect);
        var currentY = 0f;

        // Draw quality indicator
        Text.Font = GameFont.Medium;
        var qualityRect = new Rect(0f, currentY, rect.width, 30f);
        Widgets.Label(qualityRect, $"{thing.LabelCap} ({quality})");
        currentY += 35f;

        Text.Font = GameFont.Small;

        // Draw efficiency stats for each relevant recipe
        foreach (var recipe in relevantRecipes)
        {
            var hediffDef = recipe.addsHediff;
            var baseEfficiency = QualityBionicsManager.GetBaseEfficiency(hediffDef);
            var qualityMultiplier = Settings.GetQualityMultipliers(quality);
            var finalEfficiency = baseEfficiency * qualityMultiplier;

            var efficiencyRect = new Rect(0f, currentY, rect.width, 20f);
            var label = $"Part efficiency: {finalEfficiency:P0} (base: {baseEfficiency:P0}, quality: +{(qualityMultiplier - 1f):P0})";
            Widgets.Label(efficiencyRect, label);
            currentY += 25f;
        }

        GUI.EndGroup();
    }
}
