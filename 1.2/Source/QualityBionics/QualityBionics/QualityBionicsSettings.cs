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
    class QualityBionicsSettings : ModSettings
    {
        public Dictionary<QualityCategory, float> qualityMultipliers = new Dictionary<QualityCategory, float>
        {
            {QualityCategory.Awful, 0.50f},
            {QualityCategory.Poor, 0.75f},
            {QualityCategory.Normal, 1f},
            {QualityCategory.Good, 1.25f},
            {QualityCategory.Excellent, 1.5f},
            {QualityCategory.Masterwork, 1.7f},
            {QualityCategory.Legendary, 2f},
        };

        public float GetQualityMultipliers(QualityCategory quality)
        {
            return qualityMultipliers[quality];
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref qualityMultipliers, "qualityMultipliers", LookMode.Value, LookMode.Value, ref qualityKeys, ref floatValues);
        }

        private List<QualityCategory> qualityKeys;
        private List<float> floatValues;
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width / 3, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            foreach (QualityCategory quality in Enum.GetValues(typeof(QualityCategory)))
            {
                var value = qualityMultipliers[quality];
                listingStandard.SliderLabeled(Enum.GetName(typeof(QualityCategory), quality), ref value, value.ToStringDecimalIfSmall(), 0.01f, 5f);
                qualityMultipliers[quality] = value;
            }
            if (listingStandard.ButtonText("Reset".Translate()))
            {
                qualityMultipliers = new Dictionary<QualityCategory, float>
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
            listingStandard.End();
            base.Write();
        }
    }
}
