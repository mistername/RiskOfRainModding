using BepInEx;
using BepInEx.Configuration;

namespace FirstPersonView
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Main : BaseUnityPlugin
    {
        internal const string modname = "FirstPersonView";
        internal const string version = "0.2.0";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        //config values;
        internal static ConfigWrapper<int> Height;
        internal static ConfigWrapper<int> right;
        internal static ConfigWrapper<int> forwards;
        internal static ConfigWrapper<bool> Off;

        public void Awake()
        {
            On.RoR2.CameraRigController.SetCameraState += Camera.CameraRigController_SetCameraState;

            Height = file.Wrap(new ConfigDefinition("Camera", "Height"), 0);
            right = file.Wrap(new ConfigDefinition("Camera", "right"), 0);
            forwards = file.Wrap(new ConfigDefinition("Camera", "forwards"), 0);
            Off = file.Wrap(new ConfigDefinition("Camera", "Height"), false);
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

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad6))
            {
                right.Value++;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad4))
            {
                right.Value--;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad8))
            {
                forwards.Value++;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad2))
            {
                forwards.Value--;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad3))
            {
                Off.Value = true;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Keypad9))
            {
                Off.Value = false;
            }
        }
    }
}
