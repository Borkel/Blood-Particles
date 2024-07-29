using BepInEx.Logging;
using DeferredDecals;
using System.Collections.Generic;
using Systems.Effects;
using Comfort.Common;
using UnityEngine;

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
            if (!Plugin.SplatterEnabled.Value)
                return;
            /*ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            int particleCount = particleSystem.GetParticles(particles);*/

            int numEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;

            while (i < numEvents)
            {
                if(Random.value > Plugin.SplatterChance.Value)
                {
                    i++;
                    continue;
                }
                // variable soup
                LayerMask layerMask = 1 << LayerMask.NameToLayer("HighPolyCollider");
                Vector3 hitPos = collisionEvents[i].intersection;
                Vector3 hitVel = collisionEvents[i].velocity;
                Vector3 hitNormal = collisionEvents[i].normal;

                // set blood on surface that hit
                Singleton<Effects>.Instance.EmitBleeding(hitPos, hitNormal);

                // clear particle (i already do it in the particle system itself)
                //particles[i].remainingLifetime = 0f;
                //particleSystem.SetParticles(particles);

                i++;
            }
        }
    }

    public class ParticleHelper
    {
        public static void CreateParticle(GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            GameObject newParticle = Object.Instantiate(gameObject, position, rotation);

            if (newParticle != null)
            {
                ParticleSystem particleSystem = newParticle.GetComponent<ParticleSystem>();

                if (particleSystem != null) // NESTED IF STATEMENT AHHH!!!!!
                {
                    newParticle.AddComponent<CollisionHandler>();
                }
            }
        }
    }
}
