using BepInEx;
using RoR2;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace UnlimitedLunar
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class UnlimitedLunar : BaseUnityPlugin
    {
        const string modname = "UnlimitedLunar";
        const string version = "1.0.1";

        public void Awake()
        {
            On.RoR2.UserProfile.XmlUtility.FromXml += XmlUtility_FromXml; ;
        }

        //this should work but it doesn't
        private UserProfile XmlUtility_FromXml(On.RoR2.UserProfile.XmlUtility.orig_FromXml orig, System.Xml.Linq.XDocument doc)
        {
            var profile = orig(doc);
            profile.coins = uint.MaxValue;
            Debug.Log("setting coins");
            return profile;
        }
    }
}