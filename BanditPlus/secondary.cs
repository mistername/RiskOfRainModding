using BanditPlus;
using RoR2;
using System.Linq;
using UnityEngine;

namespace EntityStates.Bandit
{
    public class Secondary : BaseState
    {
        public SkillLocator skill = null;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            AddRecoil(-3f * recoilAmplitude, -4f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f, false);
            string muzzleName = "MuzzlePistol";
            Util.PlaySound(attackSoundString, gameObject);
            PlayAnimation("Gesture, Additive", "FireRevolver");
            PlayAnimation("Gesture, Override", "FireRevolver");
            if (effectPrefab)
            {
                EffectManager.instance.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, true);
            }
            if (isAuthority)
            {
                skill = gameObject?.GetComponentInChildren<SkillLocator>();
                BulletAttack bulletAttack = new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = minSpread,
                    maxSpread = maxSpread,
                    bulletCount = (uint)((bulletCount > 0) ? bulletCount : 0),
                    damage = damageCoefficient * damageStat,
                    force = force,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    HitEffectNormal = false,
                    radius = 0.5f,
                    smartCollision = true
                };
                bulletAttack.damageType |= DamageType.ResetCooldownsOnKill;
                if (Timer.Timer.flag)
                {
                    bulletAttack.hitCallback = new BulletAttack.HitCallback(Hit) + bulletAttack.hitCallback;
                }
                bulletAttack.Fire();
            }
        }

        private bool Hit(ref BulletAttack.BulletHit hit)
        {
            if (hit.entityObject != null)
            {
                if (Timer.Timer.buffplus)
                {
                    var timer = hit.entityObject.GetComponent<Timer.Counting>();
                    if (timer != null)
                    {
                        timer.timeLeft = Timer.Timer.timeStart;
                    }
                    else
                    {
                        timer = hit.entityObject.AddComponent<Timer.Counting>();
                        timer.bandit = skill;
                    }
                }
                else
                {
                    var timer = hit.entityObject.GetComponent<counting>();
                    if (timer != null)
                    {
                        timer.timeLeft = Timer.Timer.timeStart;
                    }
                    else
                    {
                        timer = hit.entityObject.AddComponent<counting>();
                        timer.bandit = skill;
                    }
                }
            }
            return false;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashBanditPistol");

        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/HitsparkBanditPistol");

        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerBanditPistol");

        public static float damageCoefficient = 6f;

        public static float force = 500f;

        public static float minSpread = 0f;

        public static float maxSpread = 0f;

        public static int bulletCount = 1;

        public static float baseDuration = 0.5f;

        public const string attackSoundString = "Play_bandit_M2_shot";

        public static float recoilAmplitude = 1f;

        public int bulletCountCurrent = 1;

        private float duration;
    }

    public class PrepSecondary : BaseState
    {
        public static float baseDuration = 0.5f;

        private static readonly GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshairrevolver");

        public const string prepSoundString = "Play_bandit_M2_load";

        private readonly GameObject chargeEffect;

        private float duration;

        private GameObject defaultCrosshairPrefab;

        public static void SetSecondary(GameObject gameObject)
        {
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            GenericSkill skillslot = component.secondary;
            SkillManagement.SetSkill(ref skillslot, typeof(PrepSecondary));
            component.secondary = skillslot;
            config();
        }

        public static void config()
        {
            parse("Aimtime", baseDuration, out baseDuration);

            parse("DamageCoefficient", Secondary.damageCoefficient, out Secondary.damageCoefficient);

            parse("Knockback", Secondary.force, out Secondary.force);

            parse("MinimumSpread", Secondary.minSpread, out Secondary.minSpread);

            parse("MaximumSpread", Secondary.maxSpread, out Secondary.maxSpread);

            parse("BaseTimeBetweenUsages", Secondary.baseDuration, out Secondary.baseDuration);

            parse("Recoil", Secondary.recoilAmplitude, out Secondary.recoilAmplitude);
        }

        public static bool parse(string field, float defaultvalue, out float value)
        {
            return SkillManagement.configfloat(nameof(Secondary), field, defaultvalue, out value);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", duration);
            PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", duration);
            Util.PlaySound(prepSoundString, gameObject);
            defaultCrosshairPrefab = characterBody.crosshairPrefab;
            characterBody.crosshairPrefab = specialCrosshairPrefab;
            if (characterBody)
            {
                characterBody.SetAimTimer(duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Secondary());
                return;
            }
        }

        public override void OnExit()
        {
            Destroy(chargeEffect);
            characterBody.crosshairPrefab = defaultCrosshairPrefab;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

