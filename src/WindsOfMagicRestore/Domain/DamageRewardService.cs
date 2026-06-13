using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Battle;
using WindsOfMagicRestore.Infrastructure;
using WindsOfMagicRestore.Integration;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Domain
{
    internal static class DamageRewardService
    {
        public static void TryGrantForCombatDamage(Agent victim, Agent? attacker, float damage)
        {
            ModGuard.Run("CombatDamageReward", () =>
            {
                var settings = WindsOfMagicRestoreSettings.Instance;
                if (settings == null)
                    return;

                TryGrantForDamage(
                    victim,
                    attacker,
                    damage,
                    settings.GetWindsForMeleeDamage,
                    settings.GetWindsForFriendlyMeleeDamage,
                    settings.GetCompanionWindsForMeleeDamage,
                    settings.GetCompanionWindsForFriendlyMeleeDamage,
                    settings.GetCompanionDamageRestoreMode);
            });
        }

        public static void TryGrantForSpellDamage(Agent victim, Agent? caster, int damage)
        {
            ModGuard.Run("SpellDamageReward", () =>
            {
                var settings = WindsOfMagicRestoreSettings.Instance;
                if (settings == null)
                    return;

                TryGrantForDamage(
                    victim,
                    caster,
                    damage,
                    d => settings.GetWindsForSpellDamage((int)d),
                    d => settings.GetWindsForFriendlySpellDamage((int)d),
                    d => settings.GetCompanionWindsForSpellDamage((int)d),
                    d => settings.GetCompanionWindsForFriendlySpellDamage((int)d),
                    settings.GetCompanionDamageRestoreMode);
            });
        }

        private static void TryGrantForDamage(
            Agent victim,
            Agent? attacker,
            float damage,
            Func<float, float> heroHostileLookup,
            Func<float, float> heroFriendlyLookup,
            Func<float, float> companionHostileLookup,
            Func<float, float> companionFriendlyLookup,
            Func<CompanionWindsRecipientMode> companionModeLookup)
        {
            if (damage <= 0f || victim == null || Hero.MainHero == null || Agent.Main == null)
                return;

            attacker = KillCreditHelper.NormalizeAgent(attacker);
            if (attacker == null)
                return;

            if (KillCreditHelper.IsMainHeroAgent(attacker))
            {
                if (KillCreditHelper.IsHostileVictim(victim, attacker))
                    TorWindsApi.AddWinds(heroHostileLookup(damage));
                else if (KillCreditHelper.IsFriendlyVictim(victim, attacker))
                    TorWindsApi.AddWinds(heroFriendlyLookup(damage));

                return;
            }

            if (!CompanionHelper.IsCompanionAgent(attacker))
                return;

            float winds;
            if (KillCreditHelper.IsHostileVictim(victim, attacker))
                winds = companionHostileLookup(damage);
            else if (KillCreditHelper.IsFriendlyVictim(victim, attacker))
                winds = companionFriendlyLookup(damage);
            else
                return;

            CompanionWindsGrantService.Grant(winds, attacker, companionModeLookup());
        }
    }
}
