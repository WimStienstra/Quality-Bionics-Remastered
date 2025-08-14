using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using QualityBionicsRemastered.Comps;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

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
            if (hediff.spawnThingOnRemoved != null && hediff.spawnThingOnRemoved.isTechHediff && hediff.addedPartProps != null)
            {
                var defName = hediff.defName.ToLower();
                if (defName.Contains("bionic") || defName.Contains("archotech") || customHediffDefs.Contains(hediff.defName)
                        || hediff.spawnThingOnRemoved.techLevel >= Settings.minTechLevelForQuality)
                {
                    hediff.comps ??= new List<HediffCompProperties>();
                    hediff.comps.Add(new HediffCompProperties_QualityBionics() { baseEfficiency = hediff.addedPartProps.partEfficiency });
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

/// <summary>
/// Patch for RecipeWorker.ApplyOnPawn to transfer quality when installing/removing bionics.
/// Based on the original mod approach with static variables.
/// </summary>
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

    /// <summary>
    /// Before applying the recipe, check for bionic removal and store quality.
    /// </summary>
    [HarmonyPrefix]
    public static void Prefix(RecipeWorker __instance, object[] __args)
    {
        try
        {
            Pawn pawn = (Pawn)__args[0];
            BodyPartRecord part = (BodyPartRecord)__args[1];

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
                            // QualityBionicsMod.Message($"Stored quality {comp.quality} for removal of {hediff.def.spawnThingOnRemoved.defName}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            QualityBionicsMod.Warning($"Error in RecipeWorker_ApplyOnPawn prefix: {ex.Message}");
        }
    }

    /// <summary>
    /// After applying the recipe (installing bionic), transfer quality from ingredient to hediff.
    /// </summary>
    [HarmonyPostfix]
    public static void Postfix(RecipeWorker __instance, object[] __args)
    {
        try
        {
            Pawn pawn = (Pawn)__args[0];
            BodyPartRecord part = (BodyPartRecord)__args[1];
            List<Thing> ingredients = (List<Thing>)__args[3];

            thingWithQuality = null;
            
            // Only process when adding hediffs (installing bionics)
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
                                QualityBionicsMod.Message($"Quality {qualityCategory} bionic installed: {hediff.def.defName}");
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            QualityBionicsMod.Warning($"Error in RecipeWorker_ApplyOnPawn postfix: {ex.Message}");
        }
    }
}