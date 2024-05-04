using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace QualityBionicsContinued;

class QualityBionicsMod : Mod
{
    public QualityBionicsMod(ModContentPack pack) : base(pack)
    {
        new Harmony(Constants.Id).PatchAll(Assembly.GetExecutingAssembly());
        GetSettings<Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoSettingsWindowContents(inRect);
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
