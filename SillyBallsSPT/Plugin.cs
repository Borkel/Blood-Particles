using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BloodParticles.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BloodParticles
{
    [BepInPlugin("com.borkel.bloodparticles", "Borkel's Blood Particles", "1.1.5")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        public static readonly string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //Silly Balls Config Settings
        
        public static GameObject blood13;
        public static GameObject blood13short;
        public static GameObject blood20;
        public static GameObject blood20short;
        public static GameObject blood40;
        public static GameObject blood40short;
        public static GameObject blood80;
        public static GameObject blood80short;
        public static GameObject blood160;
        public static GameObject blood160short;

        public static ConfigEntry<bool> LongerParticles;
        public static ConfigEntry<bool> Enabled;
        void Awake()
        {
            Enabled = Config.Bind("0.Particles", "Enable blood particles", false, "Enables blood particles");
            LongerParticles = Config.Bind("0.Particles", "Longer lasting particles", false, "Extends the duration of the particles");

            string pluginDirectory = $"{directory}";//plugin folder
            string bundlePath = $"{pluginDirectory}\\bloodparticles";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle == null)
                Logger.LogMessage("assetbundle");
            blood13 = assetBundle.LoadAsset<GameObject>("blood13");
            blood13short = assetBundle.LoadAsset<GameObject>("blood13short");
            blood20 = assetBundle.LoadAsset<GameObject>("blood20");
            blood20short = assetBundle.LoadAsset<GameObject>("blood20short");
            blood40 = assetBundle.LoadAsset<GameObject>("blood40");
            blood40short = assetBundle.LoadAsset<GameObject>("blood40short");
            blood80 = assetBundle.LoadAsset<GameObject>("blood80");
            blood80short = assetBundle.LoadAsset<GameObject>("blood80short");
            blood160 = assetBundle.LoadAsset<GameObject>("blood160");
            blood160short = assetBundle.LoadAsset<GameObject>("blood160short");
            
            if(blood13 == null || blood13short == null || blood20short == null || blood40 == null
                || blood40short == null || blood80 == null || blood80short == null || blood160 == null
                || blood160short== null)
            {
                Logger.LogError("error loading particles assets");
                return;
            }
            new ApplyDamageInfoPatch().Enable();

            Logger.LogInfo("Loaded BloodParticles");
        }
    }
}
