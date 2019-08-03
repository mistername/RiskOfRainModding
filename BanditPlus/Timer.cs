using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace EntityStates.Bandit.Timer
{
    public class Timer : MonoBehaviour
    {
        public static float fadeout = 1f;

        internal static ConfigFile file = BanditPlus.BanditMod.file;

        public static bool flag = true;

        public static bool buffplus;

        public static float timeStart = 2f;

        public static bool numbers = false;

        public static void Init()
        {
            PrepSecondary.parse("Duration_of_lightsout_timer", timeStart, out timeStart);
            numbers = file.Wrap(nameof(Secondary), "number_appearance_on_timer", null, false).Value;
            flag = file.Wrap(nameof(Secondary), "Lightsout_timer_enabled", null, true).Value;
            PrepSecondary.parse("start_of_fadeout", fadeout, out fadeout);

            if (flag)
            {
                var icon = new Texture2D(1, 1);
                var imagepath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                imagepath += @"\lightsout.png";
                if (File.Exists(imagepath))
                {
                    icon.LoadImage(File.ReadAllBytes(imagepath));
                }
                else
                {
                    Debug.Log("using default image");
                    icon.LoadImage(Convert.FromBase64String(@"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"));
                }

                var image = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0, 0));
                buffplus = BepInEx.Bootstrap.Chainloader.Plugins.Exists(p => MetadataHelper.GetMetadata(p).GUID == "com.mistername.BuffDisplayAPI");

                if (buffplus)
                {
                    BuffPlus(image);
                    Debug.Log("using BuffDisplayAPI");
                }
                else
                {
                    BanditPlus.oldbuff.Init(image);
                    Debug.Log("using old buff type");
                }
            }
        }

        private static void BuffPlus(Sprite image)
        {
            BuffDisplayAPI.Buff buff = new BuffDisplayAPI.Buff
            {
                sprite = image,
                type = typeof(Counting)
            };
            BuffDisplayAPI.CustomBuffs.Add(buff);
        }
    }

    internal class Counting : MonoBehaviour
    {
        public float timeLeft;

        public SkillLocator bandit = null;

        public void Awake()
        {
            timeLeft = Timer.timeStart;
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

        public void Alpha(out float alpha)
        {
            if (timeLeft < Timer.fadeout)
            {
                alpha = (float)Math.Sqrt((double)timeLeft / Timer.fadeout);
            }
            else
            {
                alpha = 1f;
            }
        }

        public void Text(out string text)
        {
            if (Timer.numbers)
            {
                text = timeLeft.ToString("0.0");
            }
            else
            {
                text = string.Empty;
            }
        }
    }
}