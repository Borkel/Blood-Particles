using BloodParticles;
using EFT;
using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace BloodParticlesFikaSync
{
	public static class BloodParticlesSerializationExtensions
	{
		public static void PutBloodParticlePacket(this NetDataWriter writer, ParticleInfo data)
		{
			writer.Put(data.Damage);
			writer.Put((byte)data.DamageType);
			writer.Put(data.Position);
			writer.Put(data.Direction);
			writer.Put((byte)data.EmitterShape);
			writer.Put(data.BloodAngle);
			writer.Put(data.BloodAmountMultiplier);
			writer.Put(data.BloodVelocityMin);
			writer.Put(data.BloodVelocityMax);
		}

		public static ParticleInfo GetBloodParticlePacket(this NetDataReader reader)
		{
			ParticleInfo data = new()
			{
				Damage = reader.GetFloat(),
				DamageType = (EDamageType)reader.GetByte(),
				Position = reader.GetVector3(),
				Direction = reader.GetVector3(),
				EmitterShape = (ParticleSystemShapeType)reader.GetByte(),
				BloodAngle = reader.GetFloat(),
				BloodAmountMultiplier = reader.GetFloat(),
				BloodVelocityMin = reader.GetFloat(),
				BloodVelocityMax = reader.GetFloat()
			};

			return data;
		}
	}
}
