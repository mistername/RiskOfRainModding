using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using RoR2;
using UnityEngine;
using static RoR2.Chat;

namespace BeeMoviePlus
{
    [BepInPlugin("com.mistername." + MODNAME, MODNAME, VERSION)]
    public class BeeMoviePlus : BaseUnityPlugin
    {
        internal const string MODNAME = "BeeMoviePlus";
        internal const string VERSION = "1.0.0";

        public void Awake()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            var coroutine = Bee();
            StartCoroutine(coroutine);
        }

        IEnumerator Bee()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var item in assembly.GetManifestResourceNames())
            {
                Debug.Log(item);
            }
            using (Stream stream = assembly.GetManifestResourceStream("BeeMovie.Movie.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    SendBroadcastChat(new SimpleChatMessage { baseToken = "<color=#e5eefc>{0}</color>", paramTokens = new[] { line } });
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}
