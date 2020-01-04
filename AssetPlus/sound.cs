using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AssetPlus
{
    /// <summary>
    /// class for SoundBanks to load
    /// </summary>
    public static class SoundBanks
    {
        public static uint Add(byte[] bank)
        {
            return R2API.AssetPlus.SoundBanks.Add(bank);
        }

        /// <summary>
        /// Adds an external soundbank to load, returns the ID used for unloading (.sound files are loaded automatically)
        /// </summary>
        /// <param name="path">the absolute path to the file</param>
        public static uint Add(string path)
        {
            return R2API.AssetPlus.SoundBanks.Add(path);
        }

        /// <summary>
        /// Unloads an bank using the ID (ID is returned at the Add() of the bank)
        /// </summary>
        /// <param name="ID">BankID</param>
        /// <returns></returns>
        public static AKRESULT Remove(uint ID)
        {
            return R2API.AssetPlus.SoundBanks.Remove(ID);
        }
    }
}
