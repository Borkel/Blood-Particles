using BepInEx;
using BloodParticles.Patches;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Coop.Utils;
using Comfort.Common;
using Fika.Core.Networking;
using BloodParticles;
using BloodParticlesFikaSync.Packets;

namespace BloodParticlesFikaSync
{
    [BepInPlugin("com.borkel.bloodparticlesfikasync", "Borkel's Blood Particles Fika Sync", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static ApplyDamageInfoPatch _applyDamageInfoPatch;

        void Awake()
        {
            FikaEventDispatcher.SubscribeEvent<FikaGameCreatedEvent>(OnFikaGameCreated);
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnFikaNetworkManagerCreatedEvent);
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerDestroyedEvent>(OnFikaNetworkManagerDestroyedEvent);

            _applyDamageInfoPatch = new ApplyDamageInfoPatch();
            // Set default state of the patch to disabled
            _applyDamageInfoPatch.Disable();

            Logger.LogInfo("Loaded BloodParticles Fika Sync");
        }

        private void OnFikaGameCreated(FikaGameCreatedEvent @event)
        {
            ParticleHelper.Instance.OnBloodParticleCreated += OnBloodParticleCreated;
        }

        private void OnBloodParticleCreated(object sender, ParticleInfo ParticleInfo)
        {
            ParticleInfoPacket packet = new()
            {
                ParticleInfo = ParticleInfo
            };

            if (FikaBackendUtils.IsServer)
            {
                Singleton<FikaServer>.Instance.SendDataToAll(ref packet, LiteNetLib.DeliveryMethod.Unreliable);
            }
        }

        private void OnFikaNetworkManagerCreatedEvent(FikaNetworkManagerCreatedEvent ManagerCreatedEvent)
        {
            // Enable this to run only on the host, host will broadcast to clients
            if (FikaBackendUtils.IsServer)
            {
                _applyDamageInfoPatch.Enable();
            }

            ManagerCreatedEvent.Manager.RegisterPacket<ParticleInfoPacket>(OnPacketReceived);
        }

        private void OnFikaNetworkManagerDestroyedEvent(FikaNetworkManagerDestroyedEvent ManagerCreatedEvent)
        {
            // Disable again for the case if the current host is not the host next raid
            if (FikaBackendUtils.IsServer)
            {
                _applyDamageInfoPatch.Disable();
            }
        }

        private void OnPacketReceived(ParticleInfoPacket packet)
        {
            ParticleHelper.Instance.CreateBlood(packet.ParticleInfo, false);
        }
    }
}
