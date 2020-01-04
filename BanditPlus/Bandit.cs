#define skills
#if skills
#define timer
#endif

using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Bandit;
using EntityStates.Bandit.Timer;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections;
using UnityEngine;

namespace BanditPlus
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.mistername.BuffDisplayAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BanditMod : BaseUnityPlugin
    {
        internal const string modname = "BanditPlus";
        internal const string version = "0.4.5";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            #region survivor
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                {
                    var bandit = BodyCatalog.FindBodyPrefab("BanditBody");
                    //var display = bandit?.GetComponent<ModelLocator>()?.modelTransform?.gameObject;
                    
                    SurvivorDef item = new SurvivorDef
                    {
                        bodyPrefab = bandit,
                        descriptionToken = "test",
                        displayPrefab = bandit,//bandit.GetComponent<CharacterBody>().modelLocator.modelTransform.gameObject,
                        primaryColor = new Color(0.87890625f, 0.662745098f, 0.3725490196f),
                        unlockableName = "",
                        survivorIndex = SurvivorIndex.Count
                    };
                    #region skills
#if skills
                    Primary.SetPrimary(bandit);
                    PrepSecondary.SetSecondary(bandit);
                    Banditspecial(bandit);
                    EntityStates.Bandit.Utility.SetUtility(bandit);
#endif
                    #endregion skills
                    //SkillManagement.banditskilldescriptions(bandit);
                    SurvivorAPI.AddSurvivor(item);
                }
            };

            StartCoroutine(FixIce());

            #endregion
            #region timer
#if timer
            Timer.Init();
#endif
            #endregion
        }

        //FUCK HOPOO - fixes ice explosion killing people
        IEnumerator FixIce()
        {
            while (true)
            {
                GameObject bandit = BodyCatalog.FindBodyPrefab("BanditBody");
                if (bandit != null)
                {
                    SetStateOnHurt setStateOnHurt = bandit.GetComponent<SetStateOnHurt>();
                    if (setStateOnHurt != null)
                    {
                        var ildeStateMachine = setStateOnHurt.idleStateMachine;
                        if (ildeStateMachine != null)
                        {
                            
                            if (ildeStateMachine.Length != 0)
                            {
                                ildeStateMachine[0] = ildeStateMachine[1];
                                yield return null;
                            }
                        }
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private class animation : MonoBehaviour
        {
            internal void OnEnable()
            {
                if (gameObject.transform.parent.gameObject.name == "CharacterPad")
                {
#if DEBUG
                    Debug.Log("animation");
#endif
                    Animation();

                }
#if DEBUG
                else
                {
                    Debug.Log("no animation");
                }
#endif
                Destroy(this);
            }

            private void Animation()
            {
                IEnumerator coroutine;
                //if (RoR2Application.rng.nextBool)
                //{
                    coroutine = Fire();
                //}
                //else
                //{
                //    coroutine = Impact();
                //}
                StartCoroutine(coroutine);
            }

            IEnumerator Fire()
            {
                var animator = gameObject.GetComponent<Animator>();

                PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", 0.5f, animator);
                PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", 0.5f, animator);

                yield return new WaitForSeconds(0.5f);

                int layerIndex = animator.GetLayerIndex("Gesture, Additive");
                animator.PlayInFixedTime("FireRevolver", layerIndex, 0f);

                int layerIndex2 = animator.GetLayerIndex("Gesture, Override");
                animator.PlayInFixedTime("FireRevolver", layerIndex2, 0f);
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

            IEnumerator Impact()
            {
                var animator = gameObject.GetComponent<Animator>();

                if (animator)
                {
                    int layerIndex = animator.GetLayerIndex("Gesture, Smokescreen");
                    animator.SetFloat("CastSmokescreen.playbackRate", 1f);
                    animator.CrossFadeInFixedTime("CastSmokescreen", 0.2f, layerIndex);
                    animator.Update(0f);
                    float length = animator.GetNextAnimatorStateInfo(layerIndex).length;
                    animator.SetFloat("CastSmokescreen.playbackRate", length / 1f);
                }

                yield return new WaitForSeconds(0.6f);

                EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/smokescreeneffect"), new EffectData
                {
                    origin = gameObject.transform.position
                }, false);
                int layerIndeximpact = animator.GetLayerIndex("Impact");
                animator.SetLayerWeight(layerIndeximpact, 1f);
                animator.PlayInFixedTime("LightImpact", layerIndeximpact, 0f);
            }
        }

        private void Banditspecial(GameObject gameObject)
        {
            var component = gameObject.GetComponent<SkillLocator>();
            var special = component.special;

            var skillDef = (SkillDef)ScriptableObject.CreateInstance(typeof(SkillDef));

            skillDef.skillName = "Mortar";
            skillDef.baseRechargeInterval = 5f;
            skillDef.baseMaxStock = 1;
            skillDef.rechargeStock = 1;
            skillDef.isBullets = false;
            skillDef.shootDelay = 0.3f;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Toolbot.AimStunDrone));
            skillDef.interruptPriority = InterruptPriority.Skill;
            skillDef.isCombatSkill = true;
            skillDef.noSprint = false;
            skillDef.canceledFromSprinting = false;
            skillDef.mustKeyPress = true;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 1;
            //skillDef.icon = special.icon;

            var variant = new SkillFamily.Variant
            {
                skillDef = skillDef
            };


            var skillFamily = special.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length);
            skillFamily.variants[skillFamily.variants.Length -1] = variant;
        }
    }
}

