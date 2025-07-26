using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to add efficiency and HP stats to bionic items based on their quality.
/// This makes the stats show up in the item description and allows proper saving/loading.
/// </summary>
[HarmonyPatch(typeof(StatUtility), "GetStatValue")]
public static class Thing_GetStatValue
{
    [HarmonyPostfix]
    private static void Postfix(ref float __result, Thing thing, StatDef stat, bool applyPostProcess)
    {
        try
        {
            if (thing?.def == null || !thing.def.isTechHediff) return;

            // Check if this thing has quality
            if (!thing.TryGetQuality(out var quality)) return;

            // Find the corresponding hediff
            var correspondingHediff = FindCorrespondingHediffDef(thing.def);
            if (correspondingHediff == null || !QualityBionicsManager.IsQualityEligible(correspondingHediff)) return;

            // Apply quality modifiers to relevant stats
            if (stat == StatDefOf.MedicalPotency || stat.defName.Contains("efficiency") || stat.defName.Contains("Efficiency"))
            {
                var baseEfficiency = QualityBionicsManager.GetBaseEfficiency(correspondingHediff);
                var qualityMultiplier = Settings.GetQualityMultipliers(quality);
                var finalEfficiency = baseEfficiency * qualityMultiplier;
                
                // Override the result with our quality-modified efficiency
                __result = finalEfficiency;
                QualityBionicsMod.Message($"Applied efficiency stat {finalEfficiency} (base: {baseEfficiency}, quality: {qualityMultiplier}) to {thing.def.defName}");
            }
            else if (stat == StatDefOf.MaxHitPoints)
            {
                var hpMultiplier = Settings.GetQualityMultipliersForHP(quality);
                __result *= hpMultiplier;
                QualityBionicsMod.Message($"Applied HP multiplier {hpMultiplier} to {thing.def.defName}, result: {__result}");
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in Thing_GetStatValue patch: {ex.Message}");
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
}
