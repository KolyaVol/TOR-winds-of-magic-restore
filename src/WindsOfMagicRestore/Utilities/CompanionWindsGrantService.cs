using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    internal static class CompanionWindsGrantService
    {
        public static void Grant(float amount, Agent? companionAgent, CompanionWindsRecipientMode mode)
        {
            if (amount <= 0f)
                return;

            var companion = CompanionHelper.GetCompanionHero(companionAgent);

            switch (mode)
            {
                case CompanionWindsRecipientMode.MainHero:
                    TorWindsApi.AddWinds(Hero.MainHero, amount);
                    break;

                case CompanionWindsRecipientMode.Self:
                    if (companion != null)
                        TorWindsApi.AddWinds(companion, amount);
                    break;

                case CompanionWindsRecipientMode.Both:
                    TorWindsApi.AddWinds(Hero.MainHero, amount);
                    if (companion != null)
                        TorWindsApi.AddWinds(companion, amount);
                    break;
            }
        }
    }
}
