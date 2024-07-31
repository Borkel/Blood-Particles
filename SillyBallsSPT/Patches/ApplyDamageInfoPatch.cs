using SPT.Reflection.Patching;
using EFT;
using System.Reflection;
using UnityEngine;
using System;

namespace BloodParticles.Patches
{
    public class ApplyDamageInfoPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.ApplyDamageInfo));
        }

        [PatchPrefix]
        public static void Postfix(DamageInfo damageInfo, EBodyPart bodyPartType, EBodyPartColliderType colliderType, float absorbed)
        {
            if (!Plugin.Enabled.Value)
            {
                return;
            }

            EDamageType dmgType = damageInfo.DamageType;
            bool penetrated = (String.IsNullOrEmpty(damageInfo.BlockedBy) || String.IsNullOrEmpty(damageInfo.DeflectedBy));
            bool explosionDamage = (dmgType == EDamageType.Explosion || dmgType == EDamageType.Landmine);
            Logger.LogInfo(penetrated);

            if (explosionDamage || !penetrated)
            {
                return;
            }

            // entry wound
            Vector3 entryDirection = -damageInfo.Direction.normalized;
            Quaternion entryRotation = Quaternion.LookRotation(entryDirection);
            ParticleHelper.CreateBlood(damageInfo, damageInfo.HitPoint, entryRotation, Plugin.EntryBloodVelocityMin.Value, Plugin.EntryBloodVelocityMax.Value,
                Plugin.EntryBloodAngle.Value, Plugin.EntryBloodAmountMultiplier.Value, false);
            if (damageInfo.Damage < 30 || damageInfo.DamageType == EDamageType.Melee || dmgType == EDamageType.GrenadeFragment) //only do exit wounds if enough damage is applied or not melee (buckshot should not go through)
                return;
            // exit wound
            Vector3 exitDirection = damageInfo.Direction.normalized;
            Quaternion exitRotation = Quaternion.LookRotation(exitDirection);
            ParticleHelper.CreateBlood(damageInfo, damageInfo.HitPoint, exitRotation, Plugin.ExitBloodVelocityMin.Value, Plugin.ExitBloodVelocityMax.Value,
                Plugin.ExitBloodAngle.Value, Plugin.ExitBloodAmountMultiplier.Value, bodyPartType == 0);
        }
    }
}
