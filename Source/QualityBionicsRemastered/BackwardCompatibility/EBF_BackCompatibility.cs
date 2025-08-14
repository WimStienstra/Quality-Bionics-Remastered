using System.Runtime.CompilerServices;
using RimWorld;

// Note: TypeForwardedTo is not needed since everything is in the same assembly now.
// EBF compatibility is maintained through the QualityBionics namespace wrapper classes below.

namespace QualityBionics
{
    /// <summary>
    /// Backward compatibility wrapper for HediffCompQualityBionics.
    /// This allows EBF and other mods to find the class in the expected namespace.
    /// </summary>
    public class HediffCompQualityBionics : QualityBionicsRemastered.Comps.HediffCompQualityBionics
    {
        // This class inherits everything from the real implementation
        // and provides backward compatibility by existing in the expected namespace
    }

    /// <summary>
    /// Backward compatibility patch placeholder for EBF.
    /// This exists solely to be fake-patched by EBF; we skip the real implementation ourselves.
    /// </summary>
    class GetMaxHealth_Patch
    {
        private void Postfix()
        {
            // Just here to be fake-patched by EBF; we skip the real one ourselves now.
        }
    }

    /// <summary>
    /// Legacy QualityBionicsMod class for backward compatibility.
    /// These are here to pretend to be their old versions from 1.4, since apparently it's my fault
    /// if something breaks when I change things, even though these are internal/private classes not
    /// meant to be accessed externally.
    /// </summary>
    class QualityBionicsMod
    {
        public static QualityBionicsSettings settings = new();
        
        public static void WarningOnce(string msg, int key)
        {
            QualityBionicsRemastered.QualityBionicsMod.WarningOnce(msg, key);
        }
    }

    /// <summary>
    /// Legacy QualityBionicsSettings class for backward compatibility.
    /// Forwards calls to the actual implementation in QualityBionicsRemastered namespace.
    /// </summary>
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

    /// <summary>
    /// Direct settings access for EBF compatibility.
    /// Provides static access to settings that some mods may expect.
    /// </summary>
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
}
