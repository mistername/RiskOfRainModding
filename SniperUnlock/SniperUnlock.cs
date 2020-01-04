using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace HANDunlock
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BanditMod : BaseUnityPlugin
    {
        internal const string modname = "SniperUnlock";
        internal const string version = "1.0.4";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                {
                    var sniper = BodyCatalog.FindBodyPrefab("SniperBody");
                    SurvivorDef item = new SurvivorDef
                    {
                        bodyPrefab = sniper,
                        descriptionToken = "Sniper",
                        displayPrefab = sniper,//.GetComponent<ModelLocator>().modelTransform.gameObject,
                        primaryColor = new Color(0.87890625f, 0.662745098f, 0.3725490196f),
                        unlockableName = "",
                        survivorIndex = (SurvivorIndex) int.MaxValue
                    };
                    SurvivorAPI.AddSurvivor(item);
                }
            };
        }
    }
}
