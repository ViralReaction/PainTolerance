using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

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
                if (animal.race.Insect)
                {
                    toleranceValue *= 0.75f;
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
        static float CalculatePainTolerance(float bodySize)
        {
            float decayConstant = 0.4621f;

            if (bodySize <= 1.5f)
            {
                return 1f;
            }
            else if (bodySize >= 5.0f)
            {
                return 0f;
            }
            else
            {
                return (float)Math.Exp(-decayConstant * (bodySize - 1.5));
            }
        }
    }
}
