using BepInEx;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BuffDisplayAPI
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class CustomBuffDisplay : BaseUnityPlugin
    {
        const string version = "0.1.0";
        const string modname = "BuffDisplayAPI";

        private static FieldInfo icons = null;

        private static FieldInfo form = null;

        public void Awake()
        {
            On.RoR2.UI.BuffDisplay.AllocateIcons += BuffDisplay_AllocateIcons;
        }

        public void onDestroy()
        {
            On.RoR2.UI.BuffDisplay.AllocateIcons -= BuffDisplay_AllocateIcons;
        }

        private static void BuffDisplay_AllocateIcons(On.RoR2.UI.BuffDisplay.orig_AllocateIcons orig, BuffDisplay self)
        {
            orig(self);
            if (self != null)
            {
                if (self.source != null)
                {
                    if (icons == null)
                    {
                        icons = typeof(BuffDisplay).GetField("buffIcons", BindingFlags.NonPublic | BindingFlags.Instance);
                    }

                    if (form == null)
                    {
                        form = typeof(BuffDisplay).GetField("rectTranform", BindingFlags.NonPublic | BindingFlags.Instance);
                    }

                    List<BuffIcon> buffIcons = (List<BuffIcon>)icons?.GetValue(self);

                    if (buffIcons != null)
                    {
                        RectTransform trans = (RectTransform)form?.GetValue(self);
                        if (trans != null)
                        {
                            CustomBuff(self, trans, buffIcons);
                        }
                        else
                        {
                            Debug.LogError("Can't get tranform");
                        }
                    }
                    else
                    {
                        Debug.LogError("Can't get bufficons");
                    }
                }
            }
        }

        private static void CustomBuff(BuffDisplay self, RectTransform trans, List<BuffIcon> buffIcons)
        {
            Vector2 zero = Vector2.zero;

            if (self.buffIconPrefab)
            {
                for (int i = 0; i < CustomBuffs.buffs.Count; i++)
                {
                    var type = self.source.GetComponentInParent(CustomBuffs.buffs[i].type);
                    if (type == null)
                    {
                        buffIcons.RemoveAll(p => p.buffIndex == BuffIndex.Count + 1 + i);
                    }
                    else
                    {
                        BuffIcon buff = Newbuff(i, trans, buffIcons, self.buffIconPrefab, out bool createdbuff);

                        zero.x += (buffIcons.Count - (createdbuff ? 0 : 1)) * self.iconWidth;
                        buff.rectTransform.anchoredPosition = zero;

                        Effectsbuff(buff, CustomBuffs.buffs[i], type);

                        if (createdbuff)
                        {
                            buffIcons.Add(buff);
                        }
                    }
                }
            }
        }

        private static void Effectsbuff(BuffIcon buff, Buff custombuff, object obj)
        {
            object[] argText = new object[] { string.Empty };
            if (custombuff.text_method == null)
            {
                custombuff.text_method = custombuff.type.GetMethod("Text");
            }
            custombuff.text_method.Invoke(obj, argText);
            buff.stackCount.text = (string)argText[0];

            float alpha;
            object[] argAlpha = new object[] { 1f };
            if (custombuff.alpha_method == null)
            {
                custombuff.alpha_method = custombuff.type.GetMethod("Alpha");
            }
            custombuff.alpha_method.Invoke(obj, argAlpha);
            alpha = (float)argAlpha[0];

            buff.stackCount.alpha = alpha;

            Color color = buff.iconImage.color;

            color.a = alpha;

            buff.iconImage.color = color;
        }

        private static BuffIcon Newbuff(int i, Transform transform, List<BuffIcon> buffIcons, GameObject iconPrefab, out bool newbuff)
        {
            BuffIcon buff = null;
            newbuff = false;
            for (var j = 0; j < buffIcons.Count; j++)
            {
                if (buffIcons[j].buffIndex == BuffIndex.Count + 1 + i)
                {
                    buff = buffIcons[j];
                }
            }
            if (buff == null)
            {
                buff = Instantiate(iconPrefab, transform)?.GetComponent<BuffIcon>();
                buff.buffIndex = BuffIndex.Count + 1 + i;
                buff.buffCount = 1;
                buff.iconImage.sprite = CustomBuffs.buffs[i].sprite;
                newbuff = true;
            }
            return buff;
        }
    }

    public static class CustomBuffs
    {
        public static void Add(Buff buff)
        {
            buffs.Add(buff);
        }

        internal static List<Buff> buffs = new List<Buff>();
    }

    /// <summary>
    /// Contains all data buffdisplayAPI needs for display
    /// </summary>
    public class Buff
    {
        /// <summary>
        /// Holds an unity Sprite for display
        /// </summary>
        public Sprite sprite;

        /// <summary>
        /// Holds the class type for search
        /// </summary>
        public Type type;

        internal MethodInfo text_method = null;

        internal MethodInfo alpha_method = null;
    }
}
