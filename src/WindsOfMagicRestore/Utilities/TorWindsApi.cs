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

        private static bool _torApiMissingLogged;

        public static bool IsAvailable => AddWindsOfMagic != null;

        public static void AddWinds(float amount)
        {
            if (amount <= 0f || Hero.MainHero == null)
                return;

            if (AddWindsOfMagic == null)
            {
                if (!_torApiMissingLogged)
                {
                    _torApiMissingLogged = true;
                    Debug.Print("[WindsOfMagicRestore] TOR_Core.Extensions.HeroExtensions.AddWindsOfMagic not found. Is TOR_Core loaded?");
                }
                return;
            }

            AddWindsOfMagic.Invoke(null, new object[] { Hero.MainHero, amount });
        }
    }
}
