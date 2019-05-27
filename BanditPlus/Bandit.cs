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
using System;
using UnityEngine;

namespace BanditPlus
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.mistername.BuffDisplayAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BanditMod : BaseUnityPlugin
    {
        internal const string modname = "BanditPlus";
        internal const string version = "4.0.2";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            #region survivor
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                {
                    
                    var bandit = BodyCatalog.FindBodyPrefab("BanditBody");
                    SurvivorDef item = new SurvivorDef
                    {
                        bodyPrefab = bandit,
                        descriptionToken = "test",
                        displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
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
            #endregion
            #region timer
            #if timer
            Timer.Init();
            #endif
            #endregion
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

