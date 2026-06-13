using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Battle;
using WindsOfMagicRestore.Domain;
using WindsOfMagicRestore.Integration;

namespace WindsOfMagicRestore.Patches
{
    public static class ApplyGeneralDamageModifiersPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.ApplyGeneralDamageModifiersMethod();

        public static void Postfix(AttackInformation attackInformation, ref float __result)
        {
            if (__result <= 0f)
                return;

            var attacker = KillCreditHelper.NormalizeAgent(attackInformation.AttackerAgent);
            var victim = KillCreditHelper.NormalizeAgent(attackInformation.VictimAgent);
            if (victim == null)
                return;

            DamageRewardService.TryGrantForCombatDamage(victim, attacker, __result);
        }
    }
}
