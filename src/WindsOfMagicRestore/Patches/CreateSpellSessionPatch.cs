using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class CreateSpellSessionPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.CreateSpellSessionMethod();

        public static void Postfix(Agent caster, object abilityTemplate, int __result)
        {
            ModGuard.Run("CreateSpellSession", () =>
                AugmentBuffTracker.RegisterFromSpellSession(caster, abilityTemplate, __result));
        }
    }
}
