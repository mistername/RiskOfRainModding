﻿using BepInEx;
using System;
using System.IO;

namespace Pixelfont
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Main : BaseUnityPlugin
    {
        internal const string modname = "FontPlus";
        internal const string version = "1.0.0";

        TMPro.TMP_FontAsset font;

        public void Awake()
        {
            try
            {
                font = UnityEngine.AssetBundle.LoadFromFile(getFont()).LoadAllAssets<TMPro.TMP_FontAsset>()[0];
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }

            On.RoR2.UI.HGTextMeshProUGUI.OnCurrentLanguageChanged += HGTextMeshProUGUI_OnCurrentLanguageChanged;
            On.RoR2.Language.SetCurrentLanguage += Language_SetCurrentLanguage;
        }

        private void Language_SetCurrentLanguage(On.RoR2.Language.orig_SetCurrentLanguage orig, string language)
        {
            orig(language);
            RoR2.UI.HGTextMeshProUGUI.defaultLanguageFont = font;
        }

        private void HGTextMeshProUGUI_OnCurrentLanguageChanged(On.RoR2.UI.HGTextMeshProUGUI.orig_OnCurrentLanguageChanged orig)
        {
            RoR2.UI.HGTextMeshProUGUI.defaultLanguageFont = font;
        }

        private string getFont()
        {
            var files = Directory.GetFiles(Paths.PluginPath, "*.font", SearchOption.AllDirectories);
            return files[0];
        }
    }
}
