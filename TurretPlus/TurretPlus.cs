#define turretinventory

using BepInEx;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using System.Reflection;
using RoR2.CharacterAI;
using UnityEngine.Networking;
using RoR2.Projectile;
using System.Collections.Generic;

namespace TurretPlus
{

    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class AllSurvivors : BaseUnityPlugin
    {
        internal const string modname = "TurretPlus";
        internal const string version = "0.1.0";

        public static ConfigWrapper<bool> ConfigShouldRunMod { get; private set; }
        int currentRule = 3;
        bool lastWasRebar = false;


        public void Awake()//Code that runs when the game starts
        {
            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter += FireMegaTurret_OnEnter;

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            //On.RoR2.EngiMineController.Explode += EngiMineController_Explode;

            On.RoR2.CharacterBody.HandleConstructTurret += CharacterBody_HandleConstructTurret;

#if turretinventory
            On.RoR2.CharacterMaster.AddDeployable += CharacterMaster_AddDeployable;
#endif
        }


        float defaultspeed = 0f;
        float defaultleveldamage = 0f;
        float defaultdamage = 0f;

        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();

            defaultspeed = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed;
            defaultdamage = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage;
            defaultleveldamage = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage;
        }


        bool didReflect = false;

        private void FireMegaTurret_OnEnter(On.EntityStates.Drone.DroneWeapon.FireMegaTurret.orig_OnEnter orig, EntityStates.BaseState self)
        {
            if (!didReflect)
            {
                typeof(RoR2.Console).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret").GetField("force", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(self, 0.1f);
                didReflect = true;
            }
            orig(self);
        }

        private void CharacterBody_HandleConstructTurret(On.RoR2.CharacterBody.orig_HandleConstructTurret orig, NetworkMessage netMsg)
        {
            switch (currentRule)
            {
                case 1:
                    {
                        SetToDefault();
                        break;
                    }
                case 2:
                    {
                        SetToRail();
                        break;
                    }
                case 3:
                    {
                        SetToMinigun();
                        break;
                    }
                default:
                    {
                        if (lastWasRebar == false)
                        {
                            SetToRail();
                            lastWasRebar = true;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Machine Gun]"
                            });
                        }
                        else
                        {
                            SetToMinigun();
                            lastWasRebar = false;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Railgun]"
                            });
                        }
                        break;
                    }
            }

            orig(netMsg);
        }

#region SetTurret
        private void SetToDefault()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.EngiTurret.EngiTurretWeapon.FireGauss");
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = defaultspeed;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = defaultdamage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = defaultleveldamage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Weak Boi Default";

            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 60;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                }
            }
        }

        private static void SetToMinigun()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret");
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 1f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 2f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 0.4f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = ".50 Cal Maxim";
            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 100;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                }
            }
        }

        private static void SetToRail()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Toolbot.FireSpear);
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 0.5f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 13f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 2.12f;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Portable Railgun";
            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 1024;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                }
            }
        }
#endregion

#region turretinventory
#if turretinventory
        private void CharacterMaster_AddDeployable(On.RoR2.CharacterMaster.orig_AddDeployable orig, CharacterMaster self, Deployable deployable, DeployableSlot slot)
        {
            orig(self, deployable, slot);
            var turretinventory = self.gameObject.GetComponent<turretinventory>();
            if(turretinventory == null)
            {
                turretinventory = self.gameObject.AddComponent<turretinventory>();
            }

            turretinventory.deployables =  DeployList(self);
            turretinventory.characterMaster = self;
        }

        private List<DeployableInfo> DeployList(CharacterMaster characterMaster)
        {
            var type = typeof(CharacterMaster);
            var fieldinfo = type.GetField("deployablesList", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = fieldinfo.GetValue(characterMaster);
            return (List<DeployableInfo>)value;
        }

        internal class turretinventory : MonoBehaviour
        {
            public CharacterMaster characterMaster;
            public List<DeployableInfo> deployables;

            public void Update()
            {
                foreach (var deployable in deployables)
                {
                    inventory(deployable);
                }
            }

            private static void inventory(DeployableInfo deployable)
            {
                if (deployable.slot == DeployableSlot.EngiTurret)
                {
                    Inventory inventory = getinventory(deployable);
                    changeinventory(deployable, inventory);
                }
            }

            private static void changeinventory(DeployableInfo deployable, Inventory inventory)
            {
                int extralife = inventory.GetItemCount(ItemIndex.ExtraLife);
                int extralifeconsumed = inventory.GetItemCount(ItemIndex.ExtraLifeConsumed);
                inventory.CopyItemsFrom(deployable.deployable.ownerMaster.inventory);
                inventory.ResetItem(ItemIndex.WardOnLevel);
                inventory.ResetItem(ItemIndex.BeetleGland);
                inventory.ResetItem(ItemIndex.CrippleWardOnLevel);


                inventory.ResetItem(ItemIndex.ExtraLife);
                inventory.ResetItem(ItemIndex.ExtraLifeConsumed);
                inventory.GiveItem(ItemIndex.ExtraLife, extralife);
                inventory.GiveItem(ItemIndex.ExtraLifeConsumed, extralifeconsumed);
            }

            private static Inventory getinventory(DeployableInfo deployable)
            {
                return deployable.deployable.GetComponent<CharacterMaster>().inventory;
            }
#endif
#endregion turretinventory
        }

        public void Update()//Code that runs once a frame
        {

            if (Input.GetKeyDown(KeyCode.F1))//Code that runs once you have pressed 'F1'
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "Changing Turret Placement Rule"
                });

                switch (currentRule)
                {
                    case 1:
                        currentRule = 2;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Railgun Only]"
                        });
                        break;
                    case 2:
                        currentRule = 3;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Railgun And Machinegun]"
                        });
                        break;
                    case 3:
                        currentRule = 1;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Machinegun Only]"
                        });
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))//Code that runs once you have pressed 'F2'
            {

                if (currentRule == 3)
                {
                    switch (lastWasRebar)
                    {
                        case false:
                            lastWasRebar = true;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Machine Gun]"
                            });
                            break;
                        case true:
                            lastWasRebar = false;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Railgun]"
                            });
                            break;
                    }
                }
            }
            
        }
    }
}
