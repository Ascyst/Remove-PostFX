using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnboundLib;
using UnityEngine;

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
            //            Unbound.RegisterGUI(ModName, new Action(this.DrawGUI));
            Unbound.RegisterMenu("Remove PostFX", () => { }, DrawOtherGUI);
        }

        private void DrawOtherGUI(GameObject obj)
        {
            Unbound.CreateText("Remove Post Effects Options", 85, obj, out _);
            Unbound.CreateToggle(rShakes.Value, "Remove Screen Shakes", 60, obj, flag => { rShakes.Value = flag; });
            Unbound.CreateToggle(rRain.Value, "Remove Chromatic Aberration", 60, obj, flag => { rRain.Value = flag; });
            Unbound.CreateToggle(rTerrain.Value, "Remove Ground Effects", 60, obj, flag => { rLight.Value = flag; });
            Unbound.CreateToggle(rPGlow.Value, "Remove Player Glow Effects", 60, obj, flag => { rTerrain.Value = flag; });
            Unbound.CreateToggle(rLight.Value, "Remove Light", 60, obj, flag => { rPGlow.Value = flag; });
        }
/*        private void DrawGUI()
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
*/
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
            Debug.Log("Adding Camera");
            // Only method I could find to consistently remove the background color from scene to scene.
            Camera bgCam = GameObject.Find("Post_Background").GetOrAddComponent<Camera>();
            bgCam.SetPropertyValue("backgroundColor", Color.black);
            Debug.Log("Added Camera, set bgcolor");
            Debug.Log("Removing PGlow");
            foreach (var system in GameObject.FindObjectsOfType<ParticleSystem>())
            {
                if (system.gameObject.name.Contains("Skin_Player"))
                {
                    system.SetPropertyValue("enableEmission", RemovePostFX.rPGlow.Value);
                }
            }

            Debug.Log("Removed PGlow");
            Debug.Log("Removing BGPart");
            GameObject.Find("BackgroudParticles").SetActive(!RemovePostFX.rTerrain.Value);
            Debug.Log("Removed BGPart");
            Debug.Log("Removing FPart");
            GameObject.Find("FrontParticles").SetActive(!RemovePostFX.rTerrain.Value);
            Debug.Log("Removed FPart");
            Debug.Log("Removing Light");
            GameObject.Find("LightShake").SetActive(RemovePostFX.rLight.Value);
            Debug.Log("Removed Light");
            Debug.Log("Set Camera active/inactive");
            bgCam.SetPropertyValue("enabled", RemovePostFX.rTerrain.Value);
            Debug.Log("Camera flip flops fine, what breaking? ");
        }
    }
}
