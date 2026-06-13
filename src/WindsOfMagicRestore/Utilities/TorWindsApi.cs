using TaleWorlds.CampaignSystem;

namespace WindsOfMagicRestore.Utilities
{
    internal static class TorWindsApi
    {
        public static bool IsAvailable => TorTypes.AddWindsOfMagic != null || TorTypes.AddCustomResource != null;

        public static void AddWinds(float amount)
        {
            AddWinds(Hero.MainHero, amount);
        }

        public static void AddWinds(Hero? hero, float amount)
        {
            if (amount <= 0f || hero == null)
                return;

            ModGuard.Run("AddWinds", () =>
            {
                if (TorTypes.AddWindsOfMagic != null)
                {
                    TorTypes.AddWindsOfMagic.Invoke(null, new object[] { hero, amount });
                    WindsRestoreMessages.Record(amount);
                    return;
                }

                if (TorTypes.AddCustomResource != null)
                {
                    TorTypes.AddCustomResource.Invoke(null, new object[] { hero, "WindsOfMagic", amount });
                    WindsRestoreMessages.Record(amount);
                    return;
                }

                ModLog.OnceWarn(
                    "tor:winds-api",
                    "TOR wind gain API not found (AddWindsOfMagic and AddCustomResource both missing). " +
                    "No rewards can be granted — check TOR_Core version with wom.diagnostics.");
            });
        }
    }
}
