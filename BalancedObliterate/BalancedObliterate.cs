using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace BalancedObliterate
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BalancedObliterate : BaseUnityPlugin
    {
        internal const string modname = "BalancedObliterate";
        internal const string version = "1.1.0";

        internal static ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        internal static float[] difconfigs;

        public void Awake()
        {
            Run.OnServerGameOver += Run_OnServerGameOver;
            if(Language.GetString("OPTION_OFF", "EN_US") == "OPTION_OFF")
            {
                Language.LoadAllFilesForLanguage("EN_US");
            }
            InitConfig();
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
            bool tmp = float.TryParse(configFile.Wrap(catagorie, description, null, defaultvalue.ToString()).Value, out value);
            if (!tmp)
            {
                Debug.LogError(catagorie + " " + description + " " + "is formatted wrong");
            }
            return tmp;
        }

        private void Run_OnServerGameOver(Run run, GameResultType gameResultType)
        {
            if (gameResultType == GameResultType.Unknown || gameResultType == GameResultType.Won)
            {
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
        }

#if DEBUG
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Run.instance.AdvanceStage(Run.instance.nextStageScene.SceneName);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Run.instance.BeginGameOver(GameResultType.Unknown);
            }
        }
#endif
    }
}
