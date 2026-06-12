using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class AgentPartyHelper
    {
        public static bool IsBelongsToMainPartyAvailable => TorTypes.BelongsToMainParty != null;

        public static bool IsMainPartyAgent(Agent? agent)
        {
            if (agent == null)
                return false;

            if (TorTypes.BelongsToMainParty != null)
            {
                return ModGuard.Run(
                    "BelongsToMainParty",
                    () => (bool)TorTypes.BelongsToMainParty.Invoke(null, new object[] { agent }),
                    false);
            }

            return Agent.Main != null && agent.Team != null && agent.Team == Agent.Main.Team;
        }
    }
}
