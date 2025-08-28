using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to assign random quality to newly created bionic things.
/// This ensures dev-spawned items and items on the floor get quality.
/// </summary>
[HarmonyPatch(typeof(Thing), nameof(Thing.PostMake))]
public static class Thing_PostMake
{
    [HarmonyPostfix]
    private static void Postfix(Thing __instance)
    {
        try
        {
            if (__instance?.def == null) return;
            
            // Early exit if not on main thread to avoid Unity threading issues
            if (!UnityEngine.Application.isPlaying) return;

            // Only process things that are tech hediffs
            if (!__instance.def.isTechHediff) return;

            // Check if this thing corresponds to a quality-eligible bionic
            var correspondingHediff = FindCorrespondingHediffDef(__instance.def);
            if (correspondingHediff == null || !QualityBionicsManager.IsQualityEligible(correspondingHediff)) return;

            // Check if it already has quality (from transfer system)
            if (__instance.TryGetQuality(out var existingQuality)) return;
            
            // Check if the thing has CompQuality component
            var hasCompQuality = __instance.HasComp<CompQuality>();
            if (!hasCompQuality) return;

            // Generate random quality and apply it
            var randomQuality = GenerateRandomQuality();
            
            if (QualityBionicsManager.TryApplyQuality(__instance, randomQuality))
            {
                // QualityBionicsMod.Message($"Successfully applied quality {randomQuality} to {__instance.def.label}");
            }
            else
            {
                QualityBionicsMod.Warning($"Failed to apply quality {randomQuality} to {__instance.def.label}");
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in Thing_PostMake patch for {__instance?.def?.defName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Find the HediffDef that would spawn this ThingDef when removed.
    /// </summary>
    private static HediffDef? FindCorrespondingHediffDef(ThingDef thingDef)
    {
        foreach (var hediffDef in DefDatabase<HediffDef>.AllDefs)
        {
            if (hediffDef.spawnThingOnRemoved == thingDef)
                return hediffDef;
        }
        return null;
    }

    /// <summary>
    /// Generate random quality using RimWorld's standard quality generation.
    /// </summary>
    private static QualityCategory GenerateRandomQuality()
    {
        // Use a simple random distribution for quality
        // This gives a reasonable spread of qualities
        var random = Rand.Value;
        
        if (random < 0.05f) return QualityCategory.Awful;
        if (random < 0.15f) return QualityCategory.Poor;
        if (random < 0.60f) return QualityCategory.Normal;
        if (random < 0.85f) return QualityCategory.Good;
        if (random < 0.95f) return QualityCategory.Excellent;
        if (random < 0.99f) return QualityCategory.Masterwork;
        return QualityCategory.Legendary;
    }
}
