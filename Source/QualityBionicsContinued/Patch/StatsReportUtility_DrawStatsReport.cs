using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionics;
//using QualityBionicsContinued.Comps;
using RimWorld;
using UnityEngine;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(StatsReportUtility), "DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) })]
public static class StatsReportUtility_DrawStatsReport
{
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
                        //diff.addedPartProps.partEfficiency *= QualityBionicsMod.settings.GetQualityMultipliers(qc);
                        diff.addedPartProps.partEfficiency = diff.comps.OfType<HediffCompProperties_QualityBionics>().First().baseEfficiency * Settings.GetQualityMultipliers(qc); //new - Changed the calculation to prevent infinite loops.
                    }
                }
            }

        }
    }

    private static void Postfix(Rect rect, Thing thing, Pair<HediffDef, float>? __state)
    {
        if (__state.HasValue)
        {
            __state.Value.First.addedPartProps.partEfficiency = __state.Value.Second;
        }
    }
}
