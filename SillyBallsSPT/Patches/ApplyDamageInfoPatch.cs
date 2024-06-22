using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using UnityEngine;

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
            /*if (damageInfo.DamageType != EDamageType.Bullet && damageInfo.DamageType != EDamageType.Sniper && damageInfo.DamageType != EDamageType.Explosion
                && damageInfo.DamageType != EDamageType.GrenadeFragment && damageInfo.DamageType != EDamageType.Landmine &&
                damageInfo.DamageType != EDamageType.Melee)
                return; //maybe implement bleeds*/
            if (damageInfo.DamageType == EDamageType.Explosion || damageInfo.DamageType == EDamageType.Landmine
                || damageInfo.BlockedBy != "" || damageInfo.DeflectedBy != "")
                return;
            GameObject bloodParticles;
            bool LongerParticles = Plugin.LongerParticles.Value;
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee && damageInfo.DamageType != EDamageType.GrenadeFragment)
                bloodParticles = Object.Instantiate(LongerParticles ? Plugin.bloodParticlesHead : Plugin.bloodParticlesHeadShort, damageInfo.HitPoint, Quaternion.identity);
            else
                bloodParticles = Object.Instantiate(LongerParticles ? Plugin.bloodParticlesBody : Plugin.bloodParticlesBodyShort, damageInfo.HitPoint, Quaternion.identity);
            ParticleSystem particleSystem = bloodParticles.GetComponent<ParticleSystem>();
            if (particleSystem != null)
                particleSystem.Emit(1);
        }
    }
}
