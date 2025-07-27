using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Spawn setup patch that applies quality from removal operations.
/// Based on the original mod approach.
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

            // Check if this matches a stored removal quality
            if (RecipeWorker_ApplyOnPawn.thingWithQuality.HasValue && __instance.def == RecipeWorker_ApplyOnPawn.thingWithQuality.Value.First)
            {
                var comp = __instance.TryGetComp<CompQuality>();
                if (comp != null)
                {
                    comp.SetQuality(RecipeWorker_ApplyOnPawn.thingWithQuality.Value.Second, ArtGenerationContext.Colony);
                    QualityBionicsMod.Message($"Quality {RecipeWorker_ApplyOnPawn.thingWithQuality.Value.Second} bionic extracted: {__instance.def.label}");
                    RecipeWorker_ApplyOnPawn.thingWithQuality = null;
                }
            }

            // Check for medical recipes quality transfer
            if (MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities != null)
            {
                var pair = MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities.FirstOrDefault(x => x.HasValue && x.Value.First == __instance.def);
                if (pair.HasValue)
                {
                    var comp = __instance.TryGetComp<CompQuality>();
                    if (comp != null)
                    {
                        comp.SetQuality(pair.Value.Second, ArtGenerationContext.Colony);
                        // QualityBionicsMod.Message($"Applied medical quality {pair.Value.Second} to spawned {__instance.def.label}");
                        MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities.Remove(pair);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in ThingWithComps_SpawnSetup: {ex.Message}");
        }
    }
}
