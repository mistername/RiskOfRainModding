using BepInEx;
using System;

namespace NoItemDisplay
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class NoItemDisplay : BaseUnityPlugin
    {
        internal const string modname = nameof(NoItemDisplay);
        internal const string version = "1.0.1";

        public void Awake()
        {
            On.RoR2.CharacterModel.EnableItemDisplay += this.CharacterModel_EnableItemDisplay;
            On.RoR2.CharacterModel.SetEquipmentDisplay += CharacterModel_SetEquipmentDisplay;
        }

        private void CharacterModel_SetEquipmentDisplay(On.RoR2.CharacterModel.orig_SetEquipmentDisplay orig, RoR2.CharacterModel self, RoR2.EquipmentIndex newEquipmentIndex)
        {
            return;
        }

        private void CharacterModel_EnableItemDisplay(On.RoR2.CharacterModel.orig_EnableItemDisplay orig, global::RoR2.CharacterModel self, global::RoR2.ItemIndex itemIndex)
        {
            return;
        }
    }
}
