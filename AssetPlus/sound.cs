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
            //finds all .sound files
            var files = LoadFiles("*.sound");

            foreach (var file in files)
            {
                SoundBanks.Add(file);
            }

            //for bank loading
            On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;            

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
            foreach (var bank in SoundBanks.soundBanks)
            {
                BankLoading(bank);
            }
        }

        private static void BankLoading(SoundBanks.Bank bank)
        {
            //Creates IntPtr of sufficient size.
            bank.Memory = Marshal.AllocHGlobal(bank.BankData.Length);

            //copies the byte array to the IntPtr
            Marshal.Copy(bank.BankData, 0, bank.Memory, bank.BankData.Length);

            //Loads the entire IntPtr as a bank
            var result = AkSoundEngine.LoadBank(bank.Memory, uint.Parse(bank.BankData.Length.ToString()), out bank.BankID);
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
        /// Adds a soundbank to load, returns the ID used for unloading
        /// </summary>
        /// <param name="bank">byte array of the entire .bnk file</param>
        public static uint Add(byte[] bank)
        {
            var bankToAdd= new Bank(bank);
            soundBanks.Add(bankToAdd);
            return bankToAdd.PublicID;
        }

        /// <summary>
        /// Adds an external soundbank to load, returns the ID used for unloading (.sound files are loaded automatically)
        /// </summary>
        /// <param name="path">the absolute path to the file</param>
        public static uint Add(string path)
        {
            byte[] bank = File.ReadAllBytes(path);

            return Add(bank);
        }

        /// <summary>
        /// Unloads an bank using the ID (ID is returned at the Add() of the bank)
        /// </summary>
        /// <param name="ID">BankID</param>
        /// <returns></returns>
        public static AKRESULT Remove(uint ID)
        {
            var bankToUnload = soundBanks.Find(bank => bank.PublicID == ID);            
            var result = AkSoundEngine.UnloadBank(bankToUnload.BankID, bankToUnload.Memory);
            if(result != AKRESULT.AK_Success)
            {
                Debug.LogError("Failed to unload bank " + ID.ToString() + ": " + result.ToString());
                return result;
            }
            Marshal.FreeHGlobal(bankToUnload.Memory);
            soundBanks.Remove(bankToUnload);
            return result;
        }

        internal class Bank
        {
            internal Bank(byte[] bankData)
            {
                BankData = bankData;
                PublicID = number++;
            }

            internal byte[] BankData;

            internal uint PublicID;

            internal IntPtr Memory;

            internal uint BankID;
        }

        static uint number = 0;

        internal static List<Bank> soundBanks = new List<Bank>();

    }
}
