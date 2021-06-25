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
        public static QualityBionicsSettings settings;
        public QualityBionicsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<QualityBionicsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Quality Bionics";
        }
    }
}
