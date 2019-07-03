using RoR2.ConVar;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SettingsPlus
{
    public class Settings
    {
        public class BaseCustomSetting
        {
            public BaseCustomSetting(BaseConVar boolConvar, string settingName, string settingText = null)
            {
                conVar = boolConvar;

                settingname = settingName;

                if (settingText == null)
                {
                    settingtext = settingName;
                }
                else
                {
                    settingtext = settingText;
                }
            }

            public BaseConVar conVar;

            public string settingname;

            public string settingtext;

            public Type type;
        }
        
        public class CustomBoolSetting : BaseCustomSetting
        {
            public CustomBoolSetting(Convars.BoolConvar boolConvar, string settingName, string settingText = null) : base(boolConvar, settingName, settingText)
            {
                type = typeof(CustomBoolSetting);
                listSettings.Add(this);
            }
        }

        public class CustomMultichoiceSetting : BaseCustomSetting
        {
            public CustomMultichoiceSetting(Convars.StringConVar boolConvar, string settingName, List<choice> choices,string settingText = null) : base(boolConvar, settingName, settingText)
            {
                type = typeof(CustomMultichoiceSetting);

                Choices = choices.ToArray();

                listSettings.Add(this);
            }

            public choice[] Choices;

            public struct choice
            {
                public string DisplayText;
                public string Value;
            }
        }

        internal static List<BaseCustomSetting> listSettings = new List<BaseCustomSetting>();
    }
}
