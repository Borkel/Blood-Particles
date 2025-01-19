using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BloodParticles.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BloodParticles
{
    [BepInPlugin("com.borkel.bloodparticles", "Borkel's Blood Particles", "1.2.4")]
    public class Plugin : BaseUnityPlugin
    {
        public static readonly string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static ConfigEntry<bool> Enabled;

        //public static ConfigEntry<float> BloodSize;
        public static ConfigEntry<float> BloodSplatterChance;
        public static ConfigEntry<float> HeadshotMultiplier;
        public static ConfigEntry<float> EntryBloodAmountMultiplier;
        public static ConfigEntry<float> EntryBloodVelocityMin;
        public static ConfigEntry<float> EntryBloodVelocityMax;
        public static ConfigEntry<float> EntryBloodAngle;
        public static ConfigEntry<float> ExitBloodVelocityMin;
        public static ConfigEntry<float> ExitBloodAmountMultiplier;
        public static ConfigEntry<float> ExitBloodVelocityMax;
        public static ConfigEntry<float> ExitBloodAngle;

        void Awake()
        {
            string general = "1. General Settings";
            string EntryBlood = "2. Entry Particle Settings";
            string ExitBlood = "3. Exit Particle Settings";

            // General settings
            Enabled = Config.Bind(general, "Enable Blood Particles", true, new ConfigDescription(
                    "Enables blood effects",
                    null,
                    new ConfigurationManagerAttributes { Order = 1000 }
                ));

            BloodSplatterChance = Config.Bind(general, "Blood Splatter Chance", 0.25f, new ConfigDescription(
                    "Changes the chance for blood particles to create a splatter",
                    new AcceptableValueRange<float>(0f, 1f),
                    new ConfigurationManagerAttributes { Order = 990 }
                ));

            HeadshotMultiplier = Config.Bind(general, "Headshot Multiplier", 3f, new ConfigDescription(
                    "Changes the amount of blood particles created on hit",
                    new AcceptableValueRange<float>(0.1f, 10f),
                    new ConfigurationManagerAttributes { Order = 985 }
                ));
            // Entry Particle settings
            EntryBloodAmountMultiplier = Config.Bind(EntryBlood, "Blood Amount Multiplier", 1f, new ConfigDescription(
                    "Changes the amount of blood particles created on hit",
                    new AcceptableValueRange<float>(0.1f, 5f),
                    new ConfigurationManagerAttributes { Order = 980 }
                ));

            EntryBloodVelocityMin = Config.Bind(EntryBlood, "Blood Minimum Velocity", 0.75f, new ConfigDescription(
                    "Changes the minimum initial velocity of blood particles",
                    new AcceptableValueRange<float>(0f, 10f),
                    new ConfigurationManagerAttributes { Order = 970 }
                ));

            EntryBloodVelocityMax = Config.Bind(EntryBlood, "Blood Maximum Velocity", 3.75f, new ConfigDescription(
                    "Changes the maximum initial velocity of blood particles",
                    new AcceptableValueRange<float>(0f, 10f),
                    new ConfigurationManagerAttributes { Order = 960 }
                ));

            EntryBloodAngle = Config.Bind(EntryBlood, "Blood Angle", 15f, new ConfigDescription(
                    "Changes the blood spray angle",
                    new AcceptableValueRange<float>(0f, 90f),
                    new ConfigurationManagerAttributes { Order = 950 }
                ));

            // Exit Particle settings
            ExitBloodAmountMultiplier = Config.Bind(ExitBlood, "Blood Amount Multiplier", 3f, new ConfigDescription(
                    "Changes the amount of blood particles created on hit",
                    new AcceptableValueRange<float>(0.1f, 5f),
                    new ConfigurationManagerAttributes { Order = 940 }
                ));


            ExitBloodVelocityMin = Config.Bind(ExitBlood, "Blood Minimum Velocity", 1f, new ConfigDescription(
                    "Changes the minimum initial velocity of blood particles",
                    new AcceptableValueRange<float>(0f, 10f),
                    new ConfigurationManagerAttributes { Order = 930 }
                ));

            ExitBloodVelocityMax = Config.Bind(ExitBlood, "Blood Maximum Velocity", 6f, new ConfigDescription(
                    "Changes the maximum initial velocity of blood particles",
                    new AcceptableValueRange<float>(0f, 10f),
                    new ConfigurationManagerAttributes { Order = 920 }
                ));

            ExitBloodAngle = Config.Bind(ExitBlood, "Blood Angle", 35f, new ConfigDescription(
                    "Changes the blood spray angle",
                    new AcceptableValueRange<float>(0f, 90f),
                    new ConfigurationManagerAttributes { Order = 910 }
                ));

            ParticleHelper.Instance.LoadParticleBundle();

            new ApplyDamageInfoPatch().Enable();

            Logger.LogInfo("Loaded Blood Particles");
        }
    }
}
