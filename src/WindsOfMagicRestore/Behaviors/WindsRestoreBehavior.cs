using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Behaviors
{
    public class WindsRestoreBehavior : MissionLogic
    {
        public override void AfterStart()
        {
            AugmentBuffTracker.Reset();
        }

        public override void OnAgentRemoved(
            Agent affectedAgent,
            Agent affectorAgent,
            AgentState agentState,
            KillingBlow blow)
        {
            if (agentState != AgentState.Killed)
                return;

            if (Hero.MainHero == null || Agent.Main == null)
                return;

            if (affectorAgent == null || !affectedAgent.IsEnemyOf(affectorAgent))
                return;

            var tier = (affectedAgent.Character as CharacterObject)?.Tier ?? 1;
            if (tier < 1)
                tier = 1;
            else if (tier > 6)
                tier = 6;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            if (affectorAgent == Agent.Main)
            {
                TorWindsApi.AddWinds(settings.GetWindsForTier(tier));
                return;
            }

            if (!AgentPartyHelper.IsMainPartyAgent(affectorAgent))
                return;

            if (!AugmentBuffTracker.HasActivePlayerAugmentBuff(affectorAgent))
                return;

            TorWindsApi.AddWinds(settings.GetWindsForAugmentKillTier(tier));
        }

        public override void OnMissionTick(float dt)
        {
            if (Hero.MainHero == null || Agent.Main == null || !Agent.Main.IsActive())
                return;

            if (Mission.IsFriendlyMission || Mission.IsMissionEnding)
                return;

            var rate = WindsOfMagicRestoreSettings.Instance?.WindsPerSecond ?? 0f;
            if (rate <= 0f)
                return;

            TorWindsApi.AddWinds(rate * dt);
        }
    }
}
