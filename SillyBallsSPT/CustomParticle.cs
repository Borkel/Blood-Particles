using BepInEx.Logging;
using System.Collections.Generic;
using Systems.Effects;
using Comfort.Common;
using UnityEngine;
using System.IO;
using System.Reflection;
using EFT;

namespace BloodParticles
{
    public struct ParticleInfo
    {
        public float Damage;
        public EDamageType DamageType;

        public Vector3 Position;
        public Vector3 Direction;
        public ParticleSystemShapeType EmitterShape;

        public float BloodAngle;
        public float BloodAmountMultiplier;
        public float BloodVelocityMin;
        public float BloodVelocityMax;

        public ParticleInfo(DamageInfo damageInfo, Vector3 position, Vector3 direction, ParticleSystemShapeType emitterShape, float bloodAngle, float bloodMult, float bloodMin, float bloodMax)
        {
            Damage = damageInfo.Damage;
            DamageType = damageInfo.DamageType;

            Position = position;
            Direction = direction;
            EmitterShape = emitterShape;

            BloodAngle = bloodAngle;
            BloodAmountMultiplier = bloodMult;
            BloodVelocityMin = bloodMin;
            BloodVelocityMax = bloodMax;
        }
    }

    public class CollisionHandler : MonoBehaviour
    {
        //public ManualLogSource logSource = new ManualLogSource("Particle");

        public ParticleSystem particleSystem;
        public List<ParticleCollisionEvent> collisionEvents;

        public void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
            //BepInEx.Logging.Logger.Sources.Add(logSource);
        }

        public void OnParticleCollision(GameObject other)
        {
            int numEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;

            while (i < numEvents)
            {
                // variable soup
                LayerMask layerMask = 1 << LayerMask.NameToLayer("HighPolyCollider");
                Vector3 hitPos = collisionEvents[i].intersection;
                Vector3 hitVel = collisionEvents[i].velocity;
                Vector3 hitNormal = collisionEvents[i].normal;
                float splatterRand = Random.Range(0.0f, 1.0f);

                // set blood on surface that hit
                if (splatterRand < Plugin.BloodSplatterChance.Value)
                {
                    Singleton<Effects>.Instance.EmitBleeding(hitPos, hitNormal);
                }

                i++;
            }
        }
    }

    public static class ParticleHelper
    {
        private static AssetBundle _bloodBundle;
        private static GameObject _bloodPrefab;

        public static void LoadParticleBundle()
        {
            string bundleDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string bundlePath = $"{bundleDirectory}\\bloodparticles";

            _bloodBundle = AssetBundle.LoadFromFile(bundlePath);
            _bloodPrefab = _bloodBundle.LoadAsset<GameObject>("blood13");
        }

        public static void CreateBlood(ParticleInfo particleInfo)
        {
            Vector3 position = particleInfo.Position;
            Quaternion rotation = Quaternion.LookRotation(particleInfo.Direction);
            EDamageType damageType = particleInfo.DamageType;

            GameObject newParticle = Object.Instantiate(_bloodPrefab, position, rotation);

            // set particle properties
            ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Destroy; // just to make sure
            main.startSpeed = new ParticleSystem.MinMaxCurve(particleInfo.BloodVelocityMin, particleInfo.BloodVelocityMax);
            
            // set shape properties
            var shape = particleSystem.shape;
            shape.shapeType = particleInfo.EmitterShape;
            shape.angle = particleInfo.BloodAngle;
            shape.radius = 0.01f;

            // i dont think these actually do anything significant but if they do then feel free to uncomment
            shape.randomDirectionAmount = 0.5f;
            shape.randomPositionAmount = 0.1f;

            // set emission properties
            var emission = particleSystem.emission;
            var burstInfo = emission.GetBurst(0);
            burstInfo.count = Mathf.CeilToInt((particleInfo.Damage / 5) * particleInfo.BloodAmountMultiplier);
            emission.SetBurst(0, burstInfo);

            // add collisionhandler component
            CollisionHandler collisionHandler = newParticle.AddComponent<CollisionHandler>();

            // start the emitter
            // or not since it starts itself anyway...
            // particleSystem.Play();
        }
    }
}
