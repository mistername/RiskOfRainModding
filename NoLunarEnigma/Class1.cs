using BepInEx;
using RoR2;

namespace NoLunarEnigma
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class NoLunarEnigma : BaseUnityPlugin
    {
        const string modname = "NoLunarEnigma";
        const string version = "1.0.0";

        public void Awake()
        {
            On.RoR2.EquipmentCatalog.RegisterEquipment += EquipmentCatalog_RegisterEquipment;
        }

        private void EquipmentCatalog_RegisterEquipment(On.RoR2.EquipmentCatalog.orig_RegisterEquipment orig, EquipmentIndex equipmentIndex, EquipmentDef equipmentDef)
        {
            if (equipmentDef.isLunar)
            {
                equipmentDef.enigmaCompatible = false;
            }
            orig(equipmentIndex, equipmentDef);
        }
    }
}
