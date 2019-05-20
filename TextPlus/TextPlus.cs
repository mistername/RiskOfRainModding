using BepInEx;
using JetBrains.Annotations;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LanguagePlus
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Language : BaseUnityPlugin
    {
        const string modname = "Text+";
        const string version = "1.0.2";

        public void Awake()
        {
            On.RoR2.Language.LoadAllFilesForLanguage += Language_LoadAllFilesForLanguage;
        }
        public void OnDestroy()
        {
            On.RoR2.Language.LoadAllFilesForLanguage -= Language_LoadAllFilesForLanguage;
        }

        private void Language_LoadAllFilesForLanguage(On.RoR2.Language.orig_LoadAllFilesForLanguage orig, string language)
        {
            orig(language);
            ImportCustomLanguageFiles(language);
        }


        private void ImportCustomLanguageFiles(string language)
        {
            Dictionary<string, string> dictionary = LoadCustomLanguageDictionary(language);

            var files = Directory.GetFiles(Paths.PluginPath, "*.language", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in LoadCustomTokensFromFile(file))
                    {
                        dictionary[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }

        private KeyValuePair<string, string>[] LoadCustomTokensFromFile([NotNull] string path)
        {
            if (File.Exists(path))
            {
                var language = File.ReadAllText(path, Encoding.UTF8);
                try
                {
                    JSONNode jsonnode = JSON.Parse(language);
                    if (jsonnode != null)
                    {
                        JSONNode jsonnode2 = jsonnode["strings"];
                        if (jsonnode2 != null)
                        {
                            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[jsonnode2.Count];
                            int num = 0;
                            foreach (string text in jsonnode2.Keys)
                            {
                                array[num++] = new KeyValuePair<string, string>(text, jsonnode2[text].Value);
                            }
                            return array;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogFormat("Parsing error in language file \"{0}\". Error: {1}", new object[]
                    {
                        path,
                        ex
                    });
                }
            }
            else
            {
                Debug.Log("Error Loading File" + path);
            }
            return Array.Empty<KeyValuePair<string, string>>();
        }

        private Dictionary<string, string> LoadCustomLanguageDictionary(string language)
        {
            Dictionary<string, Dictionary<string, string>> originalDictionary = loadOriginalDictionary();
            if (!originalDictionary.TryGetValue(language, out Dictionary<string, string> dictionary))
            {
                dictionary = new Dictionary<string, string>();
                originalDictionary[language] = dictionary;
            }
            return dictionary;
        }

        private Dictionary<string, Dictionary<string, string>> loadOriginalDictionary()
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
}