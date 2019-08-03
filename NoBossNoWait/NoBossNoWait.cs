using BepInEx;

namespace teleporter
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername.NoBossNoWait", "NoBossNoWait", "1.0.2")]
    public class Teleporter : BaseUnityPlugin
    {
        public void Awake()
        {
            RoR2.BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
        }

        private void BossGroup_onBossGroupDefeatedServer(RoR2.BossGroup obj)
        {
            RoR2.TeleporterInteraction.instance.remainingChargeTimer = 0f;
        }
    }
}
