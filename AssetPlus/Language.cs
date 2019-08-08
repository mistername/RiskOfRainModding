using BepInEx;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AssetPlus
{
    public partial class AssetPlus : BaseUnityPlugin
    {
        private void LanguageAwake()
        {
            var languagePaths = LoadFiles("*.language");
            foreach (var path in languagePaths)
            {
                Languages.AddPath(path);
            }
            On.RoR2.Language.LoadAllFilesForLanguage += Language_LoadAllFilesForLanguage;
        }

        private static bool Language_LoadAllFilesForLanguage(On.RoR2.Language.orig_LoadAllFilesForLanguage orig, string language)
        {
            var tmp = orig(language);
            ImportCustomLanguageFiles(language);
            return tmp;
        }

        private static void ImportCustomLanguageFiles(string language)
        {
            Dictionary<string, string> dictionary = LoadCustomLanguageDictionary(language);

            foreach (var file in Languages.languages)
            {
                foreach (KeyValuePair<string, string> keyValuePair in LoadCustomTokensFromFile(file, language))
                {
                    dictionary[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }

        //based upon RoR2.language.LoadTokensFromFile but with specific language support
        private static IEnumerable<KeyValuePair<string, string>> LoadCustomTokensFromFile(string file, string language)
        {
            try
            {
                JSONNode jsonnode = JSON.Parse(file);
                if (jsonnode != null)
                {
                    JSONNode generic = jsonnode["strings"];
                    if (generic != null)
                    {
                        KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[generic.Count];
                        int num = 0;
                        foreach (string text in generic.Keys)
                        {
                            array[num++] = new KeyValuePair<string, string>(text, generic[text].Value);
                        }
                        return array;
                    }

                    JSONNode specific = jsonnode[language];
                    if (specific != null)
                    {
                        KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[specific.Count];
                        int num = 0;
                        foreach (string text in specific.Keys)
                        {
                            array[num++] = new KeyValuePair<string, string>(text, specific[text].Value);
                        }
                        return array;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogFormat("Parsing error in language file , Error: {0}", new object[]
                {
                        ex
                });
            }
            return Array.Empty<KeyValuePair<string, string>>();
        }

        private static Dictionary<string, string> LoadCustomLanguageDictionary(string language)
        {
            Dictionary<string, Dictionary<string, string>> originalDictionary = loadOriginalDictionary();
            if (!originalDictionary.TryGetValue(language, out Dictionary<string, string> dictionary))
            {
                dictionary = new Dictionary<string, string>();
                originalDictionary[language] = dictionary;
            }
            return dictionary;
        }

        private static Dictionary<string, Dictionary<string, string>> loadOriginalDictionary()
        {
            var dictionary = typeof(RoR2.Language).GetField("languageDictionaries", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if (dictionary == null)
            {
                return new Dictionary<string, Dictionary<string, string>>();
            }
            else
            {
                return (Dictionary<string, Dictionary<string, string>>)dictionary;
            }
        }
    }

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
            if (File.Exists(path))
            {
                languages.Add(File.ReadAllText(path));
            }
        }

        /// <summary>
        /// Adding an file which is read into an string
        /// </summary>
        /// <param name="file">entire file as string</param>
        public static void Add(string file)
        {
            languages.Add(file);
        }


        internal static List<string> languages = new List<string>();
    }
}
