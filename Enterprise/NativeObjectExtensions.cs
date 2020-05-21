using System;
using HarmonyLib;
using TaleWorlds.DotNet;

namespace LoadScene
{
    public static class NativeObjectExtensions
    {
        private static readonly AccessTools.FieldRef<NativeObject, UIntPtr> PointerFieldGetter = AccessTools.FieldRefAccess<NativeObject, UIntPtr>("_pointer");

        public static ref UIntPtr RefPointer(this NativeObject nativeObject)
        {
            return ref PointerFieldGetter(nativeObject);
        }
        
    }
}