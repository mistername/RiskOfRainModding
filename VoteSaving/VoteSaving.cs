using BepInEx;
using BepInEx.Configuration;
using JetBrains.Annotations;
using RoR2;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VoteSaving
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class VoteSaving : BaseUnityPlugin
    {
        internal const string modname = "VoteSaving";
        internal const string version = "1.0.0";

        Dictionary<string, int> savedchoices = new Dictionary<string, int>();
        internal string path;

        public void Awake()
        {
            path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = path.Substring(0, path.LastIndexOf("BepInEx") + "BepInEx".Length);
            path += @"\config\" + modname + ".cfg";
            LoadChoices();

            On.RoR2.PreGameRuleVoteController.GetDefaultChoice += PreGameRuleVoteController_GetDefaultChoice;
            On.RoR2.PreGameRuleVoteController.SetVote += PreGameRuleVoteController_SetVote;
        }

        public void OnDestroy()
        {
            On.RoR2.PreGameRuleVoteController.GetDefaultChoice -= PreGameRuleVoteController_GetDefaultChoice;
            On.RoR2.PreGameRuleVoteController.SetVote -= PreGameRuleVoteController_SetVote;
        }

        private void PreGameRuleVoteController_SetVote(On.RoR2.PreGameRuleVoteController.orig_SetVote orig, PreGameRuleVoteController self, int ruleIndex, int choiceValue)
        {
            string name = RuleCatalog.GetRuleDef(ruleIndex).globalName;
            if(choiceValue != -1)
            {
                if (savedchoices.ContainsKey(name))
                {
                    savedchoices[name] = choiceValue;
                }
                else
                {
                    savedchoices.Add(name, choiceValue);
                }
                WriteChoices();
            }
            orig(self, ruleIndex, choiceValue);
        }

        private RuleChoiceDef PreGameRuleVoteController_GetDefaultChoice(On.RoR2.PreGameRuleVoteController.orig_GetDefaultChoice orig, RoR2.PreGameRuleVoteController self, RoR2.RuleDef ruleDef)
        {
            string name = ruleDef.globalName;
            if (savedchoices.ContainsKey(name))
            {
                return ruleDef.choices[savedchoices[name]];
            }
            else
            {
                return orig(self, ruleDef);
            }
        }

        private void LoadChoices()
        {
            if (File.Exists(path))
            {
                var language = File.ReadAllLines(path, Encoding.UTF8);
                try
                {
                    foreach (var sentence in language)
                    {
                        var pair = sentence.Split((char)61);
                        savedchoices.Add(pair[0], int.Parse(pair[1]));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogFormat("Parsing error in vote file \"{0}\". Error: {1}", new object[]
                    {
                        path,
                        ex
                    });
                }
            }
            else
            {
                Debug.Log("Error Loading File");
            }
        }

        private void WriteChoices()
        {
            var writer = File.CreateText(path);
            try
            {
                foreach (var savedchoice in savedchoices)
                {
                    writer.WriteLine(savedchoice.Key + (char)61 + savedchoice.Value.ToString());
                }
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.LogFormat("writing error in vote file \"{0}\". Error: {1}", new object[]
                {
                    path,
                    ex
                });
            }
        }
    }
}
