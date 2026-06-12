using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellDamagePatch
    {
        private static readonly Type? LogicType =
            Type.GetType("TOR_Core.AbilitySystem.AbilityManagerMissionLogic, TOR_Core");

        public static MethodInfo? TargetMethod()
        {
            if (LogicType == null)
                return null;

            foreach (var method in LogicType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name != "BookSpellDamage")
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length >= 3
                    && parameters[0].ParameterType == typeof(int)
                    && parameters[1].ParameterType == typeof(Agent)
                    && parameters[2].ParameterType == typeof(int))
                {
                    return method;
                }
            }

            return null;
        }

        public static void Postfix(int castId, Agent victim, int damage, object __instance)
        {
            if (castId < 0 || victim == null || damage <= 0 || __instance == null)
                return;

            var caster = SpellSessionResolver.ResolveCaster(__instance, castId);
            DamageRewardService.TryGrantForSpellDamage(victim, caster, damage);
        }
    }
}
