using AssetPlus;
using BepInEx;

namespace SoundPlusExample
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(AssetPlus.AssetPlus.modguid)]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlusExample : BaseUnityPlugin
    {

        public const string modguid = "com.mistername." + modname;

        //you need to put the .sound bank somewhere in the plugin folder for this to work

        const string modname = "SoundPlusExample";
        const string version = "0.2.0";

        //You get the eventid from the wwise .txt file exported with your .bnk
        const uint eventid = 1256202815;

        internal void Awake()
        {
            On.EntityStates.Mage.Weapon.FireBolt.OnEnter += FireBolt_OnEnter;
        }

        void FireBolt_OnEnter(On.EntityStates.Mage.Weapon.FireBolt.orig_OnEnter orig, EntityStates.BaseState self)
        {
            //plays the eventid on the gameobject of the character when it does the firebolt attack (normally only artificer)
            var id = AkSoundEngine.PostEvent(eventid, self.outer.commonComponents.characterBody.coreTransform.gameObject);
            orig(self);
            //you can use the id later to stop the sound while it is still playing
            //xAkSoundEngine.StopPlayingID(id);
        }
    }
}
