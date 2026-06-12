using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class ApplyGeneralDamageModifiersPatch
    {
        private static readonly Type? DamageModelType =
            Type.GetType("TOR_Core.Models.TORAgentApplyDamageModel, TOR_Core");

        private static readonly Type? AttackInformationType =
            Type.GetType("TaleWorlds.MountAndBlade.AttackInformation, TaleWorlds.MountAndBlade");

        private static readonly PropertyInfo? AttackerAgentProperty =
            AttackInformationType?.GetProperty("AttackerAgent");

        private static readonly PropertyInfo? VictimAgentProperty =
            AttackInformationType?.GetProperty("VictimAgent");

        private static readonly PropertyInfo? IsAttackerAgentMountProperty =
            AttackInformationType?.GetProperty("IsAttackerAgentMount");

        private static readonly PropertyInfo? IsVictimAgentMountProperty =
            AttackInformationType?.GetProperty("IsVictimAgentMount");

        public static MethodInfo? TargetMethod()
        {
            return DamageModelType?.GetMethod(
                "ApplyGeneralDamageModifiers",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { AttackInformationType, typeof(AttackCollisionData).MakeByRefType(), typeof(float) },
                null);
        }

        public static void Postfix(object attackInformation, ref float __result)
        {
            if (__result <= 0f || attackInformation == null)
                return;

            var attacker = ResolveAgent(attackInformation, isAttacker: true);
            var victim = ResolveAgent(attackInformation, isAttacker: false);
            if (victim == null)
                return;

            DamageRewardService.TryGrantForCombatDamage(victim, attacker, __result);
        }

        private static Agent? ResolveAgent(object attackInformation, bool isAttacker)
        {
            var agent = (isAttacker ? AttackerAgentProperty : VictimAgentProperty)?.GetValue(attackInformation) as Agent;
            var isMount = (bool)((isAttacker ? IsAttackerAgentMountProperty : IsVictimAgentMountProperty)
                ?.GetValue(attackInformation) ?? false);

            if (agent == null)
                return null;

            return isMount ? agent.RiderAgent : agent;
        }
    }
}
