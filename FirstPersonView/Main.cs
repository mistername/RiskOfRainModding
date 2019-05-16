using BepInEx;

namespace Camera
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public partial class Main : BaseUnityPlugin
    {
        internal const string modname = "FirstPersonView";
        internal const string version = "0.1.0";

        public void Awake()
        {
            On.RoR2.CameraRigController.SetCameraState += CameraRigController_SetCameraState;
        }

    }
}
