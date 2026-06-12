using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellKillPatch
    {
        private static readonly Type? LogicType =
            Type.GetType("TOR_Core.AbilitySystem.AbilityManagerMissionLogic, TOR_Core");

        public static MethodInfo? TargetMethod()
        {
            return LogicType?.GetMethod(
                "BookSpellKill",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(Agent) },
                null);
        }

        public static void Postfix(int castId, Agent victim, object __instance)
        {
            if (castId < 0 || victim == null || __instance == null)
                return;

            var caster = SpellSessionResolver.ResolveCaster(__instance, castId);
            KillRewardService.TryGrantForSpellKill(victim, caster);
        }
    }
}
