using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace BloodParticles.Packets
{
    public class SpawnBloodParticlePacket : INetSerializable
    {
        public Vector3 spawnPosition;
        public bool type;

        public void Deserialize(NetDataReader reader)
        {
            spawnPosition = reader.GetVector3();
            type = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(spawnPosition);
        }
    }
}