
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QualityBionicsContinued.Comps;
using RimWorld;
using Verse;

namespace QualityBionicsContinued.Patch;

[HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnThingsFromHediffs")]
public class MedicalRecipesUtility_SpawnThingsFromHediffs
{
    public static List<Pair<ThingDef, QualityCategory>?>? thingsWithQualities = new List<Pair<ThingDef, QualityCategory>?>();

    private static void Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
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
                    }
                }
            }
        }
    }

    private static void Postfix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        thingsWithQualities = null;
    }
}
