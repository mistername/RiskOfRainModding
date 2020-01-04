using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SettingsPlus
{
    public static class GenerateMenu
    {
        internal static void GenerateNewHeader(On.RoR2.UI.HeaderNavigationController.orig_RebuildHeaders orig, HeaderNavigationController self)
        {
            //should prevent duplicates
            if (!self.headers.Any(p => p.headerName == "Button, Custom"))
            {
                //only in the options section
                if (self.headers.Any(p => p.headerName == "Gameplay"))
                {
                    MakeModCategory(self);
                }
            }

            orig(self);
        }

        //Fuction that makes the entire mod option section
        private static void MakeModCategory(HeaderNavigationController self)
        {
            GameObject optionSection = makeHeader(self);

            RemoveOldOptions(optionSection);

            var optionPart = getLayout(optionSection);

            GetSettingInputs(self, out GameObject boolean, out GameObject multipleChoice);

            setBoolChoices(optionPart, boolean);
            //Getmultichoice(self);
        }

        private static void RemoveOldOptions(GameObject optionSection)
        {
            var layout = getLayout(optionSection);

            for (int i = 0; i < layout.transform.childCount; i++)
            {
#if DEBUG
                Debug.Log("deleting: " + layout.transform.GetChild(i).name);
#endif
                UnityEngine.Object.Destroy(layout.transform.GetChild(i).gameObject);
            }
        }

        private static void GetSettingInputs(HeaderNavigationController self, out GameObject booleanOption, out GameObject multipleChoiceOption)
        {
            multipleChoiceOption = GetMultiChoiceOption(self);

            booleanOption = GetBooleanOption(self);

        }

        private static GameObject GetBooleanOption(HeaderNavigationController self)
        {
            GameObject booleanOption;
            var gameplayRoot = self.headers.First(p => p.headerName == "Gameplay").headerRoot;

            var gameplayLayout = getLayout(gameplayRoot);

            booleanOption = null;

            foreach (Transform child in gameplayLayout.transform)
            {
                if (child.gameObject.name == "Option, HUD")
                {
                    booleanOption = UnityEngine.Object.Instantiate(child.gameObject);
                    booleanOption.SetActive(false);
                    break;
                }
            }

            if (booleanOption == null)
            {
                Debug.LogError("couldn't find bool template");
            }

            return booleanOption;
        }

        private static GameObject GetMultiChoiceOption(HeaderNavigationController self)
        {
            GameObject multipleChoiceOption;
            var graphicRoot = self.headers.First(p => p.headerName == "Video").headerRoot;
            var graphiclayout = getLayout(graphicRoot);
            multipleChoiceOption = null;
            foreach (Transform child in graphiclayout.transform)
            {
                if (child.gameObject.name == "Option, Vsync")
                {
                    multipleChoiceOption = UnityEngine.Object.Instantiate(child.gameObject);
                    multipleChoiceOption.SetActive(false);
                    break;
                }
            }

            if (multipleChoiceOption == null)
            {
                Debug.LogError("couldn't find multipleChoice template");
            }

            return multipleChoiceOption;
        }

        private static GameObject makeHeader(HeaderNavigationController self)
        {
            CreateModButton(self, out MPButton button, out TMPro.TextMeshProUGUI text, out GameObject root);
            button.name = "Button, Custom";
            RemoveOldStuff(button);
            root.name = "SettingsSubPanel, Mods";

            var Header = new HeaderNavigationController.Header
            {
                headerButton = button,
                headerName = "Button, Custom",
                headerRoot = root,
                tmpHeaderText = text
            };

            var list = self.headers.ToList();
            list.Add(Header);
            self.headers = list.ToArray();
            return root;
        }

        private static void Getmultichoice(GameObject root, GameObject original)
        {
            foreach (var setting in Settings.listSettings.OfType<Settings.CustomMultichoiceSetting>())
            {
                GameObject stringconfig = Object.Instantiate(original, root.transform);
                stringconfig.name = "Option, " + setting.settingname;

                //stringconfig.AddComponent<catagorymod>();

                var textobject = stringconfig.gameObject.transform.Find("Text, Name").gameObject;
                var texttokenobject = textobject.GetComponent<LanguageTextMeshController>();
                texttokenobject.token = setting.settingtext;

                var choiceobject = stringconfig.gameObject.transform.Find("CarouselRect").gameObject;
                var choices = choiceobject.GetComponent<CarouselController>();
                choices.settingName = setting.conVar.name;
                choices.optionalText.text = setting.optionalText;
                choices.settingSource = BaseSettingsControl.SettingSource.ConVar;

                List<CarouselController.Choice> userchoises = new List<CarouselController.Choice>();
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

        private static void setBoolChoices(GameObject root, GameObject original)
        {
            original.name = "Option, original";
            foreach (var setting in Settings.listSettings.OfType<Settings.CustomBoolSetting>())
            {
                var i = 0;
                Debug.Log(i++);
                GameObject boolconfig = UnityEngine.Object.Instantiate(original, root.transform);
                Debug.Log(i++);
                boolconfig.SetActive(true);
                Debug.Log(i++);
                boolconfig.name = "Option, " + setting.settingname;
                Debug.Log(i++);
                var texttokenobject = boolconfig.GetComponentInChildren<LanguageTextMeshController>(true);
                Debug.Log(i++);
                texttokenobject.token = setting.settingtext;
                Debug.Log(i++);
                var choices = boolconfig.GetComponentInChildren<CarouselController>(true);
                Debug.Log(i++);
                choices.settingName = setting.settingname;
                Debug.Log(i++);
                if (choices.optionalText)
                {
                    choices.optionalText.text = setting.optionalText;
                }
                Debug.Log(i++);
                choices.settingSource = BaseSettingsControl.SettingSource.ConVar;
                Debug.Log(i++);
                choices.choices[0].convarValue = "0";
                choices.choices[1].convarValue = "1";
                Debug.Log(i++);
            }
            Object.Destroy(original);
        }

        private static GameObject getLayout(GameObject root)
        {
            var schrollview = root.transform.Find("Scroll View").gameObject;
            var Viewport = schrollview.transform.Find("Viewport").gameObject;
            var layout = Viewport.transform.Find("VerticalLayout").gameObject;
            return layout;
        }

        private static void RemoveOldStuff(MPButton button)
        {
            var gameplay = button.transform.Find("Text, Gameplay");
            if (gameplay)
            {
                Object.Destroy(gameplay.gameObject);
            }
        }

        private static void CreateModButton(HeaderNavigationController self, out MPButton button, out TMPro.TextMeshProUGUI text, out GameObject root)
        {
            button = Object.Instantiate(self.headers[0].headerButton, self.headers[0].headerButton.transform.parent);      //copy button

            text = Object.Instantiate(self.headers[0].tmpHeaderText, button.transform);                                    //copy textmesh field
            text.name = "Text, Custom";                                                                             //button internal name
            text.GetComponent<LanguageTextMeshController>().token = "Mods";                                         //button text

            root = Object.Instantiate(self.headers[0].headerRoot, self.headers[0].headerRoot.transform.parent);            //copy headerRoot
        }
    }
}
