
using System;
using HarmonyLib;
using QualityBionicsContinued;
using Verse;

[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.GetBackCompatibleType))]
internal static class BackCompatibility_GetBackCompatibleType
{
    private static void Postfix(ref Type __result, string providedClassName)
    {
        // This lets us load our old settings without RimWorld producing an error
        if (providedClassName == "QualityBionicsContinued.QualityBionicsSettings" || providedClassName == "QualityBionics.QualityBionicsSettings")
            __result = typeof(Settings);
    }
}
