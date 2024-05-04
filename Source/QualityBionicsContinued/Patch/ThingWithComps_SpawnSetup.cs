using HarmonyLib;
using RimWorld;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(ThingWithComps), "SpawnSetup")]
public class ThingWithComps_SpawnSetup
{
    private static void Postfix(ThingWithComps __instance)
    {
        if (RecipeWorker_ApplyOnPawn.thingWithQuality.HasValue && __instance.def == RecipeWorker_ApplyOnPawn.thingWithQuality.Value.First)
        {
            var comp = __instance.TryGetComp<CompQuality>();
            if (comp != null)
            {
                comp.SetQuality(RecipeWorker_ApplyOnPawn.thingWithQuality.Value.Second, ArtGenerationContext.Colony);
                RecipeWorker_ApplyOnPawn.thingWithQuality = null;
            }
        }
        if (MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities != null)
        {
            var pair = MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities.FirstOrDefault(x => x.HasValue && x.Value.First == __instance.def);
            if (pair.HasValue)
            {
                var comp = __instance.TryGetComp<CompQuality>();
                if (comp != null)
                {
                    comp.SetQuality(pair.Value.Second, ArtGenerationContext.Colony);
                    MedicalRecipesUtility_SpawnThingsFromHediffs.thingsWithQualities.Remove(pair);
                }
            }
        }
    }
}
