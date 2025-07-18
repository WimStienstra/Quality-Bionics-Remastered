using HarmonyLib;
using QualityBionicsRemastered;
using QualityBionicsRemastered.Core;
using Verse;

namespace QualityBionicsRemastered.Patch;

[HarmonyPatch(typeof(Hediff), "GetTooltip")]
public static class Hediff_GetTooltip
{
    [HarmonyPostfix]
    private static void Postfix(ref string __result, Hediff __instance)
    {
        try
        {
            if (!(__instance is Hediff_AddedPart addedPart)) return;

            var quality = QualityBionicsManager.GetQualityFromHediff(addedPart);
            if (quality == null) return;

            var baseEfficiency = QualityBionicsManager.GetBaseEfficiency(addedPart.def);
            var qualityMultiplier = Settings.GetQualityMultipliers(quality.Value);
            var finalEfficiency = baseEfficiency * qualityMultiplier;

            var qualityInfo = $"\n\nQuality: {quality.Value}" +
                            $"\nBase efficiency: {baseEfficiency:P0}" +
                            $"\nQuality modifier: +{(qualityMultiplier - 1f):P0}" +
                            $"\nFinal efficiency: {finalEfficiency:P0}";

            __result += qualityInfo;
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in Hediff_GetTooltip: {ex.Message}");
        }
    }
}
