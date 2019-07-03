using BepInEx;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace NoOutOfBounds
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername.NoOutOfBounds", "NoOutOfBounds", "1.0.0")]
    public class Teleporter : BaseUnityPlugin
    {
        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                
                foreach (var survivor in RoR2.BodyCatalog.allBodyPrefabs)
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

            On.RoR2.MapZone.TeleportBody += MapZone_TeleportBody;
        }

        private void MapZone_TeleportBody(On.RoR2.MapZone.orig_TeleportBody orig, MapZone self, CharacterBody characterBody)
        {
            if(characterBody.isPlayerControlled != true)
            {
                orig(self, characterBody);
            }

            if (characterBody.isPlayerControlled && Input.GetKeyDown(KeyCode.F9))
            {
                orig(self, characterBody);
            }
        }
    }
}
