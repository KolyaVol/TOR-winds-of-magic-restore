using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class CompanionHelper
    {
        public static bool IsCompanionHero(Hero? hero)
        {
            if (hero == null || hero == Hero.MainHero || !hero.IsAlive)
                return false;

            return hero.IsPlayerCompanion && hero.PartyBelongedTo == MobileParty.MainParty;
        }

        public static Hero? GetCompanionHero(Agent? agent)
        {
            agent = KillCreditHelper.NormalizeAgent(agent);
            var character = agent?.Character as CharacterObject;
            if (character?.IsHero != true)
                return null;

            var hero = character.HeroObject;
            return IsCompanionHero(hero) ? hero : null;
        }

        public static bool IsCompanionAgent(Agent? agent)
        {
            agent = KillCreditHelper.NormalizeAgent(agent);
            var character = agent?.Character as CharacterObject;
            if (character?.IsHero != true)
                return false;

            return IsCompanionHero(character.HeroObject);
        }

        public static bool IsMainHeroOrCompanionAgent(Agent? agent)
        {
            return KillCreditHelper.IsMainHeroAgent(agent) || IsCompanionAgent(agent);
        }

        public static Agent? ResolveAgentForHero(Hero? hero)
        {
            if (hero == null)
                return null;

            if (hero == Hero.MainHero)
                return Agent.Main;

            var mission = Mission.Current;
            if (mission == null)
                return null;

            foreach (var agent in mission.Agents)
            {
                var character = agent?.Character as CharacterObject;
                if (character?.HeroObject != hero)
                    continue;

                return KillCreditHelper.NormalizeAgent(agent);
            }

            return null;
        }
    }
}
