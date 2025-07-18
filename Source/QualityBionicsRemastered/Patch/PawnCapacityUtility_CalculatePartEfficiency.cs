using System.Collections.Generic;
using HarmonyLib;
using QualityBionics;
using QualityBionicsRemastered.Core;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved patch for PawnCapacityUtility.CalculatePartEfficiency that doesn't modify shared definitions.
/// This replaces the original "crude and heavy handed" approach that directly modified def values.
/// </summary>
[HarmonyPatch(typeof(PawnCapacityUtility), "CalculatePartEfficiency")]
public static class PawnCapacityUtility_CalculatePartEfficiency
{
    /// <summary>
    /// Non-destructive postfix that calculates quality-adjusted efficiency without modifying shared data.
    /// </summary>
    [HarmonyPostfix]
    private static void Postfix(ref float __result, HediffSet diffSet, BodyPartRecord part, bool ignoreAddedParts = false, List<PawnCapacityUtility.CapacityImpactor>? impactors = null)
    {
        try
        {
            if (ignoreAddedParts || __result <= 0f) return;

            // Check for quality bionics in parent parts
            var qualityMultiplier = GetQualityMultiplierForPartHierarchy(diffSet, part);
            
            if (qualityMultiplier != 1f)
            {
                __result *= qualityMultiplier;
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in PawnCapacityUtility patch: {ex.Message}");
        }
    }

    private static float GetQualityMultiplierForPartHierarchy(HediffSet diffSet, BodyPartRecord part)
    {
        // Check parent parts first (original logic)
        for (var rec = part.parent; rec != null; rec = rec.parent)
        {
            if (diffSet.HasDirectlyAddedPartFor(rec))
            {
                var addedPart = GetAddedPartForBodyPart(diffSet, rec);
                if (addedPart != null)
                {
                    var multiplier = GetQualityMultiplierForHediff(addedPart);
                    if (multiplier != 1f) return multiplier;
                }
            }
        }

        // Check if the part itself is missing (parent check)
        if (part.parent != null && diffSet.PartIsMissing(part.parent))
        {
            return 1f;
        }

        // Check the part itself
        var directAddedPart = GetAddedPartForBodyPart(diffSet, part);
        if (directAddedPart != null)
        {
            return GetQualityMultiplierForHediff(directAddedPart);
        }

        return 1f;
    }

    private static Hediff_AddedPart? GetAddedPartForBodyPart(HediffSet diffSet, BodyPartRecord part)
    {
        // More efficient search than the original LINQ query
        for (int i = 0; i < diffSet.hediffs.Count; i++)
        {
            if (diffSet.hediffs[i] is Hediff_AddedPart addedPart && addedPart.Part == part)
            {
                return addedPart;
            }
        }
        return null;
    }

    private static float GetQualityMultiplierForHediff(Hediff_AddedPart addedPart)
    {
        if (addedPart?.def?.addedPartProps == null) return 1f;

        var comp = addedPart.TryGetComp<HediffCompQualityBionics>();
        if (comp == null) return 1f;

        // Use our new manager to calculate quality-adjusted efficiency
        var baseEfficiency = QualityBionicsManager.GetBaseEfficiency(addedPart.def);
        var qualityEfficiency = QualityBionicsManager.CalculateEfficiency(addedPart.def, comp.quality);
        
        // Return the multiplier rather than the absolute value
        return baseEfficiency > 0f ? qualityEfficiency / baseEfficiency : 1f;
    }
}
