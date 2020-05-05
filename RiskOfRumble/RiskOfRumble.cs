using BepInEx;
using R2API;
using RoR2;
using System;

namespace Vibrating
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class RiskOfRumble : BaseUnityPlugin
    {
        static RiskOfRumble _instance;
        BepInEx.Configuration.ConfigEntry<bool> _rewarding;
        BepInEx.Configuration.ConfigEntry<bool> _punishing;
        const string modname = nameof(RiskOfRumble);
        const string version = "1.0.0";

        Controller _controller;

        RiskOfRumble()
        {
            _instance = this;
        }

        public void Awake()
        {
            _rewarding = this.Config.Bind("settings", "rewarding", false);
            _punishing = this.Config.Bind("settings", "punishing", true);
            _controller = new Controller();
            _controller.enumerator = _controller.Read();
            StartCoroutine(_controller.enumerator);
            RoR2.GlobalEventManager.onClientDamageNotified += GlobalEventManager_onClientDamageNotified;
        }

        public void Update()
        {
            _controller.Update(UnityEngine.Time.deltaTime);
        }

        private void GlobalEventManager_onClientDamageNotified(RoR2.DamageDealtMessage obj)
        {
            if (!obj.victim)
            {
                return;
            }

            if (_rewarding.Value)
            {
                Reward(obj);
            }

            if(_punishing.Value)
            {
                Punishing(obj);
            }
        }

        private void Punishing(DamageDealtMessage obj)
        {
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

            var victim = obj.victim;

            if (victim != playerbody.gameObject)
            {
                return;
            }

            var healthcomp = victim.GetComponent<RoR2.HealthComponent>();

            if (_controller != null)
            {
                _controller.Vibrating(2.0f * (obj.damage / healthcomp.fullCombinedHealth));
            }
        }

        private void Reward(DamageDealtMessage obj)
        {
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

            if (obj.attacker != playerbody.gameObject)
            {
                return;
            }

            var victim = obj.victim;
            if (!victim)
            {
                return;
            }

            var healthcomp = victim.GetComponent<RoR2.HealthComponent>();

            if (_controller != null)
            {
                _controller.Vibrating(0.5f * (obj.damage / healthcomp.fullCombinedHealth));
            }
        }
    }
}
