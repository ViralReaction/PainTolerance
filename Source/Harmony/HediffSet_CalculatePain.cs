using HarmonyLib;
using RimWorld;
using System;
using Verse;
using System.Collections.Generic;

namespace PainTolerance
{
    [HarmonyPatch(typeof(HediffSet), "CalculatePain")]
    public static class HediffSet_CalculatePain
    {
        static void Postfix(HediffSet __instance, ref float __result)
        {

            __result *= __instance.pawn.GetStatValue(PainTolerance_StatDefOf.VR_PainSenstivity);
        }
    }
}
