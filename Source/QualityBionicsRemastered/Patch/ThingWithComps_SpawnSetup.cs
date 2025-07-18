using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved spawn setup patch that uses the new thread-safe transfer system.
/// </summary>
[HarmonyPatch(typeof(ThingWithComps), "SpawnSetup")]
public static class ThingWithComps_SpawnSetup
{
    [HarmonyPostfix]
    private static void Postfix(ThingWithComps __instance)
    {
        try
        {
            if (__instance?.def == null) return;

            // Try to apply quality from our transfer system
            if (QualityTransferManager.TryConsumeTransfer(__instance, out var quality))
            {
                if (QualityBionicsManager.TryApplyQuality(__instance, quality))
                {
                    QualityBionicsMod.Message($"Applied quality {quality} to spawned {__instance.def.label}");
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in ThingWithComps_SpawnSetup: {ex.Message}");
        }
    }
}
