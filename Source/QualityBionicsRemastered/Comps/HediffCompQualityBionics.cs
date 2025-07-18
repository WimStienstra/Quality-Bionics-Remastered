using RimWorld;
using Verse;

// EBF compatibility requires us to use this old namespace, sadly.
//namespace QualityBionicsContinued.Comps;
namespace QualityBionics;

public class HediffCompQualityBionics : HediffComp
{
    public HediffCompProperties_QualityBionics Props => (HediffCompProperties_QualityBionics)props;

    public QualityCategory quality = QualityCategory.Normal;

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
