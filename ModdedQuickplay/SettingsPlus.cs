using BepInEx;
using SettingsPlus;
using SettingsPlus.Convars;
using System.Collections.Generic;

namespace ModdedQuickplay
{
    //public partial class Main : BaseUnityPlugin
    //{
    //    static readonly List<BoolConvar> boolConvars = new List<BoolConvar>();

    //    static readonly List<StringConVar> stringCons = new List<StringConVar>();

    //    private void SettingsPlusInit()
    //    {
    //        foreach (var item in file.ConfigDefinitions)
    //        {
    //            if (item.Section == "hosting")
    //            {
    //                if (item.Key != "!MOD GUID!")
    //                {
    //                    var value = file.Wrap<string>(item).Value;
    //                    var convar = new ConfigStringConvar(item.Key, "0", "help", file, item);
    //                    stringCons.Add(convar);

    //                    List<Settings.CustomMultichoiceSetting.choice> choice = new List<Settings.CustomMultichoiceSetting.choice>();

    //                    choice.Add(new Settings.CustomMultichoiceSetting.choice
    //                    {
    //                        DisplayText = "no",
    //                        Value = "-1"
    //                    });

    //                    choice.Add(new Settings.CustomMultichoiceSetting.choice
    //                    {
    //                        DisplayText = "don't care",
    //                        Value = "0"
    //                    });

    //                    choice.Add(new Settings.CustomMultichoiceSetting.choice
    //                    {
    //                        DisplayText = "yes",
    //                        Value = "1"
    //                    });

    //                    Settings.CustomMultichoiceSetting customMultichoice = new Settings.CustomMultichoiceSetting(convar, item.Key, choice);
    //                }
    //            }
    //        }

    //        var convarcatagory = new BoolConvar("catagory", "1", "help");
    //        boolConvars.Add(convarcatagory);
    //        convarcatagory.onValueChanged += Convarcatagory_onValueChanged;

    //        Settings.CustomBoolSetting customsettingcatagory = new Settings.CustomBoolSetting
    //        (
    //            convarcatagory,
    //            modname,
    //            modname
    //        );
    //    }



    //    private void Convarcatagory_onValueChanged(string newvalue)
    //    {
    //        if (newvalue == "1")
    //        {
    //            foreach (var item in SettingsPlus.Main.gameobjects)
    //            {
    //                item.SetActive(true);
    //            }
    //        }
    //        else
    //        {
    //            foreach (var item in SettingsPlus.Main.gameobjects)
    //            {
    //                item.SetActive(false);
    //            }
    //        }
    //    }
    //}
}
