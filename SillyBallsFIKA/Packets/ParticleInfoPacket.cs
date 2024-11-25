using BloodParticles;
using LiteNetLib.Utils;

namespace BloodParticlesFikaSync.Packets
{
    public struct ParticleInfoPacket : INetSerializable
    {
        public ParticleInfo ParticleInfo;

        public void Deserialize(NetDataReader reader)
        {
            ParticleInfo = reader.GetBloodParticlePacket();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutBloodParticlePacket(ParticleInfo);
        }
    }
}