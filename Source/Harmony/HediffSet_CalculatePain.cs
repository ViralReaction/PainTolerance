using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PainTolerance
{
    [HarmonyPatch(typeof(HediffSet), "CalculatePain")]
    public static class HediffSet_CalculatePain
    {
        static void Postfix(HediffSet __instance, ref float __result)
        {

            __result *= __instance.pawn.GetStatValue(PainTolerance_StatDefOf.VR_PainTolerance);
        }
    }
}
