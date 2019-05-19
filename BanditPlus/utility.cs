using BanditPlus;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bandit
{
    public class Utility : BaseState
    {
        public static void SetUtility(GameObject gameObject)
        {
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            GenericSkill skillslot = component.utility;
            SkillManagement.SetSkill(ref skillslot, typeof(CastSmokescreen));
            component.utility = skillslot;
            config();
        }

        public static void config()
        {
            parse("JumpTime", baseDuration, out baseDuration);

            parse("StealthDuration", stealthDuration, out stealthDuration);

            parse("DamageCoefficient", damageCoefficient, out damageCoefficient);

            parse("Radius", radius, out radius);

            parse("Knockback", forceMagnitude, out forceMagnitude);
        }

        public static bool parse(string field, float defaultvalue, out float value)
        {
            return SkillManagement.configfloat(nameof(CastSmokescreen), field, defaultvalue, out value);
        }

        private void CastSmoke()
        {
            if (!hasCastSmoke)
            {
                Util.PlaySound(startCloakSoundString, gameObject);
            }
            else
            {
                Util.PlaySound(stopCloakSoundString, gameObject);
            }
            EffectManager.instance.SpawnEffect(smokescreenEffectPrefab, new EffectData
            {
                origin = transform.position
            }, true);
            var animator = GetModelAnimator();
            var layerIndex = animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                animator.SetLayerWeight(layerIndex, 2f);
                animator.PlayInFixedTime("LightImpact", layerIndex, 5f);
            }
            if (NetworkServer.active)
            {
                new BlastAttack
                {
                    attacker = gameObject,
                    inflictor = gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(gameObject),
                    baseDamage = damageStat * damageCoefficient,
                    baseForce = forceMagnitude,
                    position = transform.position,
                    radius = radius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.Stun1s
                }.Fire();
            }
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            totalDuration = stealthDuration + duration;
            PlayCrossfade("Gesture, Smokescreen", "CastSmokescreen", "CastSmokescreen.playbackRate", duration, 0.2f);
            Util.PlaySound(jumpSoundString, gameObject);
            EffectManager.instance.SpawnEffect(initialEffectPrefab, new EffectData
            {
                origin = transform.position
            }, true);
            if (characterBody && NetworkServer.active)
            {
                characterBody.AddBuff(BuffIndex.CloakSpeed);
            }
        }
        
        public override void OnExit()
        {
            if (characterBody && NetworkServer.active)
            {
                if (characterBody.HasBuff(BuffIndex.Cloak))
                {
                    characterBody.RemoveBuff(BuffIndex.Cloak);
                }
                if (characterBody.HasBuff(BuffIndex.CloakSpeed))
                {
                    characterBody.RemoveBuff(BuffIndex.CloakSpeed);
                }
            }
            if (!outer.destroying)
            {
                CastSmoke();
            }
            base.OnExit();
        }
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && !hasCastSmoke)
            {
                CastSmoke();
                if (characterBody && NetworkServer.active)
                {
                    characterBody.AddBuff(BuffIndex.Cloak);
                }
                hasCastSmoke = true;
            }
            if (fixedAge >= totalDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!hasCastSmoke)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }
        
        public static float baseDuration = 0.5f;
        
        public static float stealthDuration = 3.5f;
        
        public const string jumpSoundString = "Play_bandit_shift_jump";
        
        public const string startCloakSoundString = "Play_bandit_shift_land";
        
        public const string stopCloakSoundString = "Play_bandit_shift_end";
        
        public static GameObject initialEffectPrefab = Resources.Load<GameObject>("Prefabs/effects/impacteffects/characterlandimpact");
        
        public static GameObject smokescreenEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/SmokescreenEffect");
        
        public static float damageCoefficient = 1.3f;
        
        public static float radius = 10f;

        public static float forceMagnitude = 10f;
        
        private float duration;
        
        private float totalDuration;
        
        private bool hasCastSmoke;
    }
}
