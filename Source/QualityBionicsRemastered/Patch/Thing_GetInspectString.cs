using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using System.Text;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to add quality efficiency information to bionic thing tooltips.
/// </summary>
[HarmonyPatch(typeof(Thing), nameof(Thing.GetInspectString))]
public static class Thing_GetInspectString
{
    [HarmonyPostfix]
    private static void Postfix(ref string __result, Thing __instance)
    {
        try
        {
            if (__instance?.def == null || !__instance.def.isTechHediff) return;

            // Check if this thing has quality and corresponds to a quality-eligible bionic
            if (!__instance.TryGetQuality(out var quality)) return;

            var correspondingHediff = FindCorrespondingHediffDef(__instance.def);
            if (correspondingHediff == null || !QualityBionicsManager.IsQualityEligible(correspondingHediff)) return;

            // Calculate efficiency information
            var baseEfficiency = QualityBionicsManager.GetBaseEfficiency(correspondingHediff);
            var qualityMultiplier = Settings.GetQualityMultipliers(quality);
            var finalEfficiency = baseEfficiency * qualityMultiplier;
            var hpMultiplier = Settings.GetQualityMultipliersForHP(quality);

            var sb = new StringBuilder(__result);
            if (sb.Length > 0) sb.AppendLine();

            // Add efficiency information
            sb.AppendLine($"Part efficiency: {(finalEfficiency * 100f):F0}%");
            
            if (qualityMultiplier != 1f)
            {
                sb.AppendLine($"Quality bonus: {((qualityMultiplier - 1f) * 100f):+0;-0;0}%");
            }

            if (hpMultiplier != 1f)
            {
                sb.AppendLine($"HP multiplier: {(hpMultiplier * 100f):F0}%");
            }

            __result = sb.ToString().TrimEnd();
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in Thing_GetInspectString patch: {ex.Message}");
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
