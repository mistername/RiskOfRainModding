using BepInEx.Configuration;
using RoR2;
using RoR2.ConVar;
using System;
using System.Collections.Generic;
using Console = RoR2.Console;

/// <summary>
/// All custom convars
/// </summary>
namespace SettingsPlus.Convars
{
    internal static class Init
    {
        internal static void Hooks()
        {
            On.RoR2.Console.InitConVars += Console_InitConVars;
        }

        private static void Console_InitConVars(On.RoR2.Console.orig_InitConVars orig, Console self)
        {
            orig(self);
            var dictionary = (Dictionary<string, BaseConVar>)typeof(Console).GetField("allConVars", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(self);
            foreach (Settings.BaseCustomSetting customsetting in Settings.listSettings)
            {
                if (dictionary.ContainsKey(customsetting.conVar.name))
                {
                    dictionary[customsetting.conVar.name] = customsetting.conVar;
                }
                else
                {
                    dictionary.Add(customsetting.conVar.name, customsetting.conVar);
                }
            }
        }
    }





    /// <summary>
    /// Vanilla convar using bool with a onValueChanged action
    /// </summary>
    public class BoolConvar : BaseConVar
    {
        public bool value { get; protected set; }

        public BoolConvar(string name, string defaultValue, string helpText, ConVarFlags flags = ConVarFlags.None) : base(name, flags, defaultValue, helpText)
        {

        }

        public void SetBool(bool newValue)
        {
            if(value != newValue)
            {
                value = newValue;

                if(newValue == true)
                {
                    onValueChanged?.Invoke("1");
                }
                else
                {
                    onValueChanged?.Invoke("0");
                }
            }
        }

        public override void SetString(string newValue)
        {
            int num;
            if (TextSerialization.TryParseInvariant(newValue, out num))
            {
                value = (num != 0);

                onValueChanged?.Invoke(newValue);
            }
        }

        public event Action<string> onValueChanged;

        public override string GetString()
        {
            if (!value)
            {
                return "0";
            }
            return "1";
        }
    }

    /// <summary>
    /// Vanilla convar using string with a onValueChanged action
    /// </summary>
    public class StringConVar : BaseConVar
    {
        public string value { get; protected set; }

        public StringConVar(string name, string defaultValue, string helpText, ConVarFlags flags = ConVarFlags.None) : base(name, flags, defaultValue, helpText)
        {
            
        }

        public override void SetString(string newValue)
        {
            if(newValue != GetString())
            {
                value = newValue;

                onValueChanged?.Invoke(newValue);
            }
        }

        public event Action<string> onValueChanged;

        public override string GetString()
        {
            return value;
        }
    }

    /// <summary>
    /// custom BoolConvar using the onValueChanged to change the config
    /// </summary>
    public class ConfigBoolConvar : BoolConvar
    {
        public ConfigBoolConvar(string name, string defaultValue, string helpText, ConfigFile configFile, ConfigDefinition configDefinition, ConVarFlags flags = ConVarFlags.None) : base(name, defaultValue, helpText, flags)
        {
            file = configFile;
            definition = configDefinition;
            SetString(configFile.Wrap<string>(configDefinition).Value);
            onValueChanged += valuechanged;
        }

        internal ConfigFile file;

        internal ConfigDefinition definition;

        private void valuechanged(string newvalue)
        {
            file.Wrap<string>(definition).Value = newvalue;
        }
    }

    /// <summary>
    /// custom StringConvar using the onValueChanged to change the config
    /// </summary>
    public class ConfigStringConvar : StringConVar
    {
        public ConfigStringConvar(string name, string defaultValue, string helpText, ConfigFile configFile, ConfigDefinition configDefinition, ConVarFlags flags = ConVarFlags.None) : base(name, defaultValue, helpText, flags)
        {
            file = configFile;
            definition = configDefinition;
            SetString(configFile.Wrap<string>(configDefinition).Value);
            onValueChanged += valuechanged;
        }

        internal ConfigFile file;

        internal ConfigDefinition definition;

        internal void valuechanged(string newvalue)
        {
            file.Wrap<string>(definition).Value = newvalue;
        }
    }
}
