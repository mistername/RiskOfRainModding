using EntityStates;
using EntityStates.Bandit;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BanditPlus
{
    public class SkillManagement
    {
        public static FieldInfo field;
        public static void SetSkill(ref GenericSkill skillslot, Type skill)
        {
            var statestype = new SerializableEntityStateType(skill);
            object box = statestype;
            if (field == null)
            {
                field = typeof(SerializableEntityStateType).GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            field.SetValue(box, skill.AssemblyQualifiedName);
            statestype = (SerializableEntityStateType)box;
            skillslot.activationState = statestype;
        }

        public static void banditskilldescriptions(GameObject bandit)
        {
            SkillLocator component = bandit.GetComponent<SkillLocator>();

            component.primary.skillNameToken = "Rapidshot";
            component.primary.skillDescriptionToken =
                "Shoots a pistol with 8 bullets for <color=#FFFF00>" +
                Primary.damageCoefficient.ToString("P0").Replace(" ", string.Empty) +
                " damage</color>. Able to quickshoot if skillbutton is clicked in rapid succession.";

            component.secondary.skillNameToken = "Lights out";
            component.secondary.skillDescriptionToken = "Shoots a finishing shot for <color=#FFFF00>";
            component.secondary.skillDescriptionToken += Secondary.damageCoefficient.ToString("P0").Replace(" ", string.Empty);
            component.secondary.skillDescriptionToken += " damage</color>. On direct kill";
            if(EntityStates.Bandit.Timer.Timer.flag) { component.secondary.skillDescriptionToken += " or on kill " + EntityStates.Bandit.Timer.Timer.timeStart.ToString() + " seconds after a hit"; }
            component.secondary.skillDescriptionToken += " with <color=#ADD8E6>Lights out</color> resets cooldown.";

            component.utility.skillNameToken = "Smokescreen";
            component.utility.skillDescriptionToken =
                "Jumps into a Smokescreen. grants a <color=#ADD8E6>speed up</color> and <color=#ADD8E6>invisible</color> buff for <color=#FFFF00>" +
                EntityStates.Commando.CommandoWeapon.CastSmokescreen.stealthDuration.ToString() +
                " seconds </color>. deal <color=#ADD8E6>stun</color> for <color=#FFFF00>" +
                EntityStates.Commando.CommandoWeapon.CastSmokescreen.damageCoefficient.ToString("P0").Replace(" ", string.Empty) +
                " damage</color> to the surrounding area.";

            component.special.skillNameToken = "Mortar";
            component.special.skillDescriptionToken =
                "Shoots an grenade which turn into mini stun grenades which give <color=#ADD8E6>stun</color> debuff";
        }

        public static bool configfloat(string catagorie, string description, float defaultvalue, out float value)
        {
            bool tmp = float.TryParse((BanditMod.file.Wrap(catagorie, description, null, defaultvalue.ToString()).Value), out value);
            if (!tmp)
            {
                UnityEngine.Debug.LogError(catagorie + " " + description + " " + "is formatted wrong");
            }
            return tmp;
        }
    }
}
