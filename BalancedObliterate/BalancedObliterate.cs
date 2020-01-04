using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace BalancedObliterate
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Hj.HjUpdaterAPI.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BalancedObliterate : BaseUnityPlugin
    {
        internal const string modname = "BalancedObliterate";
        internal const string version = "1.1.6";

        internal static ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        internal static float[] difconfigs;

        public void Awake()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Hj.HjUpdaterAPI.GUID))
            {
                Updater();
            }

            Run.OnServerGameOver += Run_OnServerGameOver;

            //loads language file if not already loaded for config.
            if(Language.GetString("OPTION_OFF", "EN_US") == "OPTION_OFF")
            {
                Language.LoadAllFilesForLanguage("EN_US");
            }

            InitConfig();
        }

        private static void Updater()
        {
            Hj.HjUpdaterAPI.Register(modname);
        }

        public void onDestroy()
        {
            Run.OnServerGameOver -= Run_OnServerGameOver;
        }


        private void InitConfig()
        {
            List<float> configList = new List<float>();
            for (DifficultyIndex i = DifficultyIndex.Easy; i < DifficultyIndex.Count; i++)
            {
                var difficulty = DifficultyCatalog.GetDifficultyDef(i);
                name = Language.GetString(difficulty.nameToken, "EN_US");
                float multiplier;
                const string catagorie = "DifficultyCoinMultipliers";
                if (i == DifficultyIndex.Easy)
                {
                    configfloat(catagorie, name, 0.5f, out multiplier);
                }
                else if (i == DifficultyIndex.Hard)
                {
                    configfloat(catagorie, name, 2f, out multiplier);
                }
                else
                {
                    configfloat(catagorie, name, 1f, out multiplier);
                }

                configList.Insert((int)i, multiplier);
            }
            difconfigs = configList.ToArray();
        }

        internal static bool configfloat(string catagorie, string description, float defaultvalue, out float value)
        {
            ConfigDefinition configDefinition = new ConfigDefinition(catagorie, description);
            var bindConfig = configFile.Bind(configDefinition, defaultvalue);
            value = bindConfig.Value;
            return true;
        }

        private void Run_OnServerGameOver(Run run, GameResultType gameResultType)
        {
            if (gameResultType != GameResultType.Unknown && gameResultType != GameResultType.Won) return;
            float multiplier = difconfigs[(int)run.selectedDifficulty];
            float lunarcoins = (run.stageClearCount * multiplier) - 5f;
            if (lunarcoins > 0)
            {
                uint totalcoins = Convert.ToUInt32(Math.Ceiling(lunarcoins));
                for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
                {
                    NetworkUser networkUser = NetworkUser.readOnlyInstancesList[i];
                    if (networkUser && networkUser.isParticipating)
                    {
                        networkUser.AwardLunarCoins(totalcoins);
                    }
                }
            }
        }

#if DEBUG
        public void Update()
        {
            //finish stage on F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Run.instance.AdvanceStage(Run.instance.nextStageScene.SceneName);
            }

            //obliterate on F2
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Run.instance.BeginGameOver(GameResultType.Unknown);
            }
        }
#endif
    }
}
