using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to make bionic things display their quality in the label.
/// DISABLED: Since we properly add CompQuality to ThingDefs, RimWorld's built-in quality labeling works automatically.
/// </summary>
// [HarmonyPatch(typeof(Thing), "LabelNoCount", MethodType.Getter)]
public static class Thing_LabelNoCount
{
    // [HarmonyPostfix]
    private static void Postfix(ref string __result, Thing __instance)
    {
        // This patch is disabled because RimWorld's built-in quality system
        // now handles the labeling automatically since we properly add CompQuality to ThingDefs
        return;
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
