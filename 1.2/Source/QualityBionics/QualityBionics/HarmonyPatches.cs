using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Verse.PawnCapacityUtility;

namespace QualityBionics
{
    [StaticConstructorOnStartup]
    internal static class HarmonyContainer
    {
        public static Harmony harmony;
        static HarmonyContainer()
        {
            harmony = new Harmony("QualityBionics.Mod");
            harmony.PatchAll();
            var postfix = typeof(HarmonyContainer).GetMethod("ApplyOnPawnPostfix");
            var baseType = typeof(RecipeWorker);
            var types = baseType.AllSubclassesNonAbstract();
            foreach (Type cur in types)
            {
                var method = cur.GetMethod("ApplyOnPawn");
                try
                {
                    harmony.Patch(method, null, new HarmonyMethod(postfix));
                }
                catch (Exception ex)
                {
                    //Log.Error("Error patching " + cur + " - " + method + " - " + ex);
                }
            }

            AddQualityToImplants();
        }

        public static void ApplyOnPawnPostfix(RecipeWorker __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (__instance.recipe?.addsHediff != null)
            {
                var hediff = pawn.health.hediffSet.hediffs.FindLast(x => x.def == __instance.recipe.addsHediff);
                if (hediff != null)
                {
                    var comp = hediff.TryGetComp<HediffCompQualityBionics>();
                    if (comp != null)
                    {
                        foreach (var ingredient in ingredients)
                        {
                            if (hediff.def.spawnThingOnRemoved == ingredient.def && ingredient.TryGetQuality(out var qualityCategory))
                            {
                                comp.quality = qualityCategory;
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void AddQualityToImplants()
        {
            foreach (var hediff in DefDatabase<HediffDef>.AllDefs)
            {
                if (hediff.spawnThingOnRemoved != null && hediff.spawnThingOnRemoved.isTechHediff)
                {
                    if (hediff.defName.ToLower().Contains("bionic") || hediff.defName.ToLower().Contains("archotech"))
                    {
                        if (hediff.comps is null)
                        {
                            hediff.comps = new List<HediffCompProperties>();
                        }
                        hediff.comps.Add(new HediffCompProperties_QualityBionics());
                        if (hediff.spawnThingOnRemoved.comps is null)
                        {
                            hediff.spawnThingOnRemoved.comps = new List<CompProperties>();
                        }
                        if (!hediff.spawnThingOnRemoved.comps.Any(x => x.compClass == typeof(CompQuality)))
                        {
                            hediff.spawnThingOnRemoved.comps.Add(new CompProperties { compClass = typeof(CompQuality) });
                        }
                        Log.Message("Tech hediff: " + hediff);
                    }
                }
                else if (hediff.defName.ToLower().Contains("bionic") || hediff.defName.ToLower().Contains("archotech"))
                {
                    Log.Error(hediff + " isn't accounted as quality bionic");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Hediff_AddedPart), "TipStringExtra", MethodType.Getter)]
    public class TipStringExtra_Patch
    {
        private static void Prefix(Hediff_AddedPart __instance, out float? __state)
        {
            __state = null;
            if (__instance.def.addedPartProps != null)
            {
                var comp = __instance.TryGetComp<HediffCompQualityBionics>();
                if (comp != null)
                {
                    __state = __instance.def.addedPartProps.partEfficiency;
                    __instance.def.addedPartProps.partEfficiency *= QualityBionicsMod.settings.GetQualityMultipliers(comp.quality);
                }
            }
        }

        private static void Postfix(Hediff_AddedPart __instance, float? __state)
        {
            if (__state.HasValue)
            {
                __instance.def.addedPartProps.partEfficiency = __state.Value;
            }
        }
    }

    [HarmonyPatch(typeof(PawnCapacityUtility), "CalculatePartEfficiency")]
    public class CalculatePartEfficiency_Patch
    {
        private static void Prefix(out Pair<Hediff, float>? __state, HediffSet diffSet, BodyPartRecord part, bool ignoreAddedParts = false, List<CapacityImpactor> impactors = null)
        {
            __state = null;
            BodyPartRecord rec;
            for (rec = part.parent; rec != null; rec = rec.parent)
            {
                if (diffSet.HasDirectlyAddedPartFor(rec))
                {
                    Hediff_AddedPart hediff_AddedPart = (from x in diffSet.GetHediffs<Hediff_AddedPart>()
                                                         where x.Part == rec
                                                         select x).First();
                    if (hediff_AddedPart != null)
                    {
                        if (hediff_AddedPart.def.addedPartProps != null)
                        {
                            var comp = hediff_AddedPart.TryGetComp<HediffCompQualityBionics>();
                            if (comp != null)
                            {
                                __state = new Pair<Hediff, float>(hediff_AddedPart, hediff_AddedPart.def.addedPartProps.partEfficiency);
                                hediff_AddedPart.def.addedPartProps.partEfficiency *= QualityBionicsMod.settings.GetQualityMultipliers(comp.quality);
                                return;
                            }
                        }
                    }
                }
            }
            if (part.parent != null && diffSet.PartIsMissing(part.parent))
            {
                return;
            }
            if (!ignoreAddedParts)
            {
                for (int i = 0; i < diffSet.hediffs.Count; i++)
                {
                    Hediff_AddedPart hediff_AddedPart2 = diffSet.hediffs[i] as Hediff_AddedPart;
                    if (hediff_AddedPart2 != null && hediff_AddedPart2.Part == part)
                    {
                        if (hediff_AddedPart2 != null)
                        {
                            if (hediff_AddedPart2.def.addedPartProps != null)
                            {
                                var comp = hediff_AddedPart2.TryGetComp<HediffCompQualityBionics>();
                                if (comp != null)
                                {
                                    __state = new Pair<Hediff, float>(hediff_AddedPart2, hediff_AddedPart2.def.addedPartProps.partEfficiency);
                                    hediff_AddedPart2.def.addedPartProps.partEfficiency *= QualityBionicsMod.settings.GetQualityMultipliers(comp.quality);
                                    return;
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        private static void Postfix(Pair<Hediff, float>? __state, HediffSet diffSet, BodyPartRecord part, bool ignoreAddedParts = false, List<CapacityImpactor> impactors = null)
        {
            if (__state.HasValue)
            {
                __state.Value.First.def.addedPartProps.partEfficiency = __state.Value.Second;
            }
        }
    }


    [HarmonyPatch(typeof(StatsReportUtility), "DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) })]
    public static class DrawStatsReport_Patch
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

                        if (diff.comps.Any(x => x.GetType() == typeof(HediffCompProperties_QualityBionics)) && diff.addedPartProps != null)
                        {
                            __state = new Pair<HediffDef, float>(diff, diff.addedPartProps.partEfficiency);
                            diff.addedPartProps.partEfficiency *= QualityBionicsMod.settings.GetQualityMultipliers(qc);
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
}
