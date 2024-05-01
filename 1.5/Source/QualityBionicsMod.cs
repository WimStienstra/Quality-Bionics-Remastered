using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace QualityBionics
{
    class QualityBionicsMod : Mod
    {
        public QualityBionicsMod(ModContentPack pack) : base(pack)
        {
            GetSettings<QualityBionicsSettings>();
            if (QualityBionicsSettings.hpQualityMultipliers is null)
            {
                QualityBionicsSettings.qualityMultipliers = new Dictionary<QualityCategory, float>
                {
                    {QualityCategory.Awful, 0.50f},
                    {QualityCategory.Poor, 0.75f},
                    {QualityCategory.Normal, 1f},
                    {QualityCategory.Good, 1.25f},
                    {QualityCategory.Excellent, 1.5f},
                    {QualityCategory.Masterwork, 1.7f},
                    {QualityCategory.Legendary, 2f},
                };
            }
            if (QualityBionicsSettings.hpQualityMultipliers is null)
            {
                QualityBionicsSettings.hpQualityMultipliers = new Dictionary<QualityCategory, float>
                {
                    {QualityCategory.Awful, 0.50f},
                    {QualityCategory.Poor, 0.75f},
                    {QualityCategory.Normal, 1f},
                    {QualityCategory.Good, 1.25f},
                    {QualityCategory.Excellent, 1.5f},
                    {QualityCategory.Masterwork, 1.7f},
                    {QualityCategory.Legendary, 2f},
                };
            }
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            QualityBionicsSettings.DoSettingsWindowContents(inRect);
            GetSettings<QualityBionicsSettings>().Write();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public static void Message(string msg)
        {
            Log.Message("[Quality Bionics (Continued)] " + msg);
        }

        public static void Warning(string msg)
        {
            Log.Warning("[Quality Bionics (Continued)] " + msg);
        }

        public static void Error(string msg)
        {
            Log.Error("[Quality Bionics (Continued)] " + msg);
        }

        public static void Exception(string msg, Exception? e = null)
        {
            Message(msg);
            if (e != null)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
