using BepInEx;
using System;
using UnityEngine;

namespace examplesetting
{
    [BepInDependency(SettingsPlus.Main.modGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(modGUID, "settingsplusexample", version)]
    public partial class Main : BaseUnityPlugin
    {
        const string version = "0.0.1";
        public const string modGUID = "com.mistername.SettingsPlusExample";
        public void Awake()
        {
            SettingsPlus.Convars.BoolConvar convar = new SettingsPlus.Convars.BoolConvar("TestSetting", "0", "this is a test");
            convar.onValueChanged += Convar_onValueChanged;
            SettingsPlus.Settings.CustomBoolSetting boolSetting = new SettingsPlus.Settings.CustomBoolSetting(convar, "TestSetting", "testText", "optionalTestText");
        }

        private void Convar_onValueChanged(string obj)
        {
            Debug.Log("changed to: " + obj);
        }
    }
}
