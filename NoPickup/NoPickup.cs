using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace NoPickup
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Class1 : BaseUnityPlugin
    {
        internal ConfigFile file = new ConfigFile(Paths.ConfigPath +  "\\" + modname + ".cfg", true);

        internal const string modname = "NoPickup";

        internal const string version = "1.1.2";

        internal Dictionary<ItemIndex,Dictionary<string, bool>> dropping = new Dictionary<ItemIndex, Dictionary<string, bool>>();

        public void Awake()
        {
            On.RoR2.RoR2Application.Update += RoR2Application_Update;

            On.RoR2.GenericPickupController.OnTriggerStay += GenericPickupController_OnTriggerStay;
        }

        private void RoR2Application_Update(On.RoR2.RoR2Application.orig_Update orig, RoR2Application self)
        {
            var codename = ItemCatalog.GetItemDef(ItemIndex.Bear)?.nameToken;
            if (codename != null)
            {
                var name = Language.GetString(codename);
                if (name != codename)
                {
                    if (SurvivorCatalog.allSurvivorDefs != null)
                    {
                        if (SurvivorCatalog.allSurvivorDefs.Count() != 0)
                        {
                            var tmp = new Dictionary<string, bool>();
                            configitem("!Item!", "examplecharacter_example", tmp);
                            var survivors = SurvivorCatalog.allSurvivorDefs;
                            for (ItemIndex i = 0; i < ItemIndex.Count; i++)
                            {
                                var item = ItemCatalog.GetItemDef(i);
                                if (item.hidden == false)
                                {
                                    var itemdic = new Dictionary<string, bool>();
                                    dropping.Add(i, itemdic);
                                    var itemname = Language.GetString(item.nameToken);
                                    configitem(itemname, "ALL", itemdic);
                                    foreach (var survivor in survivors)
                                    {
                                        configitem(itemname, survivor.descriptionToken, itemdic);
                                    }
                                }
                            }
                            On.RoR2.RoR2Application.Update -= RoR2Application_Update;
                        }
                    }
                }
            }
        }

        public void configitem(string section, string key, Dictionary<string, bool> dic)
        {
            if(section == "!Item!")
            {
                _ = file.Wrap(section, key.Split('_')[0], "if true the item is able to be auto pickedup by the character, if false auto pickup is disabled, but still able to be picked up by interacting, if ALL = false it overrides all other characters", true);
            }
            else
            {
                ConfigWrapper<bool> config = file.Wrap(section, key.Split('_')[0], null, true);
                dic.Add(key, config.Value);
            }
        }

        private void GenericPickupController_OnTriggerStay(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, Collider other)
        {
            if (NetworkServer.active)
            {
                if (other)
                {
                    bool result = true;
                    var character = other.GetComponent<CharacterBody>();
                    if (character)
                    {
                        if (self != null && self.pickupIndex != null)
                        {
                            var charactername = character.baseNameToken;
                            dropping.TryGetValue(self.pickupIndex.itemIndex, out Dictionary<string, bool> characterdic);
                            if (characterdic != null)
                            {
                                characterdic.TryGetValue(charactername, out result);
                            }
                        }
                    }
                    if (result)
                    {
                        orig(self, other);
                    }
                }
            }
        }
    }
}
