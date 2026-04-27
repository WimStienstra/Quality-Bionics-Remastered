using Verse;

namespace QualityBionicsRemastered.Core
{
    /// <summary>
    /// Per-save GameComponent that runs the bionic quality migration exactly once per save file.
    /// Storing the flag here (in the save) rather than in mod settings means:
    /// - Saves where the mod was always active: flag is true from the first save, migration never re-runs.
    /// - Saves where the mod is added mid-game: flag starts false, migration runs once on first load,
    ///   then the flag is saved as true so it never runs again for that save.
    /// </summary>
    public class QualityBionicsGameComponent : GameComponent
    {
        private bool _hasMigrated = false;

        public QualityBionicsGameComponent(Game game) : base()
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            if (!_hasMigrated)
            {
                QualityBionicsManager.MigrateExistingBionics();
                _hasMigrated = true;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _hasMigrated, "hasMigrated", false);
        }
    }
}
