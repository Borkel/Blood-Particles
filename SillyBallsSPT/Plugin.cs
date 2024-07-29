using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BloodParticles.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BloodParticles
{
    [BepInPlugin("com.borkel.bloodparticles", "Borkel's Blood Particles", "1.2.0")]
    public class Plugin : BaseUnityPlugin
    {
        //public static ManualLogSource logger;
        public static readonly string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //Silly Balls Config Settings

        public static GameObject blood13;
        public static GameObject blood20;
        public static GameObject blood40;
        public static GameObject blood80;
        public static GameObject blood160;

        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> SplatterEnabled;
        public static ConfigEntry<float> SplatterChance;
        void Awake()
        {
            Enabled = Config.Bind("0.Particles", "Enable blood particles", true, "Enables blood particles");
            SplatterEnabled = Config.Bind("0.Particles", "Blood splatters", true, "Enables blood splatters");
            SplatterChance = Config.Bind("0.Particles", "Blood splatter chance", 0.2f, "When a particle collides, chance to draw a splatter");

            string pluginDirectory = $"{directory}";//plugin folder
            string bundlePath = $"{pluginDirectory}\\bloodparticles";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
                Logger.LogMessage("assetbundle");
            blood13 = assetBundle.LoadAsset<GameObject>("blood13");
            blood20 = assetBundle.LoadAsset<GameObject>("blood20");
            blood40 = assetBundle.LoadAsset<GameObject>("blood40");
            blood80 = assetBundle.LoadAsset<GameObject>("blood80");
            blood160 = assetBundle.LoadAsset<GameObject>("blood160");

            if (blood13 == null ||  blood20 == null || blood40 == null
                || blood80 == null ||  blood160 == null)
            {
                Logger.LogError("error loading particles assets");
                return;
            }
            new ApplyDamageInfoPatch().Enable();

            Logger.LogInfo("Loaded BloodParticles");
        }
    }
}
