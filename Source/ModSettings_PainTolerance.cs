﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace PainTolerance
{
    public class ModSettings_PainTolerance : ModSettings
    {
        public static float
            bodySizeStart = 1.5f,
            bodySizeEnd = 5f,
            bodySizeMid = 3f,
            painToleranceEnd = 0.20f,
            insectSensitivityMultiplier = 0.75f,
            anomalSenstivityMultipler = 1f;

        public static bool
            insectSenstivityBonus = true,
            anomalySensitive = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref bodySizeStart, "bodySizeStart", 1.5f);
            Scribe_Values.Look(ref bodySizeEnd, "bodySizeEnd", 5f);
            Scribe_Values.Look(ref bodySizeMid, "bodySizeMid", 3f);
            Scribe_Values.Look(ref painToleranceEnd, "painToleranceEnd", 0.2f);
            Scribe_Values.Look(ref insectSensitivityMultiplier, "insectSensitivityMultiplier", 0.75f);
            Scribe_Values.Look(ref insectSenstivityBonus, "insectSenstivityBonus", true);
            Scribe_Values.Look(ref anomalySensitive, "anomalySensitive", true);
            base.ExposeData();
        }
        private Vector2 scrollPosition;
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width - 20f, inRect.height);
            Widgets.BeginScrollView(inRect, ref this.scrollPosition, rect, true);
            Listing_PainTolerance options = new Listing_PainTolerance();
                
            options.Begin(rect);
            options.CustomIntBoxWithButtons("bodySizeStart_Title".Translate(), ref bodySizeStart, 0f, bodySizeEnd - 1f, 0.1f, "bodySizeStart_Desc".Translate());
            options.CustomIntBoxWithButtons("bodySizeEnd_Title".Translate(), ref bodySizeEnd, bodySizeStart + 1f, 99f, 0.1f, "bodySizeEnd_Desc".Translate());
            options.CustomIntBoxWithButtons("bodySizeMid_Title".Translate(), ref bodySizeMid, bodySizeStart + 0.1f, bodySizeEnd - 0.1f, 0.1f, "bodySizeMid_Desc".Translate());
            options.CustomIntBoxWithButtons("painToleranceEnd_Title".Translate(), ref painToleranceEnd, 0f, 1f, 0.1f, "painToleranceEnd_Desc".Translate());
            options.CustomCheckboxLabeled("insectSenstivityBonus_Title".Translate(), ref insectSenstivityBonus, "insectSenstivityBonus_Desc".Translate());
            if (insectSenstivityBonus)
            {
                options.CustomIntBoxWithButtons("insectSensitivityMultiplier_Title".Translate(), ref insectSensitivityMultiplier, 0f, 1f, 0.05f, "insectSensitivityMultiplier_Desc".Translate());
            }
            if (ModsConfig.AnomalyActive)
            {
                options.CustomCheckboxLabeled("anomalySensitive_Title".Translate(), ref anomalySensitive, "anomalySensitive_Desc".Translate());
            }
            options.GapLine();
            options.CustomGraph("Body Size", 250f);
            options.GapLine(36f);
            if (options.ButtonText("Reset to Defaults"))
            {
                ResetSettingsToDefault();
            }
            options.End();
            Widgets.EndScrollView();
        }
        public void ResetSettingsToDefault()
        {
            bodySizeStart = 1.5f;
            bodySizeEnd = 5f;
            bodySizeMid = 3f;
            painToleranceEnd = 0.20f;

            insectSenstivityBonus = true;
            insectSensitivityMultiplier = 0.75f;
            anomalySensitive = true;

        }

    }
}