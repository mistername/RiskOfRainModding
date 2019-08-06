using BepInEx;
using R2API;
using SoundPlus;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SoundPlusExample
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(SoundPlus.SoundPlus.modguid)]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlusExample : BaseUnityPlugin
    {
        public const string modguid = "com.mistername." + modname;

        const string modname = "SoundPlusExample";
        const string version = "0.2.0";

        const string bankName = "SoundPlusExample.sound";

        const bool embedded = true;

        //You get the eventid from the wwise .txt file exported with your .bnk
        const uint eventid = 1256202815;

        internal void Awake()
        {
            //if the bank file is embedded, if not then put a .sound in the plugin folder or any of it's subdirectories
            if (embedded)
            {
                var soundbank = LoadFile(bankName);
                if (soundbank != null)
                {
                    SoundBanks.Add(soundbank);
                }
                else
                {
                    UnityEngine.Debug.LogError("soundbank fetching failed");
                }
            }
            On.EntityStates.Mage.Weapon.FireBolt.OnEnter += FireBolt_OnEnter;
        }

        //loads an embedded resource
        internal byte[] LoadFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resourceName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadBytes(Convert.ToInt32(stream.Length.ToString()));
            }

        }

        void FireBolt_OnEnter(On.EntityStates.Mage.Weapon.FireBolt.orig_OnEnter orig, EntityStates.BaseState self)
        {
            //plays the eventid on the gameobject of the character when it does the firebolt attack (normally only artificer)
            var id = AkSoundEngine.PostEvent(eventid, self.outer.commonComponents.characterBody.coreTransform.gameObject);
            orig(self);
            //you can use the id later to stop the sound while it is still playing
            //AkSoundEngine.StopPlayingID(id);
        }
    }
}
