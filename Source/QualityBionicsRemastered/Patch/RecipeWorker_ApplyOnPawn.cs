using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using QualityBionics;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved system initialization that's more robust and mod-compatible.
/// </summary>
[StaticConstructorOnStartup]
internal static class AddQualityToImplants
{
    static AddQualityToImplants()
    {
        // The actual initialization is now handled by QualityBionicsManager
        // This class is kept for compatibility but delegates to the new system
        QualityBionicsMod.Message("Quality implant system delegated to QualityBionicsManager");
    }
}

/// <summary>
/// Improved RecipeWorker patch that uses the new thread-safe transfer system.
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
            var method = cur.GetMethod(nameof(RecipeWorker.ApplyOnPawn));
            if (method != null)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Handle quality transfer when removing bionics.
    /// </summary>
    public static void Prefix(RecipeWorker __instance, object[] __args)
    {
        try
        {
            if (__args.Length < 2) return;

            var pawn = __args[0] as Pawn;
            var part = __args[1] as BodyPartRecord;

            if (pawn?.health?.hediffSet == null || part == null) return;

            // Handle removal of quality bionics
            if (__instance.recipe?.removesHediff != null)
            {
                HandleBionicRemoval(pawn, part, __instance.recipe.removesHediff);
            }
        }
        catch (Exception ex)
        {
            QualityBionicsMod.Warning($"Error in RecipeWorker Prefix: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle quality transfer when installing bionics.
    /// </summary>
    public static void Postfix(RecipeWorker __instance, object[] __args)
    {
        try
        {
            if (__args.Length < 4) return;

            var pawn = __args[0] as Pawn;
            var part = __args[1] as BodyPartRecord;
            var ingredients = __args[3] as List<Thing>;

            if (pawn?.health?.hediffSet == null || part == null) return;

            // Handle installation of quality bionics
            if (__instance.recipe?.addsHediff != null && ingredients != null)
            {
                HandleBionicInstallation(pawn, part, __instance.recipe.addsHediff, ingredients);
            }
        }
        catch (Exception ex)
        {
            QualityBionicsMod.Warning($"Error in RecipeWorker Postfix: {ex.Message}");
        }
    }

    private static void HandleBionicRemoval(Pawn pawn, BodyPartRecord part, HediffDef removedHediffDef)
    {
        if (!pawn.health.hediffSet.GetNotMissingParts().Contains(part)) return;

        var hediff = pawn.health.hediffSet.hediffs.FirstOrDefault(x => x.def == removedHediffDef);
        if (hediff?.def?.spawnThingOnRemoved == null) return;

        var qualityComp = hediff.TryGetComp<HediffCompQualityBionics>();
        if (qualityComp != null)
        {
            // Register the quality transfer for when the thing spawns
            QualityTransferManager.RegisterTransfer(hediff.def.spawnThingOnRemoved, qualityComp.quality);
        }
    }

    private static void HandleBionicInstallation(Pawn pawn, BodyPartRecord part, HediffDef addedHediffDef, List<Thing> ingredients)
    {
        // Find the most recently added hediff of the correct type
        var hediff = pawn.health.hediffSet.hediffs.FindLast(x => x.def == addedHediffDef);
        if (hediff == null) return;

        var qualityComp = hediff.TryGetComp<HediffCompQualityBionics>();
        if (qualityComp == null) return;

        // Find a quality ingredient to transfer quality from
        foreach (var ingredient in ingredients)
        {
            if (ingredient?.def == null) continue;

            // Check if this ingredient matches the bionic's spawned thing
            if (hediff.def.spawnThingOnRemoved == ingredient.def && 
                ingredient.TryGetQuality(out var qualityCategory))
            {
                qualityComp.quality = qualityCategory;
                QualityBionicsMod.Message($"Applied quality {qualityCategory} to installed {hediff.def.label}");
                break;
            }
        }
    }
}
