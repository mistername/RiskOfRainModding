using BepInEx;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace RemoveArmor
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class RemoveArmor : BaseUnityPlugin
    {
        internal const string modname = "RemoveArmor";
        internal const string version = "1.0.1";

        public void Awake()
        {
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            
        }

        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            foreach (var item in BodyCatalog.allBodyPrefabs)
            {
                item.GetComponent<CharacterBody>().baseArmor = 0f;
                item.GetComponent<CharacterBody>().levelArmor = 0f;
            }
            On.RoR2.BodyCatalog.Init -= BodyCatalog_Init;
        }
    }
}
