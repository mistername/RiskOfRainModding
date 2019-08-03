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
using System;
using System.Linq;
using R2API.Utils;

namespace TurretPlus
{

    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class TurretPlus : BaseUnityPlugin
    {
        internal const string modname = "TurretPlus";
        internal const string version = "0.1.0";

        public static ConfigFile config = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        int currentRule = 3;


        public void Awake()//Code that runs when the game starts
        {
            Resources.Load<GameObject>("prefabs/projectiles/engiseekergrenadeprojectile").AddComponent<SphereCollider>();
            var missilecontrol = Resources.Load<GameObject>("prefabs/projectiles/engiseekergrenadeprojectile").GetComponent<MissileController>();

            missilecontrol.maxSeekDistance = config.Wrap("SeekerGrenades", "maxSeekDistance", null, 10).Value;
            missilecontrol.deathTimer = config.Wrap("SeekerGrenades", "LifeTime", null, 16).Value;
            missilecontrol.delayTimer = 0.2f;
            missilecontrol.maxVelocity = config.Wrap("SeekerGrenades", "MaxVelocity", null, 25).Value;
            missilecontrol.giveupTimer = missilecontrol.deathTimer - 2f;

            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter += FireMegaTurret_OnEnter;

            //On.RoR2.EngiMineController.Explode += EngiMineController_Explode;

            //On.RoR2.CharacterBody.HandleConstructTurret += CharacterBody_HandleConstructTurret;

#if turretinventory
            //On.RoR2.CharacterMaster.AddDeployable += CharacterMaster_AddDeployable;

            On.RoR2.CharacterBody.HandleConstructTurret += CharacterBody_HandleConstructTurret;


            var stringBuffs = config.Wrap("Shared", "buffs", null, string.Join("," , new []{ (int)BuffIndex.FullCrit, (int)BuffIndex.Energized,  (int)BuffIndex.TonicBuff, (int)BuffIndex.AffixBlue, (int)BuffIndex.AffixPoison, (int)BuffIndex.AffixRed, (int)BuffIndex.AffixWhite })).Value.Split(',').ToArray();
            List<BuffIndex> buffIndices = new List<BuffIndex>();
            foreach (var item in stringBuffs)
            {
                buffIndices.Add((BuffIndex) int.Parse(item));
            }
            turretinventory.Buffs = buffIndices.ToArray();
#endif
        }

        private void CharacterBody_HandleConstructTurret(On.RoR2.CharacterBody.orig_HandleConstructTurret orig, NetworkMessage netMsg)
        {
            var message = netMsg.ReadMessage<ConstructTurretMessage>();
            netMsg.reader.SeekZero();
            string turretType = "0";
            CSteamID steamID = new CSteamID();
            
            foreach (var item in PlayerCharacterMasterController.instances)
            {
                if (item.GetFieldValue<CharacterBody>("body") == message.builder.GetComponent<CharacterBody>())
                {
                    steamID = item.GetFieldValue<NetworkUser>("resolvedNetworkUserInstance").id.steamId;
                    Debug.Log(steamID.value.ToString());
                    break;
                    //NetworkUser.readOnlyInstancesList.First(p => p.);
                }
            }
            if (RoR2Application.isInSinglePlayer)
            {
                turretType = currentRule.ToString();
            }
            else
            {
                if(steamID != null)
                {
                    turretType = Facepunch.Steamworks.Client.Instance.Lobby.GetMemberData(steamID.value, "Turret");
                }
            }

            if (turretType == "RailGun" || turretType == "2")
            {
                SetToRail();
            }
            else if (turretType == "MiniGun" || turretType == "3")
            {
                SetToMinigun();
            }
            else
            {
                SetToDefault();
            }

            orig(netMsg);
        }

        private class ConstructTurretMessage : MessageBase
        {
            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(this.builder);
                writer.Write(this.position);
                writer.Write(this.rotation);
            }

            public override void Deserialize(NetworkReader reader)
            {
                this.builder = reader.ReadGameObject();
                this.position = reader.ReadVector3();
                this.rotation = reader.ReadQuaternion();
            }

            public GameObject builder;

            public Vector3 position;

            public Quaternion rotation;
        }

