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
            public BaseCustomSetting(BaseConVar boolConvar, string settingName, string settingText = null, string optionalText = "")
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

                this.optionalText = optionalText;

                listSettings.Add(this);
            }

            public BaseConVar conVar;

            public string settingname;

            public string settingtext;

            public string optionalText;
        }
        
        public class CustomBoolSetting : BaseCustomSetting
        {
            public CustomBoolSetting(Convars.BoolConvar boolConvar, string settingName, string settingText = null, string optionalText = "") : base(boolConvar, settingName, settingText, optionalText)
            {
                
            }
        }

        public class CustomMultichoiceSetting : BaseCustomSetting
        {
            public CustomMultichoiceSetting(Convars.StringConVar boolConvar, string settingName, List<choice> choices, string settingText = null, string optionalText = "") : base(boolConvar, settingName, settingText, optionalText)
            {
                Choices = choices.ToArray();
            }

            internal choice[] Choices;

            public struct choice
            {
                public string DisplayText;
                public string Value;
            }
        }

        internal static List<BaseCustomSetting> listSettings = new List<BaseCustomSetting>();
    }
}
