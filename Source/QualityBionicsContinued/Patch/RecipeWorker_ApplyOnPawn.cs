using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using QualityBionicsContinued.Comps;
using RimWorld;
using Verse;

namespace QualityBionicsContinued.Patch;

[StaticConstructorOnStartup]
internal static class AddQualityToImplants
{
    private static HashSet<string> customHediffDefs = new HashSet<string>
    {
        "synthetic",
        "cybernetic",
    };

    static AddQualityToImplants()
    {
        foreach (var hediff in DefDatabase<HediffDef>.AllDefs)
        {
            //if (hediff.spawnThingOnRemoved != null && hediff.spawnThingOnRemoved.isTechHediff)
            if (hediff.spawnThingOnRemoved != null && hediff.spawnThingOnRemoved.isTechHediff && hediff.addedPartProps != null) //new - Changed so only Hediffs with Effifiencies get the quality added.
            {
                var defName = hediff.defName.ToLower();
                if (defName.Contains("bionic") || defName.Contains("archotech") || customHediffDefs.Contains(hediff.defName)
                        || hediff.spawnThingOnRemoved.techLevel >= Settings.minTechLevelForQuality)
                {
                    hediff.comps ??= new List<HediffCompProperties>();
                    hediff.comps.Add(new HediffCompProperties_QualityBionics() { baseEfficiency = hediff.addedPartProps.partEfficiency }); //new - Added the base Efficiency to the Property to calculate from.
                    hediff.spawnThingOnRemoved.comps ??= new List<CompProperties>();
                    if (!hediff.spawnThingOnRemoved.comps.Any(x => x.compClass == typeof(CompQuality)))
                    {
                        hediff.spawnThingOnRemoved.comps.Add(new CompProperties { compClass = typeof(CompQuality) });
                    }
                }
            }
        }
    }
}

[HarmonyPatch]
internal static class RecipeWorker_ApplyOnPawn
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        var baseType = typeof(RecipeWorker);
        var types = baseType.AllSubclassesNonAbstract();

        foreach (Type cur in types)
        {
            yield return cur.GetMethod(nameof(RecipeWorker.ApplyOnPawn));
        }
    }

    public static Pair<ThingDef, QualityCategory>? thingWithQuality;

    public static void Prefix(RecipeWorker __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (__instance.recipe?.removesHediff != null)
        {
            if (!pawn.health?.hediffSet?.GetNotMissingParts().Contains(part) ?? false)
            {
                return;
            }
            Hediff? hediff = pawn.health?.hediffSet?.hediffs?.FirstOrDefault(x => x.def == __instance.recipe.removesHediff);
            if (hediff != null)
            {
                if (hediff.def.spawnThingOnRemoved != null)
                {
                    var comp = hediff.TryGetComp<HediffCompQualityBionics>();
                    if (comp != null)
                    {
                        thingWithQuality = new Pair<ThingDef, QualityCategory>(hediff.def.spawnThingOnRemoved, comp.quality);
                    }
                }
            }
        }


    }
    public static void Postfix(RecipeWorker __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        thingWithQuality = null;
        if (__instance.recipe?.addsHediff != null && ingredients != null)
        {
            var hediff = pawn.health?.hediffSet?.hediffs?.FindLast(x => x.def == __instance.recipe.addsHediff);
            if (hediff != null)
            {
                var comp = hediff.TryGetComp<HediffCompQualityBionics>();
                if (comp != null)
                {
                    foreach (var ingredient in ingredients)
                    {
                        if (ingredient != null && hediff.def.spawnThingOnRemoved == ingredient.def && ingredient.TryGetQuality(out var qualityCategory))
                        {
                            comp.quality = qualityCategory;
                            break;
                        }
                    }
                }
            }
        }
    }
}
