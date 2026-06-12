using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class ApplyGeneralDamageModifiersPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.ApplyGeneralDamageModifiersMethod();

        public static void Postfix(object attackInformation, ref float __result)
        {
            var damage = __result;
            ModGuard.Run("ApplyGeneralDamageModifiers", () => PostfixCore(attackInformation, damage));
        }

        private static void PostfixCore(object attackInformation, float damage)
        {
            if (damage <= 0f || attackInformation == null)
                return;

            var attacker = ResolveAgent(attackInformation, isAttacker: true);
            var victim = ResolveAgent(attackInformation, isAttacker: false);
            if (victim == null)
                return;

            DamageRewardService.TryGrantForCombatDamage(victim, attacker, damage);
        }

        private static Agent? ResolveAgent(object attackInformation, bool isAttacker)
        {
            var agent = (isAttacker ? TorTypes.AttackAttackerAgent : TorTypes.AttackVictimAgent)
                ?.GetValue(attackInformation) as Agent;

            var isMountValue = (isAttacker ? TorTypes.AttackIsAttackerMount : TorTypes.AttackIsVictimMount)
                ?.GetValue(attackInformation);
            var isMount = isMountValue is bool mountFlag && mountFlag;

            if (agent == null)
                return null;

            return isMount ? agent.RiderAgent : agent;
        }
    }
}
