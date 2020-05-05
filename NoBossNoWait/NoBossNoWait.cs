using BepInEx;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using R2API.Utils;
using RoR2;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace teleporter
{
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class NoBossNoWait : BaseUnityPlugin
    {
        internal const string modname = nameof(NoBossNoWait);
        internal const string version = "1.1.3";

        public delegate void Orig(object exception, out string message, out string stacktrace);
        public delegate void hook_LineNumber(Orig orig, object exception, out string message, out string stacktrace);
        public delegate string Orig2(Exception self, bool a);
        public delegate string tmp_hook(Orig2 orig2, Exception self, bool a);

        public delegate string orig3(Exception e, bool a);
        public delegate string dsad(orig3 orig, Exception e, bool a);

        NoBossNoWait()
        {
            new ILHook(typeof(StackTrace).GetMethod("AddFrames", BindingFlags.Instance | BindingFlags.NonPublic), test);
        }

        private void test(ILContext il)
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
            if (line == 0)
            {
                return "IL_" + instace.GetILOffset().ToString("X4");
            }

            return line.ToString();
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                _ = Exception("H", 0);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private Exception Exception(string v, int i)
        {
            if (i != 10)
            {
                return Exception(v,++i);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
