using TaleWorlds.CampaignSystem;

namespace WindsOfMagicRestore.Utilities
{
    internal static class TorWindsApi
    {
        public static bool IsAvailable => TorTypes.AddWindsOfMagic != null || TorTypes.AddCustomResource != null;

        public static void AddWinds(float amount)
        {
            if (amount <= 0f || Hero.MainHero == null)
                return;

            ModGuard.Run("AddWinds", () =>
            {
                if (TorTypes.AddWindsOfMagic != null)
                {
                    TorTypes.AddWindsOfMagic.Invoke(null, new object[] { Hero.MainHero, amount });
                    return;
                }

                if (TorTypes.AddCustomResource != null)
                {
                    TorTypes.AddCustomResource.Invoke(null, new object[] { Hero.MainHero, "WindsOfMagic", amount });
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
