using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    internal static class DamageRewardService
    {
        public static void TryGrantForCombatDamage(Agent victim, Agent? attacker, float damage)
        {
            ModGuard.Run("CombatDamageReward", () => TryGrantForCombatDamageCore(victim, attacker, damage));
        }

        public static void TryGrantForSpellDamage(Agent victim, Agent? caster, int damage)
        {
            ModGuard.Run("SpellDamageReward", () => TryGrantForSpellDamageCore(victim, caster, damage));
        }

        private static void TryGrantForCombatDamageCore(Agent victim, Agent? attacker, float damage)
        {
            if (damage <= 0f || victim == null || Hero.MainHero == null || Agent.Main == null)
                return;

            attacker = KillCreditHelper.NormalizeAgent(attacker);
            if (attacker == null)
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            if (KillCreditHelper.IsMainHeroAgent(attacker))
            {
                if (KillCreditHelper.IsHostileVictim(victim, attacker))
                    TorWindsApi.AddWinds(settings.GetWindsForMeleeDamage(damage));
                else if (KillCreditHelper.IsFriendlyVictim(victim, attacker))
                    TorWindsApi.AddWinds(settings.GetWindsForFriendlyMeleeDamage(damage));

                return;
            }

            if (!CompanionHelper.IsCompanionAgent(attacker))
                return;

            float winds;
            if (KillCreditHelper.IsHostileVictim(victim, attacker))
                winds = settings.GetCompanionWindsForMeleeDamage(damage);
            else if (KillCreditHelper.IsFriendlyVictim(victim, attacker))
                winds = settings.GetCompanionWindsForFriendlyMeleeDamage(damage);
            else
                return;

            CompanionWindsGrantService.Grant(winds, attacker, settings.GetCompanionDamageRestoreMode());
        }

        private static void TryGrantForSpellDamageCore(Agent victim, Agent? caster, int damage)
        {
            if (damage <= 0 || victim == null || Hero.MainHero == null || Agent.Main == null)
                return;

            caster = KillCreditHelper.NormalizeAgent(caster);
            if (caster == null)
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            if (KillCreditHelper.IsMainHeroAgent(caster))
            {
                if (KillCreditHelper.IsHostileVictim(victim, caster))
                    TorWindsApi.AddWinds(settings.GetWindsForSpellDamage(damage));
                else if (KillCreditHelper.IsFriendlyVictim(victim, caster))
                    TorWindsApi.AddWinds(settings.GetWindsForFriendlySpellDamage(damage));

                return;
            }

            if (!CompanionHelper.IsCompanionAgent(caster))
                return;

            float winds;
            if (KillCreditHelper.IsHostileVictim(victim, caster))
                winds = settings.GetCompanionWindsForSpellDamage(damage);
            else if (KillCreditHelper.IsFriendlyVictim(victim, caster))
                winds = settings.GetCompanionWindsForFriendlySpellDamage(damage);
            else
                return;

            CompanionWindsGrantService.Grant(winds, caster, settings.GetCompanionDamageRestoreMode());
        }
    }
}
