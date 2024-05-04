using HarmonyLib;
using QualityBionicsContinued.Comps;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(Hediff), "GetTooltip")]
public static class Hediff_GetTooltip
{
    private static void Prefix(out Pair<Hediff, float>? __state, Hediff __instance)
    {
        __state = null;
        if (__instance is Hediff_AddedPart addedPart && addedPart.TryGetComp<HediffCompQualityBionics>(out var comp))
        {
            __state = new Pair<Hediff, float>(addedPart, addedPart.def.addedPartProps.partEfficiency);
            addedPart.def.addedPartProps.partEfficiency = comp.Props.baseEfficiency * Settings.GetQualityMultipliers(comp.quality);
        }
    }

    private static void Postfix(Pair<Hediff, float>? __state)
    {
        if (__state.HasValue)
        {
            __state.Value.First.def.addedPartProps.partEfficiency = __state.Value.Second;
        }
    }
}
