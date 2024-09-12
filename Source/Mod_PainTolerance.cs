using HarmonyLib;

using UnityEngine;
using Verse;

namespace PainTolerance
{
    public class Mod_PainTolerance : Mod
    {
        public static ModSettings_PainTolerance settings;

        public Mod_PainTolerance(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModSettings_PainTolerance>();
            Harmony harmony = new (this.Content.PackageIdPlayerFacing);
            harmony.PatchAll();
        }

        public override string SettingsCategory()
        {
            return "Pain_Tolerance".Translate();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }
        public override void WriteSettings()
        {
            base.WriteSettings();
        }

    }
}
