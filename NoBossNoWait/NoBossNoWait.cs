using BepInEx;
using R2API.Utils;
using RoR2;

namespace teleporter
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Hj.HjUpdaterAPI.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class NoBossNoWait : BaseUnityPlugin
    {
        internal const string modname = nameof(NoBossNoWait);
        internal const string version = "1.1.1";
        public void Awake()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Hj.HjUpdaterAPI.GUID))
            {
                Updater();
            }
            RoR2.BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
        }

        private static void Updater()
        {
            Hj.HjUpdaterAPI.Register(modname);
        }

        private void BossGroup_onBossGroupDefeatedServer(RoR2.BossGroup obj)
        {
            if (!TeleporterInteraction.instance) return;
            var bossGroup = TeleporterInteraction.instance.GetFieldValue<BossGroup>("bossGroup");
            if (!bossGroup) return;
            if (bossGroup != obj) return;

            Run.instance.fixedTime += TeleporterInteraction.instance.remainingChargeTimer;
            TeleporterInteraction.instance.remainingChargeTimer = 0f;
        }
    }
}
