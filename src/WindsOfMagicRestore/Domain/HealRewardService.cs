using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Battle;
using WindsOfMagicRestore.Infrastructure;
using WindsOfMagicRestore.Integration;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Domain
{
    internal static class HealRewardService
    {
        public static void TryGrantForSession(object session)
        {
            ModGuard.Run("HealReward", () => TryGrantForSessionCore(session));
        }

        private static void TryGrantForSessionCore(object session)
        {
            if (session == null || Hero.MainHero == null || Agent.Main == null)
                return;

            var healingDone = (int)(TorTypes.SessionTotalHealingDone?.GetValue(session) ?? 0);
            if (healingDone <= 0)
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            var caster = TorTypes.SessionCaster?.GetValue(session) as Agent;
            if (KillCreditHelper.IsMainHeroAgent(caster))
            {
                TorWindsApi.AddWinds(settings.GetWindsForHealing(healingDone));
                return;
            }

            if (!CompanionHelper.IsCompanionAgent(caster))
                return;

            var companionWinds = settings.GetCompanionWindsForHealing(healingDone);
            CompanionWindsGrantService.Grant(companionWinds, caster, settings.GetCompanionHealRestoreMode());
        }
    }
}
