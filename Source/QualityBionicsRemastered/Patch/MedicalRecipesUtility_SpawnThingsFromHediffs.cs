using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Improved medical recipes patch that safely handles quality transfer when spawning items from removed bionics.
/// </summary>
[HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnThingsFromHediffs")]
public static class MedicalRecipesUtility_SpawnThingsFromHediffs
{
    [HarmonyPrefix]
    private static void Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        try
        {
            if (pawn?.health?.hediffSet == null || !pawn.health.hediffSet.GetNotMissingParts().Contains(part)) 
                return;

            // Look for quality bionics on the part being processed
            var hediffsToProcess = pawn.health.hediffSet.hediffs
                .Where(hediff => hediff.Part == part && hediff.def.spawnThingOnRemoved != null)
                .ToList();

            foreach (var hediff in hediffsToProcess)
            {
                var quality = QualityBionicsManager.GetQualityFromHediff(hediff);
                if (quality != null && hediff.def.spawnThingOnRemoved != null)
                {
                    QualityTransferManager.RegisterTransfer(hediff.def.spawnThingOnRemoved, quality.Value);
                    QualityBionicsMod.Message($"Registered quality {quality} for spawning {hediff.def.spawnThingOnRemoved.label}");
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in MedicalRecipesUtility_SpawnThingsFromHediffs prefix: {ex.Message}");
        }
    }
}
