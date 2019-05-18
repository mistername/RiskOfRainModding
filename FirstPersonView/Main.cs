using BepInEx;

namespace FirstPersonView
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Main : BaseUnityPlugin
    {
        internal const string modname = "FirstPersonView";
        internal const string version = "0.1.0";

        public void Awake()
        {
            On.RoR2.CameraRigController.SetCameraState += Camera.CameraRigController_SetCameraState;
        }

        public void OnDestroy()
        {
            On.RoR2.CameraRigController.SetCameraState -= Camera.CameraRigController_SetCameraState;
        }
    }
}
