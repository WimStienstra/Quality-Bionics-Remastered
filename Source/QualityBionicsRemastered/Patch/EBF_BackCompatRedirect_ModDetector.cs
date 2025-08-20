using HarmonyLib;
using System.Linq;
using System.Reflection;
using Verse;

namespace QualityBionicsRemastered.Patch
{
    /// <summary>
    /// EBF 1.5 is looking specifically for the ilyvion variant.
    /// If we are sure to welcome these 1.5 users, then EBF 1.5 needs to be patched to look for this variant instead.
    /// <para/>
    /// EBF 1.6+ will not trigger this patch since we would be using the EBF "receptor" API for compatibility.
    /// </summary>
    [HarmonyPatch]
    public static class EBF_BackCompatRedirect_ModDetector
    {
        public static bool Prepare()
        {
            return TargetMethod() != null;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter("EBF.Util.ModDetector:QualityBionicsContinuedIsLoaded"); ;
        }

        [HarmonyPrefix]
        public static bool DetectThisQualityBionicsVersionCorrectly(ref bool __result)
        {
            // redirect 1.5 EBF to look for this mod instead
            __result = LoadedModManager.RunningMods.Any((ModContentPack pack) => pack.Name.Contains("Quality Bionics") && pack.PackageId.Contains("assassinsbro"));
            return false;
        }
    }
}
