using BepInEx;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SoundPlusExample
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlusExample : BaseUnityPlugin
    {
        public const string modguid = "com.mistername." + modname;

        const string modname = "SoundPlusExample";
        const string version = "0.2.1";

        const string bankName = "SoundPlusExample.sound";

        const bool embedded = true;

        //You get the eventid from the wwise .txt file exported with your .bnk
        const uint eventid = 853566515;

        internal void Awake()
        {
            //if the bank file is embedded, if not then put a .sound in the plugin folder or any of it's subdirectories
            if (embedded)
            {
                var soundbank = LoadFile(bankName);
                if (soundbank != null)
                {
                    R2API.AssetPlus.SoundBanks.Add(soundbank);
                }
                else
                {
                    UnityEngine.Debug.LogError("soundbank fetching failed");
                }
            }
            On.EntityStates.Mage.Weapon.FireFireBolt.OnEnter += FireBolt_OnEnter;
        }

        //loads an embedded resource
        internal byte[] LoadFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            resourceName = assembly.GetManifestResourceNames()
                .First(str => str.EndsWith(resourceName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadBytes(Convert.ToInt32(stream.Length.ToString()));
            }

        }

        void FireBolt_OnEnter(On.EntityStates.Mage.Weapon.FireFireBolt.orig_OnEnter orig, EntityStates.Mage.Weapon.FireFireBolt self)
        {
            //plays the eventid on the gameobject of the character when it does the firebolt attack (normally only artificer)
            var id = AkSoundEngine.PostEvent(eventid, self.outer.commonComponents.characterBody.coreTransform.gameObject);
            if (id == 0)
            {
                Debug.Log("didn't fire");
            }
            else
            {
                Debug.Log("fired");
            }
            orig(self);
            //you can use the id later to stop the sound while it is still playing
            //xAkSoundEngine.StopPlayingID(id);
        }
    }
}
