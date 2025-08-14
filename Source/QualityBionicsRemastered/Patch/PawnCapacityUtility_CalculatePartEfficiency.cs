using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionicsRemastered.Comps;
using QualityBionicsRemastered.Core;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Patch to apply quality modifiers to bionic part efficiency calculations.
/// This temporarily modifies the efficiency during calculation and restores it afterward.
/// Based on the working version from Quality-Bionics-Continued-main.
/// </summary>
[HarmonyPatch(typeof(PawnCapacityUtility), "CalculatePartEfficiency")]
public class PawnCapacityUtility_CalculatePartEfficiency
{
    private static void Prefix(out Pair<Hediff, float>? __state, HediffSet diffSet, BodyPartRecord part, bool ignoreAddedParts = false, List<PawnCapacityUtility.CapacityImpactor>? impactors = null)
    {
        __state = null;
        BodyPartRecord rec;
        for (rec = part.parent; rec != null; rec = rec.parent)
        {
            if (diffSet.HasDirectlyAddedPartFor(rec))
            {
                List<Hediff_AddedPart> a = new List<Hediff_AddedPart>();
                diffSet.GetHediffs(ref a);

                Hediff_AddedPart hediff_AddedPart = (from x in a where x.Part == rec select x).First();

                if (hediff_AddedPart != null)
                {
                    if (hediff_AddedPart.def.addedPartProps != null)
                    {
                        var comp = hediff_AddedPart.TryGetComp<HediffCompQualityBionics>();
                        if (comp != null)
                        {
                            __state = new Pair<Hediff, float>(hediff_AddedPart, hediff_AddedPart.def.addedPartProps.partEfficiency);
                            hediff_AddedPart.def.addedPartProps.partEfficiency = comp.Props.baseEfficiency * Settings.GetQualityMultipliers(comp.quality);
                            // QualityBionicsMod.Message($"[PARENT] Applied quality {comp.quality} efficiency to {hediff_AddedPart.def.defName}: base={comp.Props.baseEfficiency} * multiplier={Settings.GetQualityMultipliers(comp.quality)} = {hediff_AddedPart.def.addedPartProps.partEfficiency}");
                            return;
                        }
                    }
                }
            }
        }
        if (part.parent != null && diffSet.PartIsMissing(part.parent))
        {
            return;
        }
        if (!ignoreAddedParts)
        {
            for (int i = 0; i < diffSet.hediffs.Count; i++)
            {
                Hediff_AddedPart? hediff_AddedPart2 = diffSet.hediffs[i] as Hediff_AddedPart;
                if (hediff_AddedPart2 != null && hediff_AddedPart2.Part == part)
                {
                    if (hediff_AddedPart2 != null)
                    {
                        if (hediff_AddedPart2.def.addedPartProps != null)
                        {
                            var comp = hediff_AddedPart2.TryGetComp<HediffCompQualityBionics>();
                            if (comp != null)
                            {
                                __state = new Pair<Hediff, float>(hediff_AddedPart2, hediff_AddedPart2.def.addedPartProps.partEfficiency);
                                hediff_AddedPart2.def.addedPartProps.partEfficiency = comp.Props.baseEfficiency * Settings.GetQualityMultipliers(comp.quality);
                                // QualityBionicsMod.Message($"[DIRECT] Applied quality {comp.quality} efficiency to {hediff_AddedPart2.def.defName}: base={comp.Props.baseEfficiency} * multiplier={Settings.GetQualityMultipliers(comp.quality)} = {hediff_AddedPart2.def.addedPartProps.partEfficiency}");
                                return;
                            }
                        }
                    }
                    return;
                }
            }
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
