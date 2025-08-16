using EBF.API;
using RimWorld;
using Verse;

namespace QualityBionicsRemastered.Comps;

public class HediffCompQualityBionics : HediffComp, IHediffCompAdjustsMaxHp
{
    public HediffCompProperties_QualityBionics Props => (HediffCompProperties_QualityBionics)props;

    public QualityCategory quality = QualityCategory.Normal;

    public BodyPartMaxHpAdjustment MaxHpAdjustment
    {
        get
        {
            // read the quality settings and emit the adjustment to EBF
            return new BodyPartMaxHpAdjustment
            {
                ScaleMultiplier = Settings.GetQualityMultipliersForHP(quality)
            };
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref quality, "quality", QualityCategory.Normal);
    }
}

public class HediffCompProperties_QualityBionics : HediffCompProperties
{
    public float baseEfficiency;

    public HediffCompProperties_QualityBionics()
    {
        this.compClass = typeof(HediffCompQualityBionics);
    }
}
