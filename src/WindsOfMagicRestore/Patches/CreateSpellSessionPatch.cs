using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Integration;

namespace WindsOfMagicRestore.Patches
{
    public static class CreateSpellSessionPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.CreateSpellSessionMethod();

        public static void Postfix(Agent caster, object abilityTemplate, int __result)
        {
            SpellCastRegistry.Register(__result, caster);
            AugmentBuffTracker.RegisterFromSpellSession(caster, abilityTemplate, __result);
        }
    }
}
