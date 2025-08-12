using System.Runtime.CompilerServices;
using RimWorld;

[assembly: TypeForwardedTo(typeof(QualityBionics.HediffCompQualityBionics))]

namespace QualityBionics;

class GetMaxHealth_Patch
{
    private void Postfix()
    {
        // Just here to be fake-patched by EBF; we skip the real one ourselves now.
    }
}

// These are here to pretend to be their old versions from 1.4, since apparently it's my fault
// if something breaks when I change things, even though these are internal/private classes not
// meant to be accessed externally.
class QualityBionicsMod
{
    public static QualityBionicsSettings settings = new();
    
    public static void WarningOnce(string msg, int key)
    {
        QualityBionicsRemastered.QualityBionicsMod.WarningOnce(msg, key);
    }
}

class QualityBionicsSettings
{
    public float GetQualityMultipliersForHP(QualityCategory category)
    {
        return QualityBionicsRemastered.Settings.GetQualityMultipliersForHP(category);
    }

    public float GetQualityMultipliers(QualityCategory category)
    {
        return QualityBionicsRemastered.Settings.GetQualityMultipliers(category);
    }
}

// Direct settings access for EBF compatibility
class Settings
{
    public static float GetQualityMultipliersForHP(QualityCategory category)
    {
        return QualityBionicsRemastered.Settings.GetQualityMultipliersForHP(category);
    }

    public static float GetQualityMultipliers(QualityCategory category)
    {
        return QualityBionicsRemastered.Settings.GetQualityMultipliers(category);
    }
}
