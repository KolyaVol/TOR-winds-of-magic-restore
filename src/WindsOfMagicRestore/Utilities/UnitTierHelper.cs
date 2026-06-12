using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class UnitTierHelper
    {
        public const int MinTier = 1;
        public const int MaxTier = 9;

        public static int ClampTier(Agent victim)
        {
            var tier = (victim.Character as CharacterObject)?.Tier ?? MinTier;
            if (tier < MinTier)
                return MinTier;
            if (tier > MaxTier)
                return MaxTier;
            return tier;
        }
    }
}
