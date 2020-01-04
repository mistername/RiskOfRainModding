using BepInEx;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AssetPlus
{
    /// <summary>
    /// class for language files to load
    /// </summary>
    public static class Languages
    {
        /// <summary>
        /// adding an file via path (.language is added automatically )
        /// </summary>
        /// <param name="path">absolute path to file</param>
        public static void AddPath(string path)
        {
            R2API.AssetPlus.Languages.AddPath(path);
        }

        /// <summary>
        /// Adds a single languagetoken and value
        /// </summary>
        /// <param name="key">Token the game asks</param>
        /// <param name="value">Value it gives back</param>
        public static void AddToken(string key, string value)
        {
            R2API.AssetPlus.Languages.AddToken(key,value);
        }

        /// <summary>
        /// Adds a single languagetoken and value to a specific language
        /// </summary>
        /// <param name="key">Token the game asks</param>
        /// <param name="value">Value it gives back</param>
        /// <param name="language">Language you want to add this to</param>
        public static void AddToken(string key, string value, string language)
        {
            R2API.AssetPlus.Languages.AddToken(key, value,language);
        }

        /// <summary>
        /// Adding an file which is read into an string
        /// </summary>
        /// <param name="file">entire file as string</param>
        public static void Add(string file)
        {
            R2API.AssetPlus.Languages.Add(file);
        }
    }
}
