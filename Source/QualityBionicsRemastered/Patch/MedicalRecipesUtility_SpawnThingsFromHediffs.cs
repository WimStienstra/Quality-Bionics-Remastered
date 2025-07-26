using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionics;
using QualityBionicsRemastered.Core;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Patch;

/// <summary>
/// Medical recipes patch that handles quality transfer when spawning items from removed bionics.
/// Based on the original mod approach.
/// </summary>
[HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnThingsFromHediffs")]
public static class MedicalRecipesUtility_SpawnThingsFromHediffs
{
    public static List<Pair<ThingDef, QualityCategory>?>? thingsWithQualities = new List<Pair<ThingDef, QualityCategory>?>();

    [HarmonyPrefix]
    private static void Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        try
        {
            if (pawn.health.hediffSet.GetNotMissingParts().Contains(part))
            {
                foreach (Hediff item in pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == part))
                {
                    if (item.def.spawnThingOnRemoved != null)
                    {
                        var comp = item.TryGetComp<HediffCompQualityBionics>();
                        if (comp != null)
                        {
                            if (thingsWithQualities is null)
                            {
                                thingsWithQualities = new List<Pair<ThingDef, QualityCategory>?>();
                            }
                            thingsWithQualities.Add(new Pair<ThingDef, QualityCategory>(item.def.spawnThingOnRemoved, comp.quality));
                            QualityBionicsMod.Message($"Registered medical quality {comp.quality} for {item.def.spawnThingOnRemoved.defName}");
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            QualityBionicsMod.Warning($"Error in MedicalRecipesUtility_SpawnThingsFromHediffs prefix: {ex.Message}");
        }
    }

    [HarmonyPostfix]
    private static void Postfix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        thingsWithQualities = null;
    }
}
