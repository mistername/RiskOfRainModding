using BepInEx;
using BepInEx.Configuration;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SettingsPlus.Settings;

namespace SettingsPlus
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(modGUID, "settingsplus", version)]
    public partial class Main : BaseUnityPlugin
    {
        const string version = "0.0.1";
        public const string modGUID = "com.mistername.SettingsPlus";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + "settingsplus" + ".cfg", true);

        void Awake()
        {
            On.RoR2.UI.HeaderNavigationController.RebuildHeaders += GenerateMenu.GenerateNewHeader;
            Convars.Init.Hooks();
        }
    }
}
