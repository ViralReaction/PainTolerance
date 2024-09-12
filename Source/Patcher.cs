using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace PainTolerance
{
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        public static List<ThingDef> AllAnimals = new List<ThingDef>();
        static Patcher()
        {
            MakeListofAnimals();
            AutoPatch();

        }
        public static void MakeListofAnimals()
        {
            var animalList = DefDatabase<ThingDef>.AllDefs.Where(x => x.category == ThingCategory.Pawn && !x.statBases.Any(x => x.stat == PainTolerance_StatDefOf.VR_PainTolerance));
            foreach (ThingDef animal in animalList)
            {
                AllAnimals.Add(animal);
            }
        }
        static void AutoPatch()
        {
            foreach (ThingDef animal in AllAnimals)
            {
                float toleranceValue = CalculatePainTolerance(animal.race.baseBodySize);

                // Insects get bonus pain tolerance
                if (ModSettings_PainTolerance.insectSenstivityBonus)
                {
                    if (animal.race.Insect)
                    {
                        toleranceValue *= ModSettings_PainTolerance.insectSensitivityMultiplier;
                    }
                }
                if (!animal.race.IsFlesh)
                {
                    toleranceValue = 0f;
                }

                animal.statBases.Add(new StatModifier { stat = PainTolerance_StatDefOf.VR_PainTolerance, value = toleranceValue } );
            }
        }

        //Curve for decayConstant = 0.4621f
        //Body Size     Pain Tolerance
        //1.5           1.0
        //2.0           0.79
        //2.5           0.62
        //3.0           0.50
        //3.5           0.39
        //4.0           0.31
        //4.5           0.25
        //5.0           0.0
        public static float CalculatePainTolerance(float bodySize)
        {
            // Define start, end, and midpoint (where pain tolerance = 0.5)
            float midpoint = ModSettings_PainTolerance.bodySizeMid;
            float midpointValue = (1f - ModSettings_PainTolerance.painToleranceEnd) / 2f;
            // End point where the curve approaches endTolerance

            // Calculate the decay constant dynamically based on the start and midpoint
            float decayConstant = -Mathf.Log(midpointValue) / (midpoint - ModSettings_PainTolerance.bodySizeStart);

            // If the body size is less than or equal to the start, return 1 (full pain tolerance)
            if (bodySize <= ModSettings_PainTolerance.bodySizeStart)
            {
                return 1f;
            }
            // Calculate the dynamic decay, ensuring the pain tolerance approaches endTolerance at bodySizeEnd
            else if (bodySize <= ModSettings_PainTolerance.bodySizeEnd)
            {
                float normalizedBodySize = (bodySize - ModSettings_PainTolerance.bodySizeStart) / (ModSettings_PainTolerance.bodySizeEnd - ModSettings_PainTolerance.bodySizeStart);
                return ModSettings_PainTolerance.painToleranceEnd + (1f - ModSettings_PainTolerance.painToleranceEnd) * (1f - normalizedBodySize) * Mathf.Exp(-decayConstant * (bodySize - ModSettings_PainTolerance.bodySizeStart));
            }
            // For body sizes beyond the end, return the end tolerance (no further decay)
            else
            {
                return ModSettings_PainTolerance.painToleranceEnd;
            }
        }
    }
}
