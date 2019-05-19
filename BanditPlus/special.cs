using EntityStates.Toolbot;
using RoR2;

namespace EntityStates.Bandit
{
    public class Special : AimGrenade
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(AimStunDrone.enterSoundString, gameObject);
            PlayAnimation("Gesture, Additive", "PrepBomb", "PrepBomb.playbackRate", minimumDuration);
            Chat.AddMessage(AimStunDrone.enterSoundString);
            Chat.AddMessage(AimStunDrone.exitSoundString);
        }

        public override void OnExit()
        {
            base.OnExit();
            outer.SetNextState(new RecoverAimStunDrone());
            Util.PlaySound(AimStunDrone.exitSoundString, gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static string enterSoundString;

        public static string exitSoundString;
    }
}
