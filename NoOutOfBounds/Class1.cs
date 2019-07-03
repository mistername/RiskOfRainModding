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
