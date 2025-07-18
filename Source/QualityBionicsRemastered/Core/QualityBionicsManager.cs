using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using QualityBionics;
using QualityBionicsRemastered;

namespace QualityBionicsRemastered.Core
{
    public static class QualityBionicsManager
    {
        private static readonly Dictionary<HediffDef, float> _baseEfficiencyCache = new Dictionary<HediffDef, float>();
        private static readonly HashSet<ThingDef> _qualityEnabledThings = new HashSet<ThingDef>();
        private static readonly object _lockObject = new object();
        private static bool _initialized = false;

        /// <summary>
        /// Initialize the quality bionics system. Called during mod startup.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            lock (_lockObject)
            {
                if (_initialized) return;

                try
                {
                    InitializeQualityBionics();
                    _initialized = true;
                    QualityBionicsMod.Message("Quality Bionics system initialized successfully");
                }
                catch (Exception ex)
                {
                    QualityBionicsMod.Error($"Failed to initialize Quality Bionics system: {ex}");
                }
            }
        }

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

        private static void InitializeQualityBionics()
        {
            var customTypes = new HashSet<string> { "synthetic", "cybernetic" };
            int processedCount = 0;

            foreach (var hediffDef in DefDatabase<HediffDef>.AllDefs)
            {
                if (!IsQualityEligible(hediffDef)) continue;

                try
                {
                    // Store base efficiency
                    GetBaseEfficiency(hediffDef);

                    // Add quality component to hediff
                    hediffDef.comps ??= new List<HediffCompProperties>();
                    if (!hediffDef.comps.Any(x => x.GetType() == typeof(HediffCompProperties_QualityBionics)))
                    {
                        hediffDef.comps.Add(new HediffCompProperties_QualityBionics 
                        { 
                            baseEfficiency = hediffDef.addedPartProps.partEfficiency 
                        });
                    }

                    // Add quality component to spawned thing
                    var thingDef = hediffDef.spawnThingOnRemoved;
                    thingDef.comps ??= new List<CompProperties>();
                    if (!thingDef.comps.Any(x => x.compClass == typeof(CompQuality)))
                    {
                        thingDef.comps.Add(new CompProperties { compClass = typeof(CompQuality) });
                        _qualityEnabledThings.Add(thingDef);
                    }

                    processedCount++;
                }
                catch (Exception ex)
                {
                    QualityBionicsMod.Warning($"Failed to process hediff {hediffDef.defName}: {ex.Message}");
                }
            }

            QualityBionicsMod.Message($"Processed {processedCount} hediffs for quality bionics");
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

        /// <summary>
        /// Clean up resources. Called when the mod is being unloaded.
        /// </summary>
        public static void Cleanup()
        {
            lock (_lockObject)
            {
                _baseEfficiencyCache.Clear();
                _qualityEnabledThings.Clear();
                _initialized = false;
            }
        }
    }
}
