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
            Vector3 directionInv = direction * -1;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Quaternion rotationInv = Quaternion.LookRotation(directionInv);
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee && damageInfo.DamageType != EDamageType.GrenadeFragment) //if headshot
            {
                if (damageInfo.Damage < 30)
                {
                    Object.Instantiate(LongerParticles ? Plugin.blood13 : Plugin.blood13short, damageInfo.HitPoint, rotationInv);
                    Object.Instantiate(LongerParticles ? Plugin.blood80 : Plugin.blood80short, damageInfo.HitPoint, rotation);
                    return;
                } //buckshot
                else
                {
                    Object.Instantiate(LongerParticles ? Plugin.blood40 : Plugin.blood40short, damageInfo.HitPoint, rotationInv);
                    Object.Instantiate(LongerParticles ? Plugin.blood160 : Plugin.blood160short, damageInfo.HitPoint, rotation);
                    return;
                }
            }
            else if(damageInfo.Damage<30) //buckshot
            {
                Object.Instantiate(LongerParticles ? Plugin.blood13 : Plugin.blood13short, damageInfo.HitPoint, rotationInv);
                return;
            }
            else if (damageInfo.Damage < 60)
            {
                Object.Instantiate(LongerParticles ? Plugin.blood13 : Plugin.blood13short, damageInfo.HitPoint, rotationInv);
                Object.Instantiate(LongerParticles ? Plugin.blood20 : Plugin.blood20short, damageInfo.HitPoint, rotation);
                    return;
            }
            else if (damageInfo.Damage < 100)
            {
                Object.Instantiate(LongerParticles ? Plugin.blood20 : Plugin.blood20short, damageInfo.HitPoint, rotationInv);
                Object.Instantiate(LongerParticles ? Plugin.blood40 : Plugin.blood40short, damageInfo.HitPoint, rotation);
                    return;
            }
            else
            {
                Object.Instantiate(LongerParticles ? Plugin.blood40 : Plugin.blood40short, damageInfo.HitPoint, rotationInv);
                Object.Instantiate(LongerParticles ? Plugin.blood80 : Plugin.blood80short, damageInfo.HitPoint, rotation);
                    return;
            }
        }
    }
}
