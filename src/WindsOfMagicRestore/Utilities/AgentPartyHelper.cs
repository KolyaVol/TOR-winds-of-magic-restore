using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class AgentPartyHelper
    {
        private static readonly MethodInfo? BelongsToMainParty = Type
            .GetType("TOR_Core.Extensions.AgentExtensions, TOR_Core")
            ?.GetMethod(
                "BelongsToMainParty",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Agent) },
                null);

        public static bool IsBelongsToMainPartyAvailable => BelongsToMainParty != null;

        public static bool IsMainPartyAgent(Agent agent)
        {
            if (agent == null)
                return false;

            if (BelongsToMainParty != null)
                return (bool)BelongsToMainParty.Invoke(null, new object[] { agent });

            return Agent.Main != null && agent.Team != null && agent.Team == Agent.Main.Team;
        }
    }
}