        private void FireMegaTurret_OnEnter(On.EntityStates.Drone.DroneWeapon.FireMegaTurret.orig_OnEnter orig, EntityStates.BaseState self)
        {
            //sets the force of the minigun lower
            typeof(RoR2.Console).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret").GetField("force", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(self, 0.1f);
            orig(self);
            //desubcribe itself from the On.
            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter -= FireMegaTurret_OnEnter;
        }

        //private void CharacterBody_HandleConstructTurret(On.RoR2.CharacterBody.orig_HandleConstructTurret orig, NetworkMessage netMsg)
        //{
        //    if (change)
        //    {
        //        switch (currentRule)
        //        {
        //            case 1:
        //                {
        //                    SetToDefault();
        //                    break;
        //                }
        //            case 2:
        //                {
        //                    SetToRail();
        //                    break;
        //                }
        //            case 3:
        //                {
        //                    SetToMinigun();
        //                    break;
        //                }
        //            default:
        //                {
        //                    break;
        //                }
        //        }
        //    }

        //    orig(netMsg);
        //}

        #region SetTurret
        readonly float default_speed = float.Parse(config.Wrap("BaseTurret", "baseAttackSpeed", "", 1f.ToString()).Value);
        readonly float default_damage = float.Parse(config.Wrap("BaseTurret", "baseDamage", "", 19f.ToString()).Value);
        readonly float default_leveldamage = float.Parse(config.Wrap("BaseTurret", "levelDamage", "", 3.8f.ToString()).Value);
        readonly float default_distance = config.Wrap("BaseTurret", "maxDistance", null, 120).Value;

        readonly float MiniGun_speed = float.Parse(config.Wrap("MiniGun", "baseAttackSpeed", "", 2f.ToString()).Value);
        readonly float miniGun_damage = float.Parse(config.Wrap("MiniGun", "baseDamage", "", 1.25f.ToString()).Value);
        readonly float miniGun_leveldamage = float.Parse(config.Wrap("MiniGun", "levelDamage", "", 0.3f.ToString()).Value);
        readonly float MiniGun_distance = config.Wrap("MiniGun", "maxDistance", null, 80).Value;

        readonly float RailGun_speed = float.Parse(config.Wrap("RailGun", "baseAttackSpeed", "", 0.5f.ToString()).Value);
        readonly float RailGun_damage = float.Parse(config.Wrap("RailGun", "baseDamage", "", 13.66666f.ToString()).Value);
        readonly float RailGun_leveldamage = float.Parse(config.Wrap("RailGun", "levelDamage", "", 3f.ToString()).Value);
        readonly float RailGun_distance = config.Wrap("RailGun", "maxDistance", null, 999999).Value;

        private void SetToDefault()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.EngiTurret.EngiTurretWeapon.FireGauss");
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = default_speed;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = default_damage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = default_leveldamage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Weak Boi Default";

            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = default_distance;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                }
            }
        }

        private void SetToMinigun()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret");
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = MiniGun_speed;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = miniGun_damage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = miniGun_leveldamage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = ".50 Cal Maxim";
            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = MiniGun_distance;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                }
            }
        }
        
        private void SetToRail()
        {
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Toolbot.FireSpear);

            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = RailGun_speed;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = RailGun_damage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = RailGun_leveldamage;
            BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Portable Railgun";
            for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
            {   
                if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                {
                    MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = RailGun_distance;
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
            string turretType;
            if (RoR2Application.isInSinglePlayer)
            {
                turretType = currentRule.ToString();
            }
            else
            {
                turretType = Facepunch.Steamworks.Client.Instance.Lobby.GetMemberData(self.GetComponent<PlayerCharacterMasterController>().netId.Value, "Turret");
            }

            if(turretType == "RailGun" || turretType == "2")
            {
                SetToRail();
            }
            else if(turretType == "MiniGun" || turretType == "3")
            {
                SetToMinigun();
            }
            else
            {
                SetToDefault();
            }


            orig(self, deployable, slot);
            var turretinventory = self.gameObject.GetComponent<turretinventory>();
            if (turretinventory == null)
            {
                turretinventory = self.gameObject.AddComponent<turretinventory>();
            }

            turretinventory.deployables = DeployList(self);
            turretinventory.characterMaster = self;
        }

        readonly FieldInfo deployablesListField = typeof(CharacterMaster).GetField("deployablesList", BindingFlags.NonPublic | BindingFlags.Instance);

        private List<DeployableInfo> DeployList(CharacterMaster characterMaster)
        {
            var value = deployablesListField.GetValue(characterMaster);
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

                    changeinventory(deployable);
                    changebuffs(deployable);
                }
            }

            public static BuffIndex[] Buffs;

            private static void changebuffs(DeployableInfo deployable)
            {
                foreach (var buff in Buffs)
                {
                    if (deployable.deployable.ownerMaster.GetBody().HasBuff(buff))
                    {
                        deployable.deployable.GetComponent<CharacterMaster>().GetBody().AddBuff(buff);
                    }
                    else
                    {
                        if (deployable.deployable.GetComponent<CharacterMaster>().GetBody().HasBuff(buff))
                        {
                            deployable.deployable.GetComponent<CharacterMaster>().GetBody().RemoveBuff(buff);
                        }
                    }
                }
            }

            private static void changeinventory(DeployableInfo deployable)
            {
                Inventory inventory = getinventory(deployable);
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

        bool seeker = false;

        readonly KeyCode TurretCode = (KeyCode) config.Wrap("Keys", "TurretChanging", "See https://pastebin.com/yYRCdM6i for the keys to number", 282).Value;

        readonly KeyCode GrenadeCode = (KeyCode)config.Wrap("Keys", "GrenadeChanging", "See https://pastebin.com/yYRCdM6i for the keys to number", 283).Value;

        FieldInfo grenadeField = typeof(RoR2Application).Assembly.GetType("EntityStates.Engi.EngiWeapon.FireGrenades").GetField("projectilePrefab", BindingFlags.Public | BindingFlags.Static);

        public void Update()//Code that runs once a frame
        {

            if (Input.GetKeyDown(TurretCode))//Code that runs once you have pressed 'F1'
            {
                Chat.AddMessage("Changing Turret Placement Rule");

                switch (currentRule)
                {
                    case 1:
                        currentRule = 2;
                        if (Facepunch.Steamworks.Client.Instance != null)
                        {
                            Facepunch.Steamworks.Client.Instance.Lobby.SetMemberData("Turret", "RailGun");
                        }
                        Chat.AddMessage("Turret Placement Rule is now: [Railgun]");
                        break;
                    case 2:
                        currentRule = 3;
                        if (Facepunch.Steamworks.Client.Instance != null)
                        {
                            Facepunch.Steamworks.Client.Instance.Lobby.SetMemberData("Turret", "MiniGun");
                        }
                        Chat.AddMessage("Turret Placement Rule is now: [Minigun]");
                        break;
                    case 3:
                        currentRule = 1;
                        if (Facepunch.Steamworks.Client.Instance != null)
                        {
                            Facepunch.Steamworks.Client.Instance.Lobby.SetMemberData("Turret", "Default");
                        }
                        Chat.AddMessage("Turret Placement Rule is now: [WeakBoy]");
                        break;
                }
            }

            if (Input.GetKeyDown(GrenadeCode))
            {
                if (seeker)
                {
                    grenadeField.SetValue(null, Resources.Load<GameObject>("prefabs/projectiles/engiseekergrenadeprojectile"));
                    Chat.AddMessage("Grenade projectile is now: [Seeker]");
                }
                else
                {
                    grenadeField.SetValue(null, Resources.Load<GameObject>("prefabs/projectiles/engigrenadeprojectile"));
                    Chat.AddMessage("Grenade projectile is now: [Default]");
                }
                seeker = !seeker;
            }
        }
    }
}
