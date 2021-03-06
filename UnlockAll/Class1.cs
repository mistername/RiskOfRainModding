﻿using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace UnlockAll
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Hj.HjUpdaterAPI.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class UnlockAll : BaseUnityPlugin
    {
        const string modname = "UnlockAll";
        const string version = "1.0.5";

        public void Awake()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Hj.HjUpdaterAPI.GUID))
            {
                Updater();
            }

            On.RoR2.UserProfile.HasUnlockable_string += (o, s, i) => true;
            On.RoR2.UserProfile.HasUnlockable_UnlockableDef += (o, s, i) => true;
            On.RoR2.UserProfile.HasSurvivorUnlocked += (o, s, i) => true;
            On.RoR2.UserProfile.HasDiscoveredPickup += (o, s, i) => true;
            On.RoR2.UserProfile.HasAchievement += (o, s, i) => true;
            On.RoR2.UserProfile.CanSeeAchievement += (o, s, i) => true;
            On.RoR2.Stats.StatSheet.HasUnlockable += (o, s, i) => true;
            On.RoR2.Run.IsUnlockableUnlocked += Run_IsUnlockableUnlocked;
            On.RoR2.Run.DoesEveryoneHaveThisUnlockableUnlocked += Run_DoesEveryoneHaveThisUnlockableUnlocked;
            On.RoR2.PreGameController.AnyUserHasUnlockable += (o, s) => true;
        }

        private bool Run_DoesEveryoneHaveThisUnlockableUnlocked(On.RoR2.Run.orig_DoesEveryoneHaveThisUnlockableUnlocked orig, Run self, string unlockableName)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::DoesEveryoneHaveThisUnlockableUnlocked(System.String)' called on client");
                return false;
            }
            return true;
        }

        private static void Updater()
        {
            Hj.HjUpdaterAPI.Register(modname);
        }

        private bool Run_IsUnlockableUnlocked(On.RoR2.Run.orig_IsUnlockableUnlocked orig, Run self, string unlockableName)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::IsUnlockableUnlocked(System.String)' called on client");
                return false;
            }
            return true;
        }
    }
}
