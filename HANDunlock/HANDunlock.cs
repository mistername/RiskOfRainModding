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
        internal const string modname = "HANDunlock";
        internal const string version = "1.0.2";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                {
                    var Bandit = BodyCatalog.FindBodyPrefab("BanditBody");
                    SurvivorDef item = new SurvivorDef
                    {
                        bodyPrefab = Bandit,
                        descriptionToken = "Bandit",
                        displayPrefab = Bandit.GetComponent<ModelLocator>().modelTransform.gameObject,
                        primaryColor = new Color(0.87890625f, 0.662745098f, 0.3725490196f),
                        unlockableName = "",
                        survivorIndex = (SurvivorIndex)int.MaxValue
                    };
                    SurvivorAPI.AddSurvivor(item);
                }
            };
        }
    }
}
