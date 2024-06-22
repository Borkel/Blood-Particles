using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BloodParticles.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BloodParticles
{
    [BepInPlugin("com.borkel.bloodparticles", "Borkel's Blood Particles", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        public static readonly string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //Silly Balls Config Settings
        
        public static GameObject bloodParticlesHead;
        public static GameObject bloodParticlesHeadShort;
        public static GameObject bloodParticlesBody;
        public static GameObject bloodParticlesBodyShort;

        public static ConfigEntry<bool> LongerParticles;
        void Awake()
        {
            LongerParticles = Config.Bind("0.Particles", "Longer lasting particles", false, "Extends the duration of the particles");

            string pluginDirectory = $"{directory}";//plugin folder
            string bundlePath = $"{pluginDirectory}\\bloodparticles";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
                Logger.LogMessage("assetbundle");
            bloodParticlesHead = assetBundle.LoadAsset<GameObject>("BloodParticlesHead");
            bloodParticlesHeadShort = assetBundle.LoadAsset<GameObject>("BloodParticlesHeadShort");
            bloodParticlesBody = assetBundle.LoadAsset<GameObject>("BloodParticlesBody");
            bloodParticlesBodyShort = assetBundle.LoadAsset<GameObject>("BloodParticlesBodyShort");
            if(bloodParticlesBody == null || bloodParticlesBodyShort==null || bloodParticlesHead == null || bloodParticlesHeadShort==null)
            {
                Logger.LogError("error loading particles assets");
                return;
            }
            new ApplyDamageInfoPatch().Enable();

            Logger.LogInfo("Loaded BloodParticles");
        }
    }
}
