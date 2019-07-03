using BepInEx;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace AllSurvivors
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BanditMod : BaseUnityPlugin
    {
        internal const string modname = "AllSurvivors";
        internal const string version = "1.0.0";

        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {

                foreach (var survivor in BodyCatalog.allBodyPrefabs)
                {
                    SurvivorDef surdef = new SurvivorDef
                    {
                        bodyPrefab = survivor,
                        displayPrefab = survivor?.GetComponent<ModelLocator>()?.modelTransform?.gameObject,
                        descriptionToken = survivor.name,
                        primaryColor = new Color(0.87890625f, 0.662745098f, 0.3725490196f),
                        unlockableName = "",
                        survivorIndex = SurvivorIndex.Count
                    };
                    SurvivorAPI.AddSurvivor(surdef);
                }
            };
        }
    }
}
