using BepInEx;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Diagnostics;
using System.Reflection;

namespace IlLine
{
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class IlLine : BaseUnityPlugin
    {
        internal const string modname = nameof(IlLine);
        internal const string version = "1.0.0";

        IlLine()
        {
            new ILHook(typeof(StackTrace).GetMethod("AddFrames", BindingFlags.Instance | BindingFlags.NonPublic), IlHook);
        }

        private void IlHook(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.GotoNext(
                x => x.MatchCallvirt(typeof(StackFrame).GetMethod("GetFileLineNumber", BindingFlags.Instance | BindingFlags.Public))
            );

            cursor.RemoveRange(2);
            cursor.EmitDelegate<Func<StackFrame, string>>(GetLineOrIL);
        }

        private static string GetLineOrIL(StackFrame instace)
        {
            var line = instace.GetFileLineNumber();
            if (line == StackFrame.OFFSET_UNKNOWN || line == 0)
            {
                return "IL_" + instace.GetILOffset().ToString("X4");
            }

            return line.ToString();
        }
    }
}
