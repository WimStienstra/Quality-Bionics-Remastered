
using System;
using HarmonyLib;
using QualityBionicsRemastered;
using Verse;

[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.GetBackCompatibleType))]
internal static class BackCompatibility_GetBackCompatibleType
{
    private static void Postfix(ref Type __result, string providedClassName)
    {
        // This lets us load our old settings without RimWorld producing an error
        if (providedClassName == "QualityBionicsContinued.QualityBionicsSettings" || 
            providedClassName == "QualityBionics.QualityBionicsSettings" ||
            providedClassName == "QualityBionicsContinued.Settings" ||
            providedClassName == "QualityBionicsRemastered.Settings")
            __result = typeof(Settings);
    }
}
