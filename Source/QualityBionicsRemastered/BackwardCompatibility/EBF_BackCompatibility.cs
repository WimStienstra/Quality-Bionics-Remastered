using RimWorld;

// EBF 1.5 compatibility is maintained here by providing dummy classes to not fail the patching.
// ideally, users will eventually leave RW 1.5 behind, and then one day we may safely remove these code.

namespace QualityBionicsContinued
{
    public class Settings
    {
        public float GetQualityMultipliersForHP(QualityCategory category)
        {
            return QualityBionicsRemastered.Settings.GetQualityMultipliersForHP(category);
        }
    }
}

namespace QualityBionics
{
    public class HediffCompQualityBionics : QualityBionicsRemastered.Comps.HediffCompQualityBionics
    {
        // note: EBF 1.5 doesn't read the new "receptor" interface
    }
}
