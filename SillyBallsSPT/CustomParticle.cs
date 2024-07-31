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

        public static void CreateBlood(DamageInfo damageInfo, Vector3 position, Quaternion rotation, float BloodVelocityMin, float BloodVelocityMax
            ,float BloodAngle, float BloodAmountMultiplier, bool headshot)
        {
            GameObject newParticle = Object.Instantiate(_bloodPrefab, position, rotation);
            float internalMultiplier=1f;

            // get the particle component
            ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Destroy; // just to make sure
            main.startSpeed = Random.Range(BloodVelocityMin, BloodVelocityMax);

            // set shape properties
            var shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = BloodAngle;
            shape.radius = 0.01f;
            shape.radiusThickness = 0.99f;
            shape.randomDirectionAmount = 0.5f;
            shape.randomPositionAmount = 0.1f;

            //take into account grenade fragmentation and headhsots
            if (headshot && damageInfo.DamageType != EDamageType.GrenadeFragment)
                internalMultiplier = 2f;
            else if (damageInfo.DamageType == EDamageType.GrenadeFragment)
            {
                shape.shapeType = ParticleSystemShapeType.Sphere;
                internalMultiplier = 0.20f;
            }

            // set emission properties
            var emission = particleSystem.emission;
            var burstInfo = emission.GetBurst(0);
            burstInfo.count = Mathf.CeilToInt((damageInfo.Damage / 5) * BloodAmountMultiplier * internalMultiplier);
            emission.SetBurst(0, burstInfo);

            // add collisionhandler component
            CollisionHandler collisionHandler = newParticle.AddComponent<CollisionHandler>();

            // start the emitter
            particleSystem.Play();
        }
    }
}
