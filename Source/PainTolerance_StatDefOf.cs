using RimWorld;

namespace PainTolerance
{
    [DefOf]
    public static class PainTolerance_StatDefOf
    {
       
        public static StatDef VR_PainTolerance;  

        static PainTolerance_StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PainTolerance_StatDefOf));
        }
    }
}
