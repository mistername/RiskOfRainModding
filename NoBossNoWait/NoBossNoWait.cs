using BepInEx;

namespace teleporter
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername.NoBossNoWait", "NoBossNoWait", "1.0.1")]
    public class Teleporter : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.BossGroup.Awake += BossGroup_Awake;
            On.RoR2.BossGroup.OnDestroy += BossGroup_OnDestroy;
        }

        private void BossGroup_Awake(On.RoR2.BossGroup.orig_Awake orig, RoR2.BossGroup self)
        {
            On.RoR2.TeleporterInteraction.FixedUpdate += TeleporterInteraction_FixedUpdate;
            orig(self);
        }

        private void BossGroup_OnDestroy(On.RoR2.BossGroup.orig_OnDestroy orig, RoR2.BossGroup self)
        {
            On.RoR2.TeleporterInteraction.FixedUpdate -= TeleporterInteraction_FixedUpdate;
            orig(self);
        }

        private void TeleporterInteraction_FixedUpdate(On.RoR2.TeleporterInteraction.orig_FixedUpdate orig, RoR2.TeleporterInteraction self)
        {
            if (self.isCharging)
            {
                    if (RoR2.BossGroup.GetTotalBossCount() == 0)
                    {
                        RoR2.TeleporterInteraction.instance.remainingChargeTimer = 0f;
                    }
            }
            orig(self);
        }
    }
}
