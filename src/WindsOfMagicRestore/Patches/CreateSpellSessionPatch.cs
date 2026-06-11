using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class CreateSpellSessionPatch
    {
        public static MethodInfo? TargetMethod()
        {
            var logicType = Type.GetType("TOR_Core.AbilitySystem.AbilityManagerMissionLogic, TOR_Core");
            if (logicType == null)
                return null;

            var abilityTemplateType = Type.GetType("TOR_Core.AbilitySystem.AbilityTemplate, TOR_Core");
            if (abilityTemplateType == null)
                return null;

            return AccessTools.Method(logicType, "CreateSpellSession", new[] { typeof(Agent), abilityTemplateType });
        }

        public static void Postfix(Agent caster, object abilityTemplate, int __result)
        {
            AugmentBuffTracker.RegisterFromSpellSession(caster, abilityTemplate, __result);
        }
    }
}
