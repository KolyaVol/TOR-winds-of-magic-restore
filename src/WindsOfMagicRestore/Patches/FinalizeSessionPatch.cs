using System.Reflection;
using WindsOfMagicRestore.Domain;
using WindsOfMagicRestore.Integration;

namespace WindsOfMagicRestore.Patches
{
    public static class FinalizeSessionPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.FinalizeSessionMethod();

        public static void Postfix(object session)
        {
            HealRewardService.TryGrantForSession(session);
        }
    }
}
