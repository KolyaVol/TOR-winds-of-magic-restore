using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Behaviors
{
    public class WindsRestoreBehavior : MissionLogic
    {
        public override void OnAgentRemoved(
            Agent affectedAgent,
            Agent affectorAgent,
            AgentState agentState,
            KillingBlow blow)
        {
            if (agentState != AgentState.Killed)
                return;

            if (affectorAgent != Agent.Main)
                return;

            if (Hero.MainHero == null)
                return;

            if (!affectedAgent.IsEnemyOf(affectorAgent))
                return;

            var tier = (affectedAgent.Character as CharacterObject)?.Tier ?? 1;
            if (tier < 1)
                tier = 1;
            else if (tier > 6)
                tier = 6;

            var amount = WindsOfMagicRestoreSettings.Instance?.GetWindsForTier(tier) ?? 1f;
            TorWindsApi.AddWinds(amount);
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
