using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BanditPlus
{
    public class oldbuff : MonoBehaviour
    {
        public static void Init(Sprite image)
        {
            sprite = image;
            On.RoR2.UI.BuffDisplay.AllocateIcons += BuffDisplay_AllocateIcons;
        }

        public static Sprite sprite;

        public static FieldInfo icons = null;

        public static FieldInfo form = null;

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
                            Buff(self, trans, ref buffIcons);
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

        private static void Buff(BuffDisplay self, RectTransform trans, ref List<BuffIcon> buffIcons)
        {
            if (self.buffIconPrefab)
            {
                var type = self.source.GetComponentInParent<counting>();
                if (type != null)
                {
                    BuffIcon buff = null;
                    for (int i = 0; i < buffIcons.Count; i++)
                    {
                        if (buffIcons[i].buffIndex == BuffIndex.Count + 412)
                        {
                            buff = buffIcons[i];
                            break;
                        }
                    }

                    Vector2 zero = Vector2.zero;

                    if (buff == null)
                    {
                        buff = Instantiate(self.buffIconPrefab, trans)?.GetComponent<BuffIcon>();
                        buff.buffIndex = BuffIndex.Count + 412;
                        zero.x += buffIcons.Count * self.iconWidth;
                        buff.buffCount = 1;

                    }
                    else
                    {
                        zero.x += (buffIcons.Count - 1) * self.iconWidth;
                    }
                    buff.rectTransform.anchoredPosition = zero;

                    if (EntityStates.Bandit.Timer.Timer.numbers == false)
                    {
                        buff.stackCount.text = string.Empty;
                    }
                    else
                    {
                        buff.stackCount.text = type.timeLeft.ToString("0.0");
                    }

                    if (type.timeLeft < EntityStates.Bandit.Timer.Timer.fadeout)
                    {
                        buff.stackCount.alpha = (float)Math.Sqrt((double)type.timeLeft / EntityStates.Bandit.Timer.Timer.fadeout);
                    }
                    else
                    {
                        buff.stackCount.alpha = 1f;
                    }

                    if (buff.stackCount.alpha < 1f)
                    {
                        Color color = buff.iconImage.color;

                        color.a = buff.stackCount.alpha;

                        buff.iconImage.color = color;
                    }

                    buff.iconImage.sprite = sprite;
                    buffIcons.Add(buff);
                }
            }
        }
    }

    public class counting : MonoBehaviour
    {
        public float timeLeft;

        public static float starttime = 2f;

        public SkillLocator bandit;

        public void Awake()
        {
            timeLeft = starttime;
            GetComponent<CharacterBody>().master.onBodyDeath.AddListener(Killed);
        }

        public void Killed()
        {
            GenericSkill[] skills = { bandit.primary, bandit.secondary, bandit.utility, bandit.special };
            foreach (var skill in skills)
            {
                while (skill.stock < skill.maxStock)
                {
                    skill.Reset();
                }
            }
            GetComponent<CharacterBody>().master.onBodyDeath.RemoveListener(Killed);
            Destroy(this);
        }

        public void Update()
        {
            if (Time.timeScale == 0f || Run.instance == null)
            {
                return;
            }
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                Destroy(this);
            }
        }
    }
}
