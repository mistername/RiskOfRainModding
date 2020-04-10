using BepInEx;
using R2API;
using RoR2;
using System;

namespace Vibrating
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Vibrating : BaseUnityPlugin
    {
        const string modname = nameof(Vibrating);
        const string version = "1.0.0";

        Controller controller;

        public void Awake()
        {
            controller = new Controller();
            RoR2.GlobalEventManager.onClientDamageNotified += GlobalEventManager_onClientDamageNotified;
        }

        public void Update()
        {
            controller.Update(UnityEngine.Time.deltaTime);
        }

        private void GlobalEventManager_onClientDamageNotified(RoR2.DamageDealtMessage obj)
        {
            if (!obj.victim)
            {
                return;
            }
            var playermaster = PlayerCharacterMasterController.instances[0].master;
            if (!playermaster)
            {
                return;
            }
            var playerbody = playermaster.GetBody();
            if (!playerbody)
            {
                return;
            }

            if (obj.victim != playerbody.gameObject)
            {
                return;
            }

            var healthcomp = obj.victim.GetComponent<RoR2.HealthComponent>();

            if(controller != null)
            {
                controller.Vibrating(1f - healthcomp.combinedHealthFraction, 0.5f);
            }
        }
    }
}
