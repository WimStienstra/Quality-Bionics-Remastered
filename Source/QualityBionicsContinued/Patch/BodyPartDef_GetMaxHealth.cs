using System.Linq;
using HarmonyLib;
using QualityBionics;
//using QualityBionicsContinued.Comps;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth))]
public class BodyPartDef_GetMaxHealth
{
    [HarmonyPrepare]
    private static bool ShouldPatch()
    {
        // Skip if EBF is running
        QualityBionicsMod.WarningOnce("Skipping BodyPartDef_GetMaxHealth patch since EBF is present", 0x1337 + 0x69 - 0x420 + 0x1986);
        return !LoadedModManager.RunningMods.Any(m => m.PackageIdPlayerFacing == "V1024.EBFramework");
    }

    [HarmonyPriority(Priority.Last)]
    private static void Postfix(BodyPartDef __instance, Pawn pawn, ref float __result)
    {
        foreach (var hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff.Part?.def == __instance)
            {
                var comp = hediff.TryGetComp<HediffCompQualityBionics>();
                if (comp != null)
                {
                    __result *= Settings.GetQualityMultipliersForHP(comp.quality);
                    __result = (int)__result;
                }
            }
        }
    }
}
