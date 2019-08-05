using BepInEx;
using UnityEngine;

namespace SoundPlusExample
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.mistername.SoundPlus")]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlusExample : BaseUnityPlugin
    {
        public const string modguid = "com.mistername." + modname;
        internal const string modname = "SoundPlusExample";
        internal const string version = "0.1.0";

        public void Awake()
        {
            On.EntityStates.Mage.Weapon.FireBolt.OnEnter += FireBolt_OnEnter;
        }

        private void FireBolt_OnEnter(On.EntityStates.Mage.Weapon.FireBolt.orig_OnEnter orig, EntityStates.BaseState self)
        {
            Debug.Log("posting event");
            var id = AkSoundEngine.PostEvent(1256202815, self.outer.commonComponents.characterBody.coreTransform.gameObject);
            Debug.Log(id.ToString());
            orig(self);
        }
    }
}
