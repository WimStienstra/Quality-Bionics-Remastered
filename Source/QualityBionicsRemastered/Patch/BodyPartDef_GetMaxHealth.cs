using System.Linq;
using HarmonyLib;
using QualityBionicsRemastered;
using QualityBionicsRemastered.Core;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved health calculation patch that applies quality multipliers to bionic health.
/// </summary>
[HarmonyPatch(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth))]
public static class BodyPartDef_GetMaxHealth
{
    [HarmonyPrepare]
    private static bool ShouldPatch()
    {
        // Skip if EBF is running to avoid conflicts
        var hasEBF = LoadedModManager.RunningMods.Any(m => m.PackageIdPlayerFacing == "V1024.EBFramework");
        if (hasEBF)
        {
            QualityBionicsMod.WarningOnce("Skipping BodyPartDef_GetMaxHealth patch since EBF is present", 0x1337 + 0x69 - 0x420 + 0x1986);
        }
        return !hasEBF;
    }

    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    private static void Postfix(BodyPartDef __instance, Pawn pawn, ref float __result)
    {
        try
        {
            if (pawn?.health?.hediffSet?.hediffs == null) return;

            // Look for quality bionics on this body part
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Part?.def != __instance) continue;

                var quality = QualityBionicsManager.GetQualityFromHediff(hediff);
                if (quality != null)
                {
                    var multiplier = Settings.GetQualityMultipliersForHP(quality.Value);
                    __result *= multiplier;
                    __result = (int)__result; // Keep as integer for consistency

                    QualityBionicsMod.Message($"Applied health multiplier {multiplier:F2} to {__instance.label} ({quality.Value})");
                    break; // Only apply the first quality bionic found on this part
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in BodyPartDef_GetMaxHealth: {ex.Message}");
        }
    }
}
