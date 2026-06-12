using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace WindsOfMagicRestore.Utilities
{
    internal static class TorWindsApi
    {
        private static bool _torApiMissingLogged;

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

                if (!_torApiMissingLogged)
                {
                    _torApiMissingLogged = true;
                    Debug.Print("[WindsOfMagicRestore] TOR wind gain API not found. Is TOR_Core loaded?");
                }
            });
        }
    }
}
