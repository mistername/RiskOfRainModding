#undef MUSIC

using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AssetPlus
{
    public partial class AssetPlus : BaseUnityPlugin
    {
        internal static void SoundAwake()
        {
            //for bank loading
            On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;

            //finds all .sound files
            var files = LoadFiles("*.sound");

            foreach (var file in files)
            {
                SoundBanks.Add(file);
            }

#if MUSIC
            //for changing the scene music
            SceneCatalog.onMostRecentSceneDefChanged += SceneCatalog_onMostRecentSceneDefChanged;
#endif
        }

        private static void RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            orig(self);
            //must be after OnLoad as it must be after the initialization of the init bank
            AddBank();
        }

        private static void AddBank()
        {
            //adds each added byte[] bank as a soundbank
            foreach (byte[] bank in SoundBanks.soundBanks)
            {
                BankLoading(bank);
            }
        }

        private static void BankLoading(byte[] bank)
        {
            //Creates IntPtr of sufficient size.
            IntPtr intPtrBank = Marshal.AllocHGlobal(bank.Length);

            //copies the byte array to the IntPtr
            Marshal.Copy(bank, 0, intPtrBank, bank.Length);

            //Loads the entire IntPtr as a bank
            var result = AkSoundEngine.LoadBank(intPtrBank, uint.Parse(bank.Length.ToString()), out var bankid);
            if (result != AKRESULT.AK_Success)
            {
                Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result);
            }
        }



#if MUSIC
        private void SceneCatalog_onMostRecentSceneDefChanged(SceneDef scene)
        {
            ReplaceMusic(scene, ref scene.songName, SceneMusic.NormalMusicList);
            ReplaceMusic(scene, ref scene.bossSongName, SceneMusic.BossMusicList);
        }

        private static void ReplaceMusic(SceneDef scene, ref string Songtype, Dictionary<string, List<string>> musicList)
        {
            Debug.Log("checking music");
            //checks if there are custom songs for the scene
            if (musicList.ContainsKey(scene.sceneName))
            {
                Debug.Log("editing music");
                //adds the default one to the list if not in it
                //if (!musicList[scene.sceneName].Contains(Songtype))
                //{
                //    musicList[scene.sceneName].Add(Songtype);
                //}
                //takes a random one from the list and put it in
                int randomInt = RoR2Application.rng.RangeInt(0, musicList.Count - 1);
                Songtype = musicList[scene.sceneName][randomInt];
            }
        }
#endif
    }




#if MUSIC
    /// <summary>
    /// class for Music replacements
    /// </summary>
    public static class SceneMusic
    {
        /// <summary>
        /// adds music to the specific scene, either normal music or boss music
        /// </summary>
        /// <param name="sceneName">name of the scene in the game files</param>
        /// <param name="bossMusic">replace the boss music or the normal music</param>
        /// <param name="eventName">name of the wwise event</param>
        public static void Add(string sceneName, bool bossMusic, string eventName)
        {
            if (bossMusic)
            {
                if (BossMusicList.ContainsKey(sceneName))
                {
                    BossMusicList[sceneName].Add(eventName);
                }
                else
                {
                    BossMusicList.Add(sceneName, new List<string>());
                    BossMusicList[sceneName].Add(eventName);
                }
            }
            else
            {
                if (NormalMusicList.ContainsKey(sceneName))
                {
                    NormalMusicList[sceneName].Add(eventName);
                }
                else
                {
                    NormalMusicList.Add(sceneName, new List<string>());
                    NormalMusicList[sceneName].Add(eventName);
                }
            }
        }

        internal static Dictionary<string, List<string>> NormalMusicList = new Dictionary<string, List<string>>();
        internal static Dictionary<string, List<string>> BossMusicList = new Dictionary<string, List<string>>();
    }
#endif

    /// <summary>
    /// class for SoundBanks to load
    /// </summary>
    public static class SoundBanks
    {
        /// <summary>
        /// Adds a soundbank to load
        /// </summary>
        /// <param name="bank">byte array of the entire .bnk file</param>
        public static void Add(byte[] bank)
        {
            soundBanks.Add(bank);
        }

        /// <summary>
        /// Adds an external soundbank to load (.sound files are loaded automatically)
        /// </summary>
        /// <param name="path">the absolute path to the file</param>
        public static void Add(string path)
        {
            byte[] bank = File.ReadAllBytes(path);

            soundBanks.Add(bank);
        }

        internal static List<byte[]> soundBanks = new List<byte[]>();

    }
}
