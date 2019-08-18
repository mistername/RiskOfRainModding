using BepInEx;
using BepInEx.Configuration;
using RoR2;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace FirstPersonView
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Main : BaseUnityPlugin
    {
        internal const string modname = "FirstPersonView";
        internal const string version = "0.2.3";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        //config values;
        internal static ConfigWrapper<int> Height;
        internal static ConfigWrapper<int> forwards;
        internal static ConfigWrapper<bool> FirstPerson;
        internal static ConfigWrapper<bool> Invisible;

        public void Awake()
        {
            On.RoR2.CameraRigController.SetCameraState += Camera.CameraRigController_SetCameraState;

            //for invisible character in first person
            On.RoR2.Run.Update += Run_Update;


            Height = file.Wrap(new ConfigDefinition("Camera", "Height"), 0);
            forwards = file.Wrap(new ConfigDefinition("Camera", "forwards"), 0);
            FirstPerson = file.Wrap(new ConfigDefinition("Camera", "FirstPerson"), true);
            Invisible = file.Wrap(new ConfigDefinition("Camera", "Invisible character", "Makes character invisible when first person, should make first person better"), true);


        }

        private void Run_Update(On.RoR2.Run.orig_Update orig, Run self)
        {
            var Models = PlayerCharacterMasterController.instances[0]?.master?.GetBody()?.modelLocator?.modelTransform?.gameObject.GetComponentsInChildren<Renderer>(true);
            if (Models != null)
            {
                if (FirstPerson.Value && Invisible.Value)
                {
                    foreach (var CharacterModel in Models)
                    {
                        if (CharacterModel.gameObject.activeSelf)
                        {
                            CharacterModel.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (var CharacterModel in Models)
                    {
                        if (!CharacterModel.gameObject.activeSelf)
                        {
                            CharacterModel.gameObject.SetActive(true);
                        }
                    }
                }
            }
            orig(self);
        }


        public void OnDestroy()
        {
            On.RoR2.CameraRigController.SetCameraState -= Camera.CameraRigController_SetCameraState;
        }

        public void Update()
        {

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.KeypadPlus))
            {
                Height.Value++;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.KeypadMinus))
            {
                Height.Value--;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad8))
            {
                forwards.Value++;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad2))
            {
                forwards.Value--;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad5))
            {
                FirstPerson.Value = !FirstPerson.Value;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.KeypadDivide))
            {
                Height.Value = 0;
                forwards.Value = 0;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.KeypadMultiply))
            {
                Invisible.Value = !Invisible.Value;
            }
        }
    }
}
