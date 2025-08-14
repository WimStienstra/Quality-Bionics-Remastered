using System.Linq;
using HarmonyLib;
using QualityBionicsRemastered.Comps;
using QualityBionicsRemastered.Core;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to apply quality HP multipliers to bionic body parts.
/// </summary>
[HarmonyPatch(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth))]
public class BodyPartDef_GetMaxHealth
{
    [HarmonyPrepare]
    private static bool ShouldPatch()
    {
        // Skip if EBF is running
        if (LoadedModManager.RunningMods.Any(m => m.PackageIdPlayerFacing == "V1024.EBFramework"))
        {
            QualityBionicsMod.WarningOnce("Skipping BodyPartDef_GetMaxHealth patch since EBF is present", 0x1337);
            return false;
        }
        return true;
    }

    [HarmonyPriority(Priority.Last)]
    private static void Postfix(BodyPartDef __instance, Pawn pawn, ref float __result)
    {
        try
        {
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Part?.def == __instance)
                {
                    var comp = hediff.TryGetComp<HediffCompQualityBionics>();
                    if (comp != null)
                    {
                        float hpMultiplier = Settings.GetQualityMultipliersForHP(comp.quality);
                        __result *= hpMultiplier;
                        __result = (int)__result;
                        
                        // QualityBionicsMod.Message($"Applied HP multiplier {hpMultiplier} for quality {comp.quality} to {__instance.defName}");
                        break;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in BodyPartDef_GetMaxHealth patch: {ex.Message}");
        }
    }
}