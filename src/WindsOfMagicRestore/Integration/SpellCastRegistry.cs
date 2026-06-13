using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Battle;

namespace WindsOfMagicRestore.Integration
{
    internal static class SpellCastRegistry
    {
        private static readonly Dictionary<int, Agent> CastersByCastId = new();

        public static void Reset()
        {
            CastersByCastId.Clear();
        }

        public static void Register(int castId, Agent? caster)
        {
            caster = KillCreditHelper.NormalizeAgent(caster);
            if (castId < 0 || caster == null)
                return;

            CastersByCastId[castId] = caster;
        }

        public static Agent? ResolveCaster(int castId)
        {
            if (castId < 0)
                return null;

            return CastersByCastId.TryGetValue(castId, out var caster) ? caster : null;
        }
    }
}
