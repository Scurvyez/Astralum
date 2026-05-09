using System;
using System.Reflection;
using Astralum.Debugging;
using HarmonyLib;

namespace Astralum.Harmony
{
    public static class HarmonyPatchesUtil
    {
        public static MethodInfo Method(Type type, string methodName, string patchDescription)
        {
            MethodInfo method = AccessTools.Method(type, methodName);
            
            if (method == null)
            {
                AstraLog.Warning($"Could not find {type.Name}.{methodName}. {patchDescription} was not applied.");
            }
            
            return method;
        }
        
        public static MethodInfo RequiredMethod(Type type, string methodName, string patchDescription)
        {
            MethodInfo method = Method(type, methodName, patchDescription);
            
            if (method == null)
            {
                AstraLog.Warning("Required Harmony patch target was missing. Patch setup will stop.");
            }
            
            return method;
        }
        
        public static MethodInfo EnumeratorMoveNext(MethodInfo enumerableMethod, string ownerDescription,
            string patchDescription)
        {
            if (enumerableMethod == null)
            {
                AstraLog.Warning($"Could not find {ownerDescription}. {patchDescription} was not applied.");
                return null;
            }
            
            MethodInfo moveNext = AccessTools.EnumeratorMoveNext(enumerableMethod);
            
            if (moveNext == null)
            {
                AstraLog.Warning($"Could not find {ownerDescription} MoveNext. {patchDescription} was not applied.");
            }

            return moveNext;
        }
        
        public static bool Missing(MethodInfo method)
        {
            return method == null;
        }
    }
}