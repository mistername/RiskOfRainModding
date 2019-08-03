using System;
using System.Collections;
using BepInEx;
using R2API;
using RoR2;
using UnityEngine;

namespace HANDMod
{
    // Token: 0x02000002 RID: 2
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.duduhed.hand", "HAN-D mod", "0.6")]
    public class HANDMod : BaseUnityPlugin
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public void Awake()
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                var display = Resources.Load<GameObject>("prefabs/characterbodies/HANDBody").GetComponent<ModelLocator>().modelTransform.gameObject;
                display.AddComponent<animation>();
                RoR2.SurvivorDef survivorDef = new RoR2.SurvivorDef
                {
                    bodyPrefab = RoR2.BodyCatalog.FindBodyPrefab("HANDBody"),
                    descriptionToken = "HAND_DESCRIPTION",
                    displayPrefab = display,
                    primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                    unlockableName = "",
                    survivorIndex = SurvivorIndex.Count
                };
                survivorDef.bodyPrefab.GetComponent<RoR2.SkillLocator>().primary.noSprint = false;
                survivorDef.bodyPrefab.GetComponent<RoR2.SkillLocator>().secondary.noSprint = false;
                survivorDef.bodyPrefab.GetComponent<RoR2.SkillLocator>().utility.noSprint = false;
                survivorDef.bodyPrefab.GetComponent<RoR2.SkillLocator>().special.noSprint = false;
                survivorDef.bodyPrefab.GetComponent<RoR2.CharacterBody>().crosshairPrefab = RoR2.BodyCatalog.FindBodyPrefab("HuntressBody").GetComponent<RoR2.CharacterBody>().crosshairPrefab;
                SurvivorAPI.SurvivorDefinitions.Insert(6, survivorDef);
            };
            On.EntityStates.HAND.Overclock.FixedUpdate += delegate (On.EntityStates.HAND.Overclock.orig_FixedUpdate orig, EntityStates.HAND.Overclock self)
            {
                self.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.EnrageAncientWisp, 2f);
                orig(self);
            };
            On.RoR2.GlobalEventManager.OnHitEnemy += delegate (On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
            {
                orig(self, damageInfo, victim);
                bool flag = damageInfo.attacker && damageInfo.procCoefficient > 0f;
                if (flag)
                {
                    RoR2.CharacterBody component = damageInfo.attacker.GetComponent<RoR2.CharacterBody>();
                    bool flag2 = component.HasBuff(BuffIndex.EnrageAncientWisp) && component.baseNameToken == "HAND_BODY_NAME";
                    if (flag2)
                    {
                        component.AddTimedBuff(BuffIndex.EnrageAncientWisp, 2f);
                        component.healthComponent.Heal(EntityStates.HAND.Overclock.healPercentage * damageInfo.damage, default(RoR2.ProcChainMask), true);
                    }
                }
            };
            On.RoR2.CharacterModel.UpdateMaterials += delegate (On.RoR2.CharacterModel.orig_UpdateMaterials orig, RoR2.CharacterModel self)
            {
                bool flag = self.body.baseNameToken != "HAND_BODY_NAME";
                if (flag)
                {
                    orig(self);
                }
            };
        }


        //lobby animation
        private class animation : MonoBehaviour
        {

            internal void OnEnable()
            {
                if (gameObject.transform.parent.gameObject.name == "CharacterPad")
                {
                    Debug.Log("animation");
                    var animator = gameObject.GetComponent<Animator>();
                    Shooting(animator);

                }
                else
                {
                    Debug.Log("no animation");
                }
            }

            private void Shooting(Animator animator)
            {
                PlayAnimation("Gesture", "ChargeSlam", "ChargeSlam.playbackRate", EntityStates.HAND.Weapon.ChargeSlam.baseDuration, animator);

                var coroutine = Fire(animator);
                StartCoroutine(coroutine);
            }

            IEnumerator Fire(Animator animator)
            {
                yield return new WaitForSeconds(0.5f);

                PlayAnimation("Gesture", "Slam", "Slam.playbackRate", EntityStates.HAND.Weapon.Slam.baseDuration, animator);

                Destroy(this);
            }

            private void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration, Animator animator)
            {
                int layerIndex = animator.GetLayerIndex(layerName);
                animator.SetFloat(playbackRateParam, 1f);
                animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
                animator.Update(0f);
                float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
                animator.SetFloat(playbackRateParam, length / duration);
            }
        }
    }
}
