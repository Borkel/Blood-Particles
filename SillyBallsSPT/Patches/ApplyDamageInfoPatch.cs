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
            /*if (bodyPartType == 0)
                Logger.LogMessage($"DAMAGE: {damageInfo.Damage}");*/
            if (!Plugin.Enabled.Value)
                return;
            if (damageInfo.DamageType == EDamageType.Explosion || damageInfo.DamageType == EDamageType.Landmine
                || damageInfo.BlockedBy != "" || damageInfo.DeflectedBy != "")
                return;
            bool LongerParticles = Plugin.LongerParticles.Value;
            Vector3 direction = damageInfo.Direction.normalized;
            Quaternion rotation1 = Quaternion.LookRotation(direction);
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee && damageInfo.DamageType != EDamageType.GrenadeFragment)
                Object.Instantiate(LongerParticles ? Plugin.bloodParticlesHead : Plugin.bloodParticlesHeadShort, damageInfo.HitPoint, rotation1);
            //GameObject bloodParticlesBody;// = Object.Instantiate(LongerParticles ? Plugin.bloodParticlesBody : Plugin.bloodParticlesBodyShort, damageInfo.HitPoint, rotation1);
            if(damageInfo.Damage>=30)
                Object.Instantiate(LongerParticles ? Plugin.bloodParticlesBody : Plugin.bloodParticlesBodyShort, damageInfo.HitPoint, rotation1);
            else //prevents buckshot from spawning too many particles
                Object.Instantiate(LongerParticles ? Plugin.bloodParticlesBodySmall : Plugin.bloodParticlesBodySmallShort, damageInfo.HitPoint, rotation1);
            /*ParticleSystem particleSystemHead = bloodParticlesHead.GetComponent<ParticleSystem>();
            ParticleSystem particleSystemBody = bloodParticlesBody.GetComponent<ParticleSystem>();
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee && damageInfo.DamageType != EDamageType.GrenadeFragment)
                particleSystemHead.Emit(1);
            particleSystemBody.Emit(1);*/
        }
    }
}
