using BepInEx;
using BepInEx.Configuration;
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
        internal const string version = "1.2.0";

        internal const float DefaultAlphaTime = 2f;

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        //config values;
        internal static ConfigWrapper<bool> TextBool;
        internal static string BuffToText;
        internal static ConfigWrapper<bool> AlphaBool;
        internal static float AlphaFloat;


        public void Awake()
        {
            On.RoR2.UI.BuffDisplay.UpdateLayout += BuffDisplay_UpdateLayout;
            config();

        }

        private static void config()
        {
            TextBool = file.Wrap("BuffText", "BuffText", "if the bufficons need to display time", true);

            ConfigWrapper<int> Decimals = file.Wrap("BuffText", "Decimals", "Amount of decimals after the timer", 1);
            if (Decimals.Value < 0)
            {
                Debug.LogError("Decimals can not be less than 0, continuing with 0");
                Decimals.Value = 0;
            }
            var builder = new System.Text.StringBuilder("0.");
            for (int i = 0; i < Decimals.Value; i++)
            {
                builder.Append("0");
            }

            BuffToText = builder.ToString();

            AlphaBool = file.Wrap("alpha", "alpha", "if the bufficons need to fade out", false);

            ConfigWrapper<string> alphaTime = file.Wrap("alpha", "alphaTime", "the time when the buffs need to start fading out", DefaultAlphaTime.ToString());

            bool tmp = float.TryParse(alphaTime.Value, out var alpha);
            if (!tmp)
            {
                Debug.LogError("AlphaTime formatted wrong");
                alphaTime.Value = DefaultAlphaTime.ToString();
            }
            AlphaFloat = alpha;
        }

        private void BuffDisplay_UpdateLayout(On.RoR2.UI.BuffDisplay.orig_UpdateLayout orig, BuffDisplay self)
        {
            orig(self);
            //checks if it has a body, should fix ice elites
            if (self.source)
            {
                //using a component to keep track of the lists
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
                    BuffText(bufficon, time);
                    BuffAlpha(bufficon, time);

                    return;
                }
            }
        }

        private static void BuffText(BuffIcon bufficon, float time)
        {
            if (TextBool.Value)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                //prefixes the buff stack count
                if (BuffCatalog.GetBuffDef(bufficon.buffIndex).canStack)
                {
                    builder.Append("x");
                    builder.Append(bufficon.buffCount);
                    builder.Append(" ");
                }

                //adds the buff time to the display
                builder.Append(time.ToString(BuffToText));
                bufficon.stackCount.enabled = true;
                bufficon.stackCount.SetText(builder);
                bufficon.stackCount.enableWordWrapping = false;
            }
        }

        private static void BuffAlpha(BuffIcon bufficon, float time)
        {
            if (AlphaBool.Value)
            {
#if DEBUG
                Debug.LogError("doing alpha");
#endif
                float buffAlpha;
                if (AlphaFloat > time)
                {
                    buffAlpha = (float)System.Math.Sqrt(time / AlphaFloat);
                }
                else
                {
                    buffAlpha = 1f;
                }
#if DEBUG
                Debug.LogWarning(buffAlpha.ToString("0.0"));
#endif

                //alpha of sprite
                Color color = bufficon.iconImage.color;
                color.a = buffAlpha;
                bufficon.iconImage.color = color;

                //alpha of text
                bufficon.stackCount.alpha = buffAlpha;
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
