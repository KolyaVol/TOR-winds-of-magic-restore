using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    internal static class KillRewardService
    {
        public static void TryGrantForAgentRemoval(
            Agent victim,
            Agent? affectorAgent,
            AgentState agentState,
            KillingBlow blow,
            Mission mission)
        {
            ModGuard.Run("AgentRemovalKillReward", () =>
            {
                if (agentState != AgentState.Killed && agentState != AgentState.Unconscious)
                    return;

                if (Hero.MainHero == null || Agent.Main == null)
                    return;

                TryGrantKill(victim, KillCreditHelper.ResolveKillerAgent(mission, affectorAgent, blow));
            });
        }

        public static void TryGrantForSpellKill(Agent victim, Agent? caster)
        {
            ModGuard.Run("SpellKillReward", () =>
            {
                if (Hero.MainHero == null || Agent.Main == null || victim == null)
                    return;

                TryGrantKill(victim, KillCreditHelper.NormalizeAgent(caster));
            });
        }

        private static void TryGrantKill(Agent victim, Agent? killer)
        {
            if (!KillRewardTracker.TryClaim(victim.Index))
                return;

            if (!TryGrant(victim, killer))
                KillRewardTracker.Release(victim.Index);
        }

        private static bool TryGrant(Agent victim, Agent? killer)
        {
            if (killer == null || !KillCreditHelper.IsHostileVictim(victim, killer))
                return false;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return false;

            var tier = UnitTierHelper.ClampTier(victim);

            if (KillCreditHelper.IsMainHeroAgent(killer))
            {
                TorWindsApi.AddWinds(settings.GetWindsForTier(tier));
                return true;
            }

            if (!AgentPartyHelper.IsMainPartyAgent(killer))
                return false;

            if (!AugmentBuffTracker.HasActivePlayerBuffForAugmentKills(killer))
                return false;

            TorWindsApi.AddWinds(settings.GetWindsForAugmentKillTier(tier));
            return true;
        }
    }
}
