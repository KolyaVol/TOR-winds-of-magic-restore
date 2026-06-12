using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace WindsOfMagicRestore.Utilities
{
    internal static class TorWindsApi
    {
        private static readonly MethodInfo? AddWindsOfMagic = Type
            .GetType("TOR_Core.Extensions.HeroExtensions, TOR_Core")
            ?.GetMethod(
                "AddWindsOfMagic",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Hero), typeof(float) },
                null);

        private static readonly MethodInfo? AddCustomResource = Type
            .GetType("TOR_Core.Extensions.HeroExtensions, TOR_Core")
            ?.GetMethod(
                "AddCustomResource",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Hero), typeof(string), typeof(float) },
                null);

        private static bool _torApiMissingLogged;

        public static bool IsAvailable => AddWindsOfMagic != null || AddCustomResource != null;

        public static void AddWinds(float amount)
        {
            if (amount <= 0f || Hero.MainHero == null)
                return;

            if (AddWindsOfMagic != null)
            {
                AddWindsOfMagic.Invoke(null, new object[] { Hero.MainHero, amount });
                return;
            }

            if (AddCustomResource != null)
            {
                AddCustomResource.Invoke(null, new object[] { Hero.MainHero, "WindsOfMagic", amount });
                return;
            }

            if (!_torApiMissingLogged)
            {
                _torApiMissingLogged = true;
                Debug.Print("[WindsOfMagicRestore] TOR wind gain API not found. Is TOR_Core loaded?");
            }
        }
    }
}
