using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Core
{
    /// <summary>
    /// Thread-safe system for managing quality transfer between hediffs and things.
    /// Replaces the problematic static variables used in the original implementation.
    /// </summary>
    public static class QualityTransferManager
    {
        private static readonly ConcurrentDictionary<int, QualityTransferData> _pendingTransfers = 
            new ConcurrentDictionary<int, QualityTransferData>();

        private static readonly ConcurrentQueue<int> _expiredTransfers = new ConcurrentQueue<int>();

        private const int MAX_TRANSFER_AGE_TICKS = 60; // 1 second at 60 FPS

        private struct QualityTransferData
        {
            public ThingDef ThingDef { get; }
            public QualityCategory Quality { get; }
            public int CreatedTick { get; }

            public QualityTransferData(ThingDef thingDef, QualityCategory quality, int createdTick)
            {
                ThingDef = thingDef;
                Quality = quality;
                CreatedTick = createdTick;
            }

            public bool IsExpired(int currentTick) => currentTick - CreatedTick > MAX_TRANSFER_AGE_TICKS;
        }

        /// <summary>
        /// Register a quality transfer for when a thing spawns.
        /// </summary>
        public static void RegisterTransfer(ThingDef thingDef, QualityCategory quality)
        {
            if (thingDef == null) return;

            int key = GenerateTransferKey(thingDef, quality);
            var transferData = new QualityTransferData(thingDef, quality, Find.TickManager?.TicksGame ?? 0);
            
            _pendingTransfers.TryAdd(key, transferData);
            
            // Clean up old transfers to prevent memory leaks
            CleanupExpiredTransfers();
        }

        /// <summary>
        /// Try to consume a quality transfer for a spawning thing.
        /// </summary>
        public static bool TryConsumeTransfer(Thing thing, out QualityCategory quality)
        {
            quality = QualityCategory.Normal;
            
            if (thing?.def == null) return false;

            // Look for a matching transfer
            var currentTick = Find.TickManager?.TicksGame ?? 0;
            
            foreach (var kvp in _pendingTransfers)
            {
                var transferData = kvp.Value;
                
                // Skip expired transfers
                if (transferData.IsExpired(currentTick))
                {
                    _expiredTransfers.Enqueue(kvp.Key);
                    continue;
                }

                // Check if this transfer matches our thing
                if (transferData.ThingDef == thing.def)
                {
                    quality = transferData.Quality;
                    _pendingTransfers.TryRemove(kvp.Key, out _);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Register multiple quality transfers from a body part removal.
        /// </summary>
        public static void RegisterMultipleTransfers(IEnumerable<(ThingDef thingDef, QualityCategory quality)> transfers)
        {
            if (transfers == null) return;

            foreach (var (thingDef, quality) in transfers)
            {
                RegisterTransfer(thingDef, quality);
            }
        }

        private static int GenerateTransferKey(ThingDef thingDef, QualityCategory quality)
        {
            // Generate a semi-unique key that allows for multiple transfers of the same type
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + thingDef.GetHashCode();
                hash = hash * 31 + quality.GetHashCode();
                hash = hash * 31 + (Find.TickManager?.TicksGame.GetHashCode() ?? 0);
                return hash;
            }
        }

        private static void CleanupExpiredTransfers()
        {
            var currentTick = Find.TickManager?.TicksGame ?? 0;

            // Process expired transfers from the queue
            while (_expiredTransfers.TryDequeue(out int expiredKey))
            {
                _pendingTransfers.TryRemove(expiredKey, out _);
            }

            // Check for expired transfers in the main collection (fallback)
            if (_pendingTransfers.Count > 100) // Only do expensive cleanup if we have many transfers
            {
                var expiredKeys = new List<int>();
                
                foreach (var kvp in _pendingTransfers)
                {
                    if (kvp.Value.IsExpired(currentTick))
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (int key in expiredKeys)
                {
                    _pendingTransfers.TryRemove(key, out _);
                }
            }
        }

        /// <summary>
        /// Clear all pending transfers. Called during cleanup or mod reload.
        /// </summary>
        public static void ClearAll()
        {
            _pendingTransfers.Clear();
            
            while (_expiredTransfers.TryDequeue(out _)) { }
        }

        /// <summary>
        /// Get debug information about pending transfers.
        /// </summary>
        public static string GetDebugInfo()
        {
            return $"Pending transfers: {_pendingTransfers.Count}, Expired queue: {_expiredTransfers.Count}";
        }
    }
}
