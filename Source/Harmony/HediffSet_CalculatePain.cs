using HarmonyLib;
using RimWorld;
using System;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace PainTolerance
{
    [HarmonyPatch(typeof(HediffSet), "CalculatePain")]
    public static class HediffSet_CalculatePain
    {
        public static void Postfix(HediffSet __instance, ref float __result)
        {
            __result *= Patcher.cachedPainTolerance.TryGetValue(__instance.pawn.def, 1f);
            __result = __result < 1f ? __result : 1f;
        }
    }
}
