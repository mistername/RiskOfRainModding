using BepInEx;
using BepInEx.Configuration;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SettingsPlus.Settings;

namespace SettingsPlus
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public partial class Main : BaseUnityPlugin
    {
        const string version = "1.0.0";
        const string modname = "SettingsPlus";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            On.RoR2.UI.HeaderNavigationController.RebuildHeaders += GenerateNewHeader;
            Convars.Init.Hooks();
        }

        private void GenerateNewHeader(On.RoR2.UI.HeaderNavigationController.orig_RebuildHeaders orig, HeaderNavigationController self)
        {
            orig(self);

            if (!self.headers.Any(p => p.headerName == "Button, custom"))
            {
                if (self.headers.Any(p => p.headerName == "Gameplay"))
                {
                    MakeHeader(self);
                }
            }
        }

        private void MakeHeader(HeaderNavigationController self)
        {
#if DEBUG
            Debug.Log("instance");
#endif
            GameObject root = makeHeader(self);

            setBoolChoices(root);
            Getmultichoice(self);
        }

        private static GameObject makeHeader(HeaderNavigationController self)
        {
            CreateModButton(self, out MPButton button, out TMPro.TextMeshProUGUI text, out GameObject root);
            button.name = "Button, custom";
            RemoveTmpButton(button);
            root.name = "SettingsSubPanel, Mods";

            var Header = new HeaderNavigationController.Header
            {
                headerButton = button,
                headerName = "Button, custom",
                headerRoot = root,
                tmpHeaderText = text
            };

            var list = self.headers.ToList();
            list.Add(Header);
            self.headers = list.ToArray();
            return root;
        }

        private void Getmultichoice(HeaderNavigationController self)
        {
            var root = self.headers[4].headerRoot;
            var graphiclayout = getLayout(root);
            GameObject original = null;
            foreach (Transform child in graphiclayout.transform)
            {
                if (child.gameObject.name != "Option, Vsync")
                {
                    //Destroy(child.gameObject);ff
                }
                else
                {
                    original = child.gameObject;
                }
            }

            foreach (var setting in listSettings.Where(p => p.type == typeof(CustomMultichoiceSetting)).Cast<CustomMultichoiceSetting>())
            {
                GameObject stringconfig = Instantiate(original, layout.transform);
                stringconfig.name = "Option, " + setting.settingname;

                stringconfig.AddComponent<catagorymod>();

                var textobject = stringconfig.gameObject.transform.Find("Text, Name").gameObject;
                var texttokenobject = textobject.GetComponent<LanguageTextMeshController>();
                texttokenobject.token = setting.settingtext;

                var choiceobject = stringconfig.gameObject.transform.Find("CarouselRect").gameObject;
                var choices = choiceobject.GetComponent<CarouselController>();
                choices.settingName = setting.conVar.name;
                choices.optionalText.text = "does the host need it?";
                choices.settingSource = BaseSettingsControl.SettingSource.ConVar;

                List <CarouselController.Choice> userchoises = new List<CarouselController.Choice>();
                foreach (var choice in setting.Choices)
                {
                    var userchoice = new CarouselController.Choice
                    {
                        convarValue = choice.Value,
                        customSprite = choices.choices[0].customSprite,
                        suboptionDisplayToken = choice.DisplayText
                    };
                    userchoises.Add(userchoice);
                }
                choices.choices = userchoises.ToArray();
            }
        }

        GameObject layout;

        private void setBoolChoices(GameObject root)
        {
            layout = getLayout(root);
            GameObject original = null;
            foreach (Transform child in layout.transform)
            {
                if (child.gameObject.name != "Option, HUD")
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    original = child.gameObject;
                }
            }

            original.name = "Option, original";

            foreach (var setting in listSettings.Where(p => p.type == typeof(CustomBoolSetting)).Cast<CustomBoolSetting>())
            {
                GameObject boolconfig = Instantiate(original, layout.transform);
                boolconfig.name = "Option, " + setting.settingname;

                var textobject = boolconfig.gameObject.transform.Find("Text, Name").gameObject;
                var texttokenobject = textobject.GetComponent<LanguageTextMeshController>();
                texttokenobject.token = setting.settingtext;

                var choiceobject = boolconfig.gameObject.transform.Find("CarouselRect").gameObject;
                var choices = choiceobject.GetComponent<CarouselController>();
                choices.settingName = setting.conVar.name;
                choices.settingSource = BaseSettingsControl.SettingSource.ConVar;

                choices.choices[0].convarValue = "0";
                choices.choices[1].convarValue = "1";
            }

            Destroy(original);
        }

        private static GameObject getLayout(GameObject root)
        {
            var schrollview = root.transform.Find("Scroll View").gameObject;
            var Viewport = schrollview.transform.Find("Viewport").gameObject;
            var layout = Viewport.transform.Find("VerticalLayout").gameObject;
            return layout;
        }

        private static void RemoveTmpButton(MPButton button)
        {
            var gameplay = button.transform.Find("Text, Gameplay");
            if (gameplay)
            {
                Destroy(gameplay.gameObject);
            }
        }

        private static void CreateModButton(HeaderNavigationController self, out MPButton button, out TMPro.TextMeshProUGUI text, out GameObject root)
        {
            button = Instantiate(self.headers[0].headerButton, self.headers[0].headerButton.transform.parent);      //copy button

            text = Instantiate(self.headers[0].tmpHeaderText, button.transform);                                    //copy textmesh field
            text.name = "Text, Custom";                                                                             //button internal name
            text.GetComponent<LanguageTextMeshController>().token = "Mods";                                         //button text

            root = Instantiate(self.headers[0].headerRoot, self.headers[0].headerRoot.transform.parent);            //copy headerRoot
        }
    }

    internal class catagorymod : MonoBehaviour
    {
        public void Awake()
        {
            Main.gameobjects.Add(gameObject);
        }

        public void OnDestroy()
        {
            Main.gameobjects.Remove(gameObject);
        }

        public void Update()
        {
            if(listSettings.OfType<CustomBoolSetting>().First().conVar.GetString() == "0")
            {
                gameObject.SetActive(false);
            }
        }
    }
}
