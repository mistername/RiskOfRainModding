using BepInEx;
using EntityStates;
using System;
using UnityEngine;

namespace ArmsRace
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class ArmsRace : BaseUnityPlugin
    {
        //TODO wait for snackalack for model


        internal const string modname = "ArmsRace";
        internal const string version = "1.0.0";

        public void Awake()
        {
            //all attackdrones attacks
            On.EntityStates.Drone.DroneWeapon.FireGatling.OnEnter += FireGatling_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter += FireMegaTurret_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.OnEnter += FireMissileBarrage_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireTwinRocket.OnEnter += FireTwinRocket_OnEnter;
            //ItemLib.CustomItem customItem = new ItemLib.CustomItem();
        }

        private void checkMissile(BaseState self)
        {
            throw new NotImplementedException();
        }

        //TODO after each of these item effect
        private void FireTwinRocket_OnEnter(On.EntityStates.Drone.DroneWeapon.FireTwinRocket.orig_OnEnter orig, BaseState self)
        {
            orig(self);
            //if(self.outer.commonComponents.characterBody.inventory.GetItemCount())
            checkMissile(self);
        }



        private void FireMissileBarrage_OnEnter(On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);
        }

        private void FireMegaTurret_OnEnter(On.EntityStates.Drone.DroneWeapon.FireMegaTurret.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);
        }

        private void FireGatling_OnEnter(On.EntityStates.Drone.DroneWeapon.FireGatling.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);
        }
    }
}
