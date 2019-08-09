#define skills
#if skills
#define timer
#endif

using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Bandit;
using EntityStates.Bandit.Timer;
using EntityStates.Commando.CommandoWeapon;
using R2API;
using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace BanditPlus
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.mistername.BuffDisplayAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BanditMod : BaseUnityPlugin
    {
        internal const string modname = "BanditPlus";
        internal const string version = "4.0.4";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            #region survivor
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                {
                    var display = Resources.Load<GameObject>("prefabs/characterbodies/banditbody").GetComponent<ModelLocator>().modelTransform.gameObject;
                    display.AddComponent<animation>();
                    var bandit = BodyCatalog.FindBodyPrefab("BanditBody");
                    SurvivorDef item = new SurvivorDef
                    {
                        bodyPrefab = bandit,
                        descriptionToken = "test",
                        displayPrefab = display,
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
                    SkillManagement.banditskilldescriptions(bandit);
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
            if(BodyCatalog.FindBodyPrefab("BanditBody") != null)
            {
                if (BodyCatalog.FindBodyPrefab("BanditBody").GetComponent<SetStateOnHurt>() != null)
                {
                    if (BodyCatalog.FindBodyPrefab("BanditBody").GetComponent<SetStateOnHurt>().idleStateMachine != null)
                    {
                        if (BodyCatalog.FindBodyPrefab("BanditBody").GetComponent<SetStateOnHurt>().idleStateMachine.Length != 0)
                        {
                            BodyCatalog.FindBodyPrefab("BanditBody").GetComponent<SetStateOnHurt>().idleStateMachine[0] = BodyCatalog.FindBodyPrefab("BanditBody").GetComponent<SetStateOnHurt>().idleStateMachine[1];
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
                if(gameObject.transform.parent.gameObject.name == "CharacterPad")
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
                if (RoR2Application.rng.nextBool)
                {
                    coroutine = Fire();
                }
                else
                {
                    coroutine = Impact();
                }
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
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            GenericSkill special = component.special;
            GenericSkill genericSkill = gameObject.AddComponent<GenericSkill>();

            genericSkill.skillName = "Mortar";
            genericSkill.baseRechargeInterval = 5f;
            genericSkill.baseMaxStock = 1;
            genericSkill.rechargeStock = 1;
            genericSkill.isBullets = false;
            genericSkill.shootDelay = 0.3f;
            genericSkill.beginSkillCooldownOnSkillEnd = false;
            genericSkill.stateMachine = component.special.stateMachine;
            genericSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.Toolbot.AimStunDrone));
            genericSkill.interruptPriority = InterruptPriority.Skill;
            genericSkill.isCombatSkill = true;
            genericSkill.noSprint = false;
            genericSkill.canceledFromSprinting = false;
            genericSkill.mustKeyPress = true;
            genericSkill.requiredStock = 1;
            genericSkill.stockToConsume = 1;
            genericSkill.hasExecutedSuccessfully = false;
            genericSkill.icon = special.icon;

            Destroy(special);
            SkillManagement.SetSkill(ref genericSkill, typeof(EntityStates.Toolbot.AimStunDrone));

            component.special = genericSkill;
        }
    }
}

