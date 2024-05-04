using HarmonyLib;
using QualityBionicsContinued.Comps;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(BodyPartDef), nameof(BodyPartDef.GetMaxHealth))]
public class BodyPartDef_GetMaxHealth
{
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
