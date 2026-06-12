using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class KillCreditHelper
    {
        public static Agent? ResolveKillerAgent(Mission mission, Agent? affectorAgent, KillingBlow blow)
        {
            var candidates = CollectCandidates(mission, affectorAgent, blow);
            if (candidates.Count == 0)
                return null;

            foreach (var agent in candidates)
            {
                if (IsMainHeroAgent(agent))
                    return agent;
            }

            foreach (var agent in candidates)
            {
                if (AgentPartyHelper.IsMainPartyAgent(agent))
                    return agent;
            }

            return candidates[0];
        }

        public static Agent? NormalizeAgent(Agent? agent)
        {
            if (agent == null)
                return null;

            if (agent.RiderAgent != null)
                return agent.RiderAgent;

            return agent;
        }

        public static bool IsMainHeroAgent(Agent? agent)
        {
            agent = NormalizeAgent(agent);
            if (agent == null || Agent.Main == null)
                return false;

            var main = Agent.Main;
            if (agent == main || agent.IsMainAgent)
                return true;

            var mainMount = main.MountAgent;
            if (mainMount != null && agent == mainMount)
                return true;

            return agent.RiderAgent == main;
        }

        public static bool IsHostileVictim(Agent victim, Agent killer)
        {
            if (victim == null || killer == null)
                return false;

            if (victim.IsEnemyOf(killer))
                return true;

            if (Agent.Main != null && victim.IsEnemyOf(Agent.Main))
                return true;

            return victim.Team != null
                && killer.Team != null
                && victim.Team.IsEnemyOf(killer.Team);
        }

        public static bool IsFriendlyVictim(Agent victim, Agent attacker)
        {
            var normalizedAttacker = NormalizeAgent(attacker);
            var normalizedVictim = NormalizeAgent(victim);
            if (normalizedAttacker == null || normalizedVictim == null)
                return false;

            if (normalizedAttacker.Index == normalizedVictim.Index)
                return false;

            if (IsHostileVictim(normalizedVictim, normalizedAttacker))
                return false;

            return normalizedVictim.IsFriendOf(normalizedAttacker);
        }

        private static List<Agent> CollectCandidates(Mission mission, Agent? affectorAgent, KillingBlow blow)
        {
            var candidates = new List<Agent>(2);

            AddCandidate(candidates, affectorAgent);

            if (blow.IsValid && blow.OwnerId >= 0 && mission != null)
                AddCandidate(candidates, mission.FindAgentWithIndex(blow.OwnerId));

            return candidates;
        }

        private static void AddCandidate(List<Agent> candidates, Agent? agent)
        {
            agent = NormalizeAgent(agent);
            if (agent == null)
                return;

            if (candidates.Any(existing => existing.Index == agent.Index))
                return;

            candidates.Add(agent);
        }
    }
}
