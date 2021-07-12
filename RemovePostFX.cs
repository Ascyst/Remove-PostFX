using BepInEx;
using HarmonyLib;
using System;
using UnboundLib;
using UnityEngine;
using BepInEx.Configuration;

namespace RemovePostFX
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModID, ModName, "2.0.0")]
    [BepInProcess("Rounds.exe")]
    public class RemovePostFX : BaseUnityPlugin
    {
        private void Awake()
        {
            rShakes = Config.Bind("RemovePostFX", "Remove Screen Shakes", true, "When enabled, removes screenshake.");
            rRain = Config.Bind("RemovePostFX", "Remove Chromatic Aberration", true, "When enabled, removes chromatic aberration.");
            rLight = Config.Bind("RemovePostFX", "Remove Light", true, "When enabled, removes the light and shadows from the game.");
            rTerrain = Config.Bind("RemovePostFX", "Remove Ground Effects", true, "When enabled, removes the ground effects, making the ground gray.");
            rPGlow = Config.Bind("RemovePostFX", "Remove Player Glow", true, "When enabled, removes the glow effect around the character. Also removes the pesky gun visual (overrated)");
            On.Screenshaker.OnGameFeel += this.Screenshaker_OnGameFeel;
            On.ChomaticAberrationFeeler.OnGameFeel += this.ChomaticAberrationFeeler_OnGameFeel;
        }

        private void Start()
        {
            new Harmony(ModID).PatchAll();
            Unbound.RegisterGUI(ModName, new Action(this.DrawGUI));
        }

        private void DrawGUI()
        {
            bool newrShakes = !GUILayout.Toggle(rShakes.Value, "Remove Screen Shakes");
            bool newrRain = !GUILayout.Toggle(rRain.Value, "Remove Chromatic Aberration (Rainbow Explosions)");
            bool newrTerrain = !GUILayout.Toggle(rTerrain.Value, "Remove colored ground (decent FPS boost)");
            bool newrPGlow = !GUILayout.Toggle(rPGlow.Value, "Remove the glow effect around players. Also removes the pesky gun visual (overrated)");
            bool newrLight = !GUILayout.Toggle(rLight.Value, "Remove the light from the top of the map, and the shadows it causes");

            rPGlow.Value = newrPGlow;
            rTerrain.Value = newrTerrain;
            rLight.Value = newrLight;
            rRain.Value = newrRain;
            rShakes.Value = newrShakes;
        }

        private void Screenshaker_OnGameFeel(On.Screenshaker.orig_OnGameFeel orig, Screenshaker self, Vector2 feelDirection)
        {
            orig(self, feelDirection * (rShakes.Value ? 1 : 0));
        }

        private void ChomaticAberrationFeeler_OnGameFeel(On.ChomaticAberrationFeeler.orig_OnGameFeel orig, ChomaticAberrationFeeler self, Vector2 feelDirection)
        {
            orig(self, feelDirection * (rRain.Value ? 1 : 0));
        }


        private const string ModID = "com.ascyst.rounds.removepostfx";

        private const string ModName = "Remove PostFX";

        public static ConfigEntry<bool> rShakes;

        public static ConfigEntry<bool> rRain;

        public static ConfigEntry<bool> rTerrain;

        public static ConfigEntry<bool> rPGlow;

        public static ConfigEntry<bool> rLight;
    }
    [HarmonyPatch(typeof(ArtHandler), "NextArt")]
    internal class ArtHandler_Patch
    {
        private static void Postfix()
        {
            foreach (var system in UnityEngine.Object.FindObjectsOfType<ParticleSystem>())
            {
                if (system.gameObject.name.Contains("Skin_Player"))
                {
                    system.SetPropertyValue("enableEmission", RemovePostFX.rPGlow.Value);
                }
            }

            var bgParticles = GameObject.Find("BackgroudParticles");
            if (bgParticles != null) bgParticles.SetActive(RemovePostFX.rTerrain.Value);

            var fgParticles = GameObject.Find("FrontParticles");
            if (fgParticles != null) fgParticles.SetActive(RemovePostFX.rTerrain.Value);

            var lightShake = GameObject.Find("Light");
            if (lightShake != null) lightShake.SetActive(RemovePostFX.rLight.Value);
        }
    }
}
