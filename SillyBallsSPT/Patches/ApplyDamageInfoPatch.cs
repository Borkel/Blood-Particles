using SPT.Reflection.Patching;
using EFT;
using System.Reflection;
using UnityEngine;
using System;

namespace BloodParticles.Patches
{
    public class ApplyDamageInfoPatch : ModulePatch
    {
        private static bool AttackPenetrated(DamageInfoStruct damageInfo)
        {
            return String.IsNullOrEmpty(damageInfo.BlockedBy) || String.IsNullOrEmpty(damageInfo.DeflectedBy);
        }

        private static bool IsExplosionDamage(EDamageType damageType)
        {
            return damageType == EDamageType.Explosion || damageType == EDamageType.Landmine;
        }

        private static bool ShouldNotDoExitWound(DamageInfoStruct damageInfo)
        {
            return damageInfo.Damage < 30 || damageInfo.DamageType == EDamageType.Melee || damageInfo.DamageType == EDamageType.GrenadeFragment;
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.ApplyDamageInfo));
        }

        [PatchPostfix]
        public static void Postfix(DamageInfoStruct damageInfo, EBodyPart bodyPartType, EBodyPartColliderType colliderType, float absorbed)
        {
            if (!Plugin.Enabled.Value)
            {
                return;
            }

            EDamageType dmgType = damageInfo.DamageType;
            float damage = damageInfo.Damage;
            bool explosionDamage = IsExplosionDamage(dmgType);
            bool penetrated = AttackPenetrated(damageInfo);

            if (explosionDamage || !penetrated)
            {
                return;
            }

            ParticleSystemShapeType shapeType = ParticleSystemShapeType.Cone;
            float bloodMult = 1f;

            if (bodyPartType == EBodyPart.Head && dmgType != EDamageType.GrenadeFragment)
            {
                bloodMult = Plugin.HeadshotMultiplier.Value;
            }
            else if (dmgType == EDamageType.GrenadeFragment)
            {
                bloodMult = 0.2f;
                shapeType = ParticleSystemShapeType.Sphere;
            }

            // entry wound
            Vector3 hitPos = damageInfo.HitPoint;
            Vector3 entryDirection = -damageInfo.Direction.normalized;

            ParticleHelper.Instance.CreateBlood(new ParticleInfo(
                damageInfo,
                hitPos,
                entryDirection,
                shapeType,
                Plugin.EntryBloodAngle.Value,
                Plugin.EntryBloodAmountMultiplier.Value * bloodMult,
                Plugin.EntryBloodVelocityMin.Value,
                Plugin.EntryBloodVelocityMax.Value
            ), true);

            // only do exit wounds if enough damage is applied or not melee (buckshot should not go through)
            if (ShouldNotDoExitWound(damageInfo))
            {
                return;
            }

            // exit wound
            Vector3 exitDirection = damageInfo.Direction.normalized;

            ParticleHelper.Instance.CreateBlood(new ParticleInfo(
                damageInfo,
                hitPos,
                exitDirection,
                shapeType,
                Plugin.ExitBloodAngle.Value,
                Plugin.ExitBloodAmountMultiplier.Value * bloodMult,
                Plugin.ExitBloodVelocityMin.Value,
                Plugin.ExitBloodVelocityMax.Value
            ), true);
        }
    }
}
