using BepInEx;
using R2API;
using RoR2;
using RoR2.Navigation;
using System;
using UnityEngine;

namespace NoOutOfBounds
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername.OutOfBounds", "NoOutOfBounds", "0.2.1")]
    public class Teleporter : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.MapZone.TeleportBody += MapZone_TeleportBody;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                var characterBody = PlayerCharacterMasterController.instances[0].master.GetBody();
                TeleportCharacter(characterBody);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                foreach (var item in PlayerCharacterMasterController.instances)
                {
                    var characterBody = item.master.GetBody();
                    TeleportCharacter(characterBody);
                }

            }
        }

        private void TeleportCharacter(CharacterBody characterBody)
        {
            if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, characterBody.gameObject.layer))
            {
                SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
                spawnCard.hullSize = characterBody.hullClassification;
                spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
                spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");
                GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    position = characterBody.transform.position
                }, RoR2Application.rng));
                if (gameObject)
                {
                    Debug.Log("tp back");
                    TeleportHelper.TeleportBody(characterBody, gameObject.transform.position);
                    GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(characterBody.gameObject);
                    if (teleportEffectPrefab)
                    {
                        EffectManager.instance.SimpleEffect(teleportEffectPrefab, gameObject.transform.position, Quaternion.identity, true);
                    }
                    UnityEngine.Object.Destroy(gameObject);
                }
                UnityEngine.Object.Destroy(spawnCard);
            }
        }

        private void MapZone_TeleportBody(On.RoR2.MapZone.orig_TeleportBody orig, MapZone self, CharacterBody characterBody)
        {
            if(characterBody.isPlayerControlled != true)
            {
                orig(self, characterBody);
            }

            //if (characterBody.isPlayerControlled && Input.GetKey(KeyCode.F9))
            //{
            //    orig(self, characterBody);
            //}
        }
    }
}
