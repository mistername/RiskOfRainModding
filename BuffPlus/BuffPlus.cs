using BepInEx;
using RoR2;
using RoR2.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BuffTime
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class BuffPlus : BaseUnityPlugin
    {
        internal const string modname = "BuffPlus";
        internal const string version = "1.1.0";

        public void Awake()
        {
            On.RoR2.UI.BuffDisplay.UpdateLayout += BuffDisplay_UpdateLayout;
        }

        private void BuffDisplay_UpdateLayout(On.RoR2.UI.BuffDisplay.orig_UpdateLayout orig, BuffDisplay self)
        {
            orig(self);
            if (self.source)
            {
                var bufficons = self.GetComponent<bufficons>();
                if (bufficons == null)
                {
                    List<BuffIcon> listbufficons = (List<BuffIcon>)icons.GetValue(self);
                    bufficons = self.gameObject.AddComponent<bufficons>();
                    bufficons.buffIcons = listbufficons;
                    bufficons.timedbuffs = (IList)buffs.GetValue(self.source);
                }

                checkBuffs(self, bufficons);
            }
        }

        private static void checkBuffs(BuffDisplay self, bufficons bufficons)
        {
            foreach (var bufficon in bufficons.buffIcons)
            {
                if (self.source.HasBuff(bufficon.buffIndex))
                {
                    editBuffs(bufficons, bufficon);
                }
            }
        }

        private static void editBuffs(bufficons bufficons, BuffIcon bufficon)
        {
            foreach (var item in bufficons.timedbuffs)
            {
                if ((BuffIndex)index.GetValue(item) == bufficon.buffIndex)
                {
                    var time = (float)timer.GetValue(item);
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    builder.Clear();

                    if (BuffCatalog.GetBuffDef(bufficon.buffIndex).canStack)
                    {
                        
                        builder.Append("x");
                        builder.Append(bufficon.buffCount);
                        builder.Append(" ");
                        
                    }
                    builder.Append(time.ToString("0.0"));
                    bufficon.stackCount.enabled = true;
                    bufficon.stackCount.SetText(builder);
                    bufficon.stackCount.enableWordWrapping = false;
                    return;
                }
            }
        }

        private static readonly FieldInfo timer = typeof(CharacterBody).GetNestedType("TimedBuff", BindingFlags.NonPublic | BindingFlags.Instance).GetField("timer");

        private static readonly FieldInfo index = typeof(CharacterBody).GetNestedType("TimedBuff", BindingFlags.NonPublic | BindingFlags.Instance).GetField("buffIndex");

        private static readonly FieldInfo icons = typeof(BuffDisplay).GetField("buffIcons", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo buffs = typeof(CharacterBody).GetField("timedBuffs", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    internal class bufficons : MonoBehaviour
    {
        internal List<BuffIcon> buffIcons;
        internal IList timedbuffs;
    }
}
