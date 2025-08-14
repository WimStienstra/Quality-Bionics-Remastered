using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using QualityBionicsRemastered.Comps;
using QualityBionicsRemastered;

namespace QualityBionicsRemastered.Core
{
    /// <summary>
    /// Manager for quality bionics functionality.
    /// </summary>
    public static class QualityBionicsManager
    {
        private static readonly Dictionary<HediffDef, float> _baseEfficiencyCache = new Dictionary<HediffDef, float>();
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Check if a hediff is eligible for quality bionics.
        /// </summary>
        public static bool IsQualityEligible(HediffDef hediffDef)
        {
            if (hediffDef?.spawnThingOnRemoved == null || hediffDef.addedPartProps == null) 
                return false;

            if (!hediffDef.spawnThingOnRemoved.isTechHediff) 
                return false;

            var defName = hediffDef.defName.ToLowerInvariant();
            
            // Check for known bionic types
            if (defName.Contains("bionic") || defName.Contains("archotech"))
                return true;

            // Check for custom defined types
            if (IsCustomBionicType(hediffDef.defName))
                return true;

            // Check tech level requirement
            return hediffDef.spawnThingOnRemoved.techLevel >= Settings.minTechLevelForQuality;
        }

        /// <summary>
        /// Get the base efficiency for a hediff (before quality modifications).
        /// </summary>
        public static float GetBaseEfficiency(HediffDef hediffDef)
        {
            if (hediffDef?.addedPartProps == null) return 1f;

            lock (_lockObject)
            {
                if (_baseEfficiencyCache.TryGetValue(hediffDef, out float cached))
                    return cached;

                float baseEfficiency = hediffDef.addedPartProps.partEfficiency;
                _baseEfficiencyCache[hediffDef] = baseEfficiency;
                return baseEfficiency;
            }
        }

        /// <summary>
        /// Calculate the effective efficiency for a bionic with quality.
        /// </summary>
        public static float CalculateEfficiency(HediffDef hediffDef, QualityCategory quality)
        {
            if (!IsQualityEligible(hediffDef)) 
                return hediffDef?.addedPartProps?.partEfficiency ?? 1f;

            float baseEfficiency = GetBaseEfficiency(hediffDef);
            float qualityMultiplier = Settings.GetQualityMultipliers(quality);
            
            return baseEfficiency * qualityMultiplier;
        }

        /// <summary>
        /// Calculate the effective HP multiplier for a bionic with quality.
        /// </summary>
        public static float CalculateHPMultiplier(QualityCategory quality)
        {
            return Settings.GetQualityMultipliersForHP(quality);
        }

        /// <summary>
        /// Get the quality of a hediff if it has quality bionics component.
        /// </summary>
        public static QualityCategory? GetQuality(Hediff hediff)
        {
            var comp = hediff?.TryGetComp<HediffCompQualityBionics>();
            return comp?.quality;
        }

        /// <summary>
        /// Try to apply quality to a thing when it spawns.
        /// </summary>
        public static bool TryApplyQuality(Thing thing, QualityCategory quality)
        {
            if (thing == null) return false;

            var comp = thing.TryGetComp<CompQuality>();
            if (comp == null) return false;

            try
            {
                comp.SetQuality(quality, ArtGenerationContext.Colony);
                return true;
            }
            catch (Exception ex)
            {
                QualityBionicsMod.Warning($"Failed to apply quality {quality} to {thing.def.defName}: {ex.Message}");
                return false;
            }
        }

        private static bool IsCustomBionicType(string defName)
        {
            // This could be expanded to read from settings or external configuration
            var customTypes = new[] { "synthetic", "cybernetic" };
            return customTypes.Contains(defName);
        }

        /// <summary>
        /// Checks if a hediff definition is a quality bionic.
        /// </summary>
        public static bool IsQualityBionic(HediffDef hediffDef)
        {
            if (hediffDef?.comps == null) return false;
            
            return hediffDef.comps.Any(comp => comp?.GetType() == typeof(HediffCompProperties_QualityBionics));
        }

        /// <summary>
        /// Gets the quality from a hediff if it has a quality component.
        /// </summary>
        public static QualityCategory? GetQualityFromHediff(Hediff hediff)
        {
            try
            {
                var comp = hediff?.TryGetComp<HediffCompQualityBionics>();
                return comp?.quality;
            }
            catch (Exception ex)
            {
                QualityBionicsMod.Warning($"Error getting quality from hediff {hediff?.def?.label}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the ThingDef that should be spawned when this hediff is removed.
        /// </summary>
        public static ThingDef? GetThingDefFromHediffDef(HediffDef hediffDef)
        {
            return hediffDef?.spawnThingOnRemoved;
        }
    }
}
