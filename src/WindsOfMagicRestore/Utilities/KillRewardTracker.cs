using System.Collections.Generic;

namespace WindsOfMagicRestore.Utilities
{
    internal static class KillRewardTracker
    {
        private static readonly HashSet<int> RewardedVictimIndexes = new();

        public static void Reset()
        {
            RewardedVictimIndexes.Clear();
        }

        public static bool TryClaim(int victimIndex)
        {
            return RewardedVictimIndexes.Add(victimIndex);
        }

        public static void Release(int victimIndex)
        {
            RewardedVictimIndexes.Remove(victimIndex);
        }
    }
}
