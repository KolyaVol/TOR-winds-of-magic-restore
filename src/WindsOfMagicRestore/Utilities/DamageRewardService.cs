using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    internal static class DamageRewardService
    {
        public static void TryGrantForCombatDamage(Agent victim, Agent? attacker, float damage)
        {
            if (damage <= 0f || victim == null || Hero.MainHero == null || Agent.Main == null)
                return;

            attacker = KillCreditHelper.NormalizeAgent(attacker);
            if (attacker == null || !KillCreditHelper.IsMainHeroAgent(attacker))
                return;

            if (!KillCreditHelper.IsHostileVictim(victim, attacker))
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            TorWindsApi.AddWinds(settings.GetWindsForMeleeDamage(damage));
        }

        public static void TryGrantForSpellDamage(Agent victim, Agent? caster, int damage)
        {
            if (damage <= 0 || victim == null || Hero.MainHero == null || Agent.Main == null)
                return;

            caster = KillCreditHelper.NormalizeAgent(caster);
            if (caster == null || !KillCreditHelper.IsMainHeroAgent(caster))
                return;

            if (!KillCreditHelper.IsHostileVictim(victim, caster))
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            TorWindsApi.AddWinds(settings.GetWindsForSpellDamage(damage));
        }
    }
}
