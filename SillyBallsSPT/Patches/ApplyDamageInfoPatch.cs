using SPT.Reflection.Patching;
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

        [PatchPostfix]
        public static void Postfix(DamageInfo damageInfo, EBodyPart bodyPartType, EBodyPartColliderType colliderType, float absorbed)
        {
            if (!Plugin.Enabled.Value)
                return;
            if (damageInfo.DamageType == EDamageType.Explosion || damageInfo.DamageType == EDamageType.Landmine
                || damageInfo.BlockedBy != "" || damageInfo.DeflectedBy != "")
                return;
            Vector3 direction = damageInfo.Direction.normalized;
            Vector3 directionInv = direction * -1;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Quaternion rotationInv = Quaternion.LookRotation(directionInv);
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee && damageInfo.DamageType != EDamageType.GrenadeFragment) //if headshot
            {
                if (damageInfo.Damage < 30)
                {
                    ParticleHelper.CreateParticle(Plugin.blood13, damageInfo.HitPoint, rotationInv);
                    ParticleHelper.CreateParticle(Plugin.blood40, damageInfo.HitPoint, rotation);
                    return;
                } //buckshot
                else
                {
                    // AHHHHHHHHH SPAM A BILLION TRILLION PARTICLES AND LAG THE GAME!!!!!!!!!    - pein, 29/07/2024
                    ParticleHelper.CreateParticle(Plugin.blood20, damageInfo.HitPoint, rotationInv);
                    ParticleHelper.CreateParticle(Plugin.blood160, damageInfo.HitPoint, rotation);
                    return;
                }
            }
            else if (damageInfo.Damage < 30) //buckshot
            {
                ParticleHelper.CreateParticle(Plugin.blood13, damageInfo.HitPoint, rotationInv);
                return;
            }
            else if (damageInfo.Damage < 60) //smgs and hadnguns
            {
                ParticleHelper.CreateParticle(Plugin.blood13, damageInfo.HitPoint, rotationInv);
                ParticleHelper.CreateParticle(Plugin.blood20, damageInfo.HitPoint, rotation);
                return;
            }
            else if (damageInfo.Damage < 100) //5.56
            {
                ParticleHelper.CreateParticle(Plugin.blood13, damageInfo.HitPoint, rotationInv);
                ParticleHelper.CreateParticle(Plugin.blood40, damageInfo.HitPoint, rotation);
                return;
            }
            else //7.62
            {
                ParticleHelper.CreateParticle(Plugin.blood20, damageInfo.HitPoint, rotationInv);
                ParticleHelper.CreateParticle(Plugin.blood80, damageInfo.HitPoint, rotation);
                return;
            }
        }
    }
}
