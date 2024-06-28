using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using BloodParticles.Packets;
using Comfort.Common;
using Fika.Core.Networking;
using LiteNetLib;
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
        public static void Prefix(DamageInfo damageInfo, EBodyPart bodyPartType, EBodyPartColliderType colliderType, float absorbed)
        {
            /*if (damageInfo.DamageType != EDamageType.Bullet && damageInfo.DamageType != EDamageType.Sniper && damageInfo.DamageType != EDamageType.Explosion
                && damageInfo.DamageType != EDamageType.GrenadeFragment && damageInfo.DamageType != EDamageType.Landmine &&
                damageInfo.DamageType != EDamageType.Melee)
                return; //maybe implement bleeds*/
            if (damageInfo.DamageType == EDamageType.Explosion || damageInfo.DamageType == EDamageType.Landmine
                                                               || damageInfo.BlockedBy != "" || damageInfo.DeflectedBy != "")
                return;
            bool longerParticles = Plugin.LongerParticles.Value;
            if (bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee &&
                damageInfo.DamageType != EDamageType.GrenadeFragment)
                SpawnBloodParticles(damageInfo.HitPoint, true, longerParticles); 
            else
                SpawnBloodParticles(damageInfo.HitPoint, false, longerParticles); 
            
            SpawnBloodParticlePacket bloodParticlePacket = new SpawnBloodParticlePacket();
            bloodParticlePacket.spawnPosition = damageInfo.HitPoint;
            bloodParticlePacket.type = bodyPartType == 0 && damageInfo.DamageType != EDamageType.Melee &&
                                       damageInfo.DamageType != EDamageType.GrenadeFragment;
            
            //Playing as server send packet to all clients
            if (Singleton<FikaServer>.Instantiated)
                Singleton<FikaServer>.Instance.SendDataToAll(Plugin.writer, ref bloodParticlePacket, DeliveryMethod.Unreliable);

            //Playing as client send packet to server
            if (Singleton<FikaClient>.Instantiated)
                Singleton<FikaClient>.Instance.SendData(Plugin.writer, ref bloodParticlePacket, DeliveryMethod.Unreliable);
        }

        public static void SpawnBloodParticles(Vector3 position, bool type, bool shortOrLong)
        {
            GameObject bloodParticles;
            if (type)
                bloodParticles = Object.Instantiate(shortOrLong ? Plugin.bloodParticlesHead : Plugin.bloodParticlesHeadShort, position, Quaternion.identity);
            else
                bloodParticles = Object.Instantiate(shortOrLong ? Plugin.bloodParticlesBody : Plugin.bloodParticlesBodyShort, position, Quaternion.identity);
            ParticleSystem particleSystem = bloodParticles.GetComponent<ParticleSystem>();
            if (particleSystem != null)
                particleSystem.Emit(1);
        }

        public static void SpawnBloodParticleFromServerOnClient(Vector3 position, bool type)
        {
            bool longerParticles = Plugin.LongerParticles.Value;
            SpawnBloodParticles(position, type, longerParticles);
        }
    }
}
