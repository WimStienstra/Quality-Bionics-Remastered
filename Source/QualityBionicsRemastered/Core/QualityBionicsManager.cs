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

            // Check if this hediff is from an excluded mod
            var modContentPack = hediffDef.modContentPack;
            if (modContentPack != null && Settings.excludedModPackageIds.Contains(modContentPack.PackageId))
            {
                return false;
            }
            
            // Check if this specific hediff is excluded
            if (Settings.excludedHediffDefs.Contains(hediffDef.defName))
            {
                return false;
            }

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

        /// <summary>
        /// Migrate existing stored bionics in saves to Normal quality on first load.
        /// This ensures that when the mod is added mid-game, existing stored bionics get Normal quality
        /// instead of the C# default enum value (Awful = 0) that CompQuality initializes to when
        /// it is dynamically added to a ThingDef for an already-saved item.
        /// </summary>
        public static void MigrateExistingBionics()
        {
            try
            {
                int migratedCount = 0;

                foreach (var map in Find.Maps)
                {
                    if (map == null) continue;

                    // Migrate things on the map (ground, stockpiles, shelves, etc.)
                    if (map.listerThings != null)
                    {
                        // ToList() to avoid modifying collection during iteration
                        var allThings = map.listerThings.AllThings.ToList();
                        foreach (var thing in allThings)
                        {
                            if (MigrateThing(thing))
                                migratedCount++;
                        }
                    }

                    // Migrate things in pawn inventories
                    foreach (var pawn in map.mapPawns.AllPawns)
                    {
                        if (pawn?.inventory?.innerContainer == null) continue;
                        foreach (var item in pawn.inventory.innerContainer)
                        {
                            if (MigrateThing(item))
                                migratedCount++;
                        }
                    }
                }

                if (migratedCount > 0)
                {
                    QualityBionicsMod.Message($"Migration complete: Applied Normal quality to {migratedCount} existing stored bionics");
                }
            }
            catch (Exception ex)
            {
                QualityBionicsMod.Warning($"Error during bionic migration: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Checks whether a thing needs migration and applies Normal quality if so.
        /// Returns true if quality was migrated.
        /// </summary>
        private static bool MigrateThing(Thing thing)
        {
            if (thing?.def == null) return false;
            if (!thing.def.isTechHediff) return false;

            var correspondingHediff = FindCorrespondingHediffDef(thing.def);
            if (correspondingHediff == null || !IsQualityEligible(correspondingHediff)) return false;

            var compQuality = thing.TryGetComp<CompQuality>();
            if (compQuality == null) return false;

            // When CompQuality is dynamically added to an already-saved ThingDef, it initializes
            // to the C# default enum value of QualityCategory (0 = Awful). We detect this case
            // by checking for Awful quality and upgrading to Normal. Items that already had a
            // quality legitimately set (Good, Excellent, etc.) are left untouched.
            if (thing.TryGetQuality(out var quality) && quality != QualityCategory.Awful)
                return false;

            return TryApplyQuality(thing, QualityCategory.Normal);
        }

        /// <summary>
        /// Find the HediffDef that would spawn this ThingDef when removed.
        /// </summary>
        private static HediffDef? FindCorrespondingHediffDef(ThingDef thingDef)
        {
            foreach (var hediffDef in DefDatabase<HediffDef>.AllDefs)
            {
                if (hediffDef.spawnThingOnRemoved == thingDef)
                    return hediffDef;
            }
            return null;
        }
    }
}
