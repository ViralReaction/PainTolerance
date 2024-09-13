using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace PainTolerance
{
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        public static HashSet<ThingDef> AllAnimals = [];
        public static HashSet<ThingDef> MissingStatBase = [];
        public static Dictionary<ThingDef, float> cachedPainTolerance = [];
        static Patcher()
        {
            MakeListofAnimals();
            AutoPatch();
            CacheStatDefs();

        }
        public static void MakeListofAnimals()
        {
            var animalList = DefDatabase<ThingDef>.AllDefs.Where(x => x.category == ThingCategory.Pawn);
            foreach (ThingDef animal in animalList)
            {
                AllAnimals.Add(animal);
                if (animal.statBases.StatListContains(PainTolerance_StatDefOf.VR_PainSenstivity))
                {
                    MissingStatBase.Add(animal);
                }
            }
        }
        static void AutoPatch()
        {
            //var animalList = DefDatabase<ThingDef>.AllDefs.Where(x => x.category == ThingCategory.Pawn && !x.statBases.Any(x => x.stat == PainTolerance_StatDefOf.VR_PainSenstivity));
            foreach (ThingDef animal in MissingStatBase)
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
                if (ModsConfig.AnomalyActive && animal.race.IsAnomalyEntity && !ModSettings_PainTolerance.anomalySensitive)
                {
                    toleranceValue = 1f;
                }
                if (!animal.race.IsFlesh)
                {
                    toleranceValue = 0f;
                }
                animal.statBases.Add(new StatModifier { stat = PainTolerance_StatDefOf.VR_PainSenstivity, value = toleranceValue });
            }
           
        }
        static void CacheStatDefs()
        {
            foreach (ThingDef animal in AllAnimals)
            {
                cachedPainTolerance.Add(animal, animal.statBases.GetStatValueFromList(PainTolerance_StatDefOf.VR_PainSenstivity, 1f));
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
            // Define start, midpoint, and end for the body size range
            float bodySizeStart = ModSettings_PainTolerance.bodySizeStart;
            float bodySizeMid = ModSettings_PainTolerance.bodySizeMid;
            float bodySizeEnd = ModSettings_PainTolerance.bodySizeEnd;
            float lowestToleranceValue = ModSettings_PainTolerance.painToleranceEnd;

            // Calculate midpoint value: (1 - lowestToleranceValue) / 2
            float midpointValue = (1f - (1.0f - lowestToleranceValue) / 2.0f);

            float painTolerance;

            // If the body size is between start and midpoint
            if (bodySize <= bodySizeMid)
            {
                // Linear interpolation between 1 and midpoint value
                return Mathf.Lerp(1f, midpointValue, (bodySize - bodySizeStart) / (bodySizeMid - bodySizeStart));

            }
            // If the body size is between midpoint and end
            else if (bodySize <= bodySizeEnd)
            {
                // Linear interpolation between midpoint value and lowestToleranceValue
                return Mathf.Lerp(midpointValue, lowestToleranceValue, (bodySize - bodySizeMid) / (bodySizeEnd - bodySizeMid));
            }
            // Beyond bodySizeEnd, return lowestToleranceValue
            else
            {
                return painTolerance = lowestToleranceValue;
            }
        }
    }
}
