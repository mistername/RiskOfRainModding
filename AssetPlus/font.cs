using BepInEx;
using System;
using System.Collections.Generic;

namespace AssetPlus
{
    /// <summary>
    /// use this class to add fonts
    /// </summary>
    public static class Fonts
    {
        /// <summary>
        /// for adding an TMP_FontAsset inside an seperate assetbundle (.font is loaded automatically)
        /// </summary>
        /// <param name="path">absolute path to the assetbundle</param>
        public static void Add(string path)
        {
            R2API.AssetPlus.Fonts.Add(path);
        }


        /// <summary>
        /// for adding an TMP_FontAsset while it is still in an assetbundle
        /// </summary>
        /// <param name="fontFile">the assetbundle file</param>
        public static void Add(byte[] fontFile)
        {
            R2API.AssetPlus.Fonts.Add(fontFile);
        }

        /// <summary>
        /// for adding an TMP_FontAsset directly
        /// </summary>
        /// <param name="fontAsset">The loaded fontasset</param>
        public static void Add(TMPro.TMP_FontAsset fontAsset)
        {
            R2API.AssetPlus.Fonts.Add(fontAsset);
        }
    }
}
