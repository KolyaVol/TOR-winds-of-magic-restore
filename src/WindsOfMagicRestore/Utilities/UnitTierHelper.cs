using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class UnitTierHelper
    {
        public static int ClampTier(Agent victim)
        {
            var tier = (victim.Character as CharacterObject)?.Tier ?? 1;
            if (tier < 1)
                return 1;
            if (tier > 6)
                return 6;
            return tier;
        }
    }
}
