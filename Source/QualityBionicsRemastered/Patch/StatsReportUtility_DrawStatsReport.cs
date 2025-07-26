using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionics;
using QualityBionicsRemastered.Core;
using RimWorld;
using UnityEngine;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to temporarily modify efficiency values during stats display to show quality-adjusted values.
/// DISABLED: This was interfering with the normal stats display. Quality info is now shown in tooltips only.
/// Based on the working version from Quality-Bionics-Continued-main.
/// </summary>
// [HarmonyPatch(typeof(StatsReportUtility), "DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) })]
public static class StatsReportUtility_DrawStatsReport
{
    // [HarmonyPrefix]
    private static void Prefix(Rect rect, Thing thing, out Pair<HediffDef, float>? __state)
    {
        __state = null;
        if (thing.def.isTechHediff)
        {
            if (thing.TryGetQuality(out var qc))
            {
                IEnumerable<RecipeDef> enumerable = DefDatabase<RecipeDef>.AllDefs.Where((RecipeDef x) => x.addsHediff != null && x.IsIngredient(thing.def));
                foreach (RecipeDef item6 in enumerable)
                {
                    HediffDef diff = item6.addsHediff;

                    if ((diff.comps?.Any(x => x?.GetType() == typeof(HediffCompProperties_QualityBionics)) ?? false) && diff.addedPartProps != null)
                    {
                        __state = new Pair<HediffDef, float>(diff, diff.addedPartProps.partEfficiency);
                        diff.addedPartProps.partEfficiency = diff.comps.OfType<HediffCompProperties_QualityBionics>().First().baseEfficiency * Settings.GetQualityMultipliers(qc);
                        QualityBionicsMod.Message($"Applied quality {qc} to stats display for {diff.defName}: efficiency now {diff.addedPartProps.partEfficiency:P0}");
                    }
                }
            }
        }
    }

    // [HarmonyPostfix]
    private static void Postfix(Rect rect, Thing thing, Pair<HediffDef, float>? __state)
    {
        if (__state.HasValue)
        {
            __state.Value.First.addedPartProps.partEfficiency = __state.Value.Second;
        }
    }
}
