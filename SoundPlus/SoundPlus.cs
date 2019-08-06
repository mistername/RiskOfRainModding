using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SoundPlus
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlus : BaseUnityPlugin
    {
        /// <summary>
        /// Use this in as the dependency string
        /// </summary>
        public const string modguid = "com.mistername." + modname;

        internal const string modname = "SoundPlus";
        internal const string version = "0.2.0";

        internal void Awake()
        {
            On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;
        }

        private void RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            orig(self);

            AddBank();
        }

        private void AddBank()
        {
            //finds all .sound files
            var files = Directory.GetFiles(Paths.PluginPath, "*.sound", SearchOption.AllDirectories);

            //adds all .sound files as banks
            foreach (var file in files)
            {
                BankLoading(file);
            }

            //adds each added byte[] bank as a soundbank
            foreach (var bank in SoundBanks.soundBanks)
            {
                BankLoading(bank);
            }
        }

        private static void BankLoading(string file)
        {
            var bank = File.ReadAllBytes(file);

            BankLoading(bank);
        }

        private static void BankLoading(byte[] bank)
        {
            //Creates IntPtr of sufficient size.
            IntPtr intPtrBank = Marshal.AllocHGlobal(bank.Length);

            //copies the byte array to the IntPtr
            Marshal.Copy(bank, 0, intPtrBank, bank.Length);
            
            //Loads the entire IntPtr as a bank
            var result = AkSoundEngine.LoadBank(intPtrBank, uint.Parse(bank.Length.ToString()), out var bankid);
            if(result != AKRESULT.AK_Success)
            {
                Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result);
            }
        }
    }

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

        internal static List<byte[]> soundBanks = new List<byte[]>();

    }
}
