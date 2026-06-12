using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class ApplyGeneralDamageModifiersPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.ApplyGeneralDamageModifiersMethod();

        public static void Postfix(AttackInformation attackInformation, ref float __result)
        {
            var damage = __result;
            ModGuard.Run("ApplyGeneralDamageModifiers", () => PostfixCore(attackInformation, damage));
        }

        private static void PostfixCore(AttackInformation attackInformation, float damage)
        {
            if (damage <= 0f)
                return;

            var attacker = ResolveAgent(attackInformation, isAttacker: true);
            var victim = ResolveAgent(attackInformation, isAttacker: false);
            if (victim == null)
                return;

            DamageRewardService.TryGrantForCombatDamage(victim, attacker, damage);
        }

        private static Agent? ResolveAgent(AttackInformation attackInformation, bool isAttacker)
        {
            var agent = isAttacker ? attackInformation.AttackerAgent : attackInformation.VictimAgent;
            var isMount = isAttacker ? attackInformation.IsAttackerAgentMount : attackInformation.IsVictimAgentMount;

            if (agent == null)
                return null;

            return isMount ? agent.RiderAgent : agent;
        }
    }
}
