using BepInEx;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace FirstPersonView
{
    public class Camera : MonoBehaviour
    {
        public static bool shouldShake = false;

        public static int height = 0;

        //remade the whole function because the original function would otherwise throw an exception
        internal static void CameraRigController_SetCameraState(On.RoR2.CameraRigController.orig_SetCameraState orig, CameraRigController self, CameraState cameraState)
        {
            if (Run.instance)
            {
                if (self.cameraMode == CameraRigController.CameraMode.PlayerBasic)
                {
                    var characterposition = self?.localUserViewer?.cachedBody?.corePosition;
                    if (characterposition.HasValue)
                    {
                        cameraState.position = (Vector3)characterposition + Vector3.up * height;
                    }
                }
            }
            currentstate(cameraState, self);
            float d = (self.localUserViewer == null) ? 1f : self.localUserViewer.userProfile.screenShakeScale;
            Vector3 position = cameraState.position;
            if (shouldShake)
            {
                shake(self, cameraState);
            }
            Vector3 vector = self.rawScreenShakeDisplacement * d;
            Vector3 position2 = position + vector;
            if (vector != Vector3.zero)
            {
                Vector3 origin = position;
                Vector3 direction = vector;
                if (Physics.SphereCast(origin, self.sceneCam.nearClipPlane, direction, out RaycastHit raycastHit, vector.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    position2 = position + vector.normalized * raycastHit.distance;
                }
            }
            self.transform.SetPositionAndRotation(position2, cameraState.rotation);
            fov(self, cameraState);
            if (self.sceneCam)
            {
                self.sceneCam.fieldOfView = cameraState.fov;
            }
        }

        private static FieldInfo currentFov;
        private static void fov(CameraRigController self, CameraState cameraState)
        {
            if(currentFov == null)
            {
                currentFov = typeof(CameraRigController).GetField("currentFov", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            currentFov.SetValue(self, cameraState.fov);
        }
        
        private static void shake(CameraRigController self, CameraState cameraState)
        {
            Vector3 vector = ShakeEmitter.ComputeTotalShakeAtPoint(cameraState.position);
            setshake(self, vector);
        }

        private static FieldInfo rawScreenShakeDisplacement;
        private static void setshake(CameraRigController self, Vector3 vector)
        {
            if(rawScreenShakeDisplacement == null)
            {
                rawScreenShakeDisplacement = typeof(CameraRigController).GetField("rawScreenShakeDisplacement", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            rawScreenShakeDisplacement.SetValue(self, vector);
        }

        private static FieldInfo currentCameraState;
        private static void currentstate(CameraState cameraState, CameraRigController self)
        {
            if (currentCameraState == null)
            {
                currentCameraState = typeof(CameraRigController).GetField("currentCameraState", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            currentCameraState.SetValue(self, cameraState);
        }
    }
}
