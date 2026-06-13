using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class FinalizeSessionPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.FinalizeSessionMethod();

        public static void Postfix(object session)
        {
            ModGuard.Run("FinalizeSession", () => PostfixCore(session));
        }

        private static void PostfixCore(object session)
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
