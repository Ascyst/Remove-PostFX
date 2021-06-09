using System;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using BepInEx;
using HarmonyLib;

namespace RemovePostFX
{

	[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(ModID, ModName, "1.0.0.0")]
	[BepInProcess("Rounds.exe")]
	public class RemovePostFX : BaseUnityPlugin
	{
		private void Awake()
		{
			On.Screenshaker.OnGameFeel += Screenshaker_OnGameFeel;
			On.ChomaticAberrationFeeler.OnGameFeel += ChomaticAberrationFeeler_OnGameFeel;
		}

		private void Start()
		{
			Unbound.RegisterGUI(ModName, new Action(this.DrawGUI));
		}

		private void DrawGUI()
		{
			bool flagS = GUILayout.Toggle(RemovePostFX.rShakes, "Remove Shakes");
			RemovePostFX.rShakes = flagS;
			bool flagR = GUILayout.Toggle(RemovePostFX.rRain, "Remove Chromatic Aberration (Rainbows)");
			RemovePostFX.rRain = flagR;
		}
		private void Screenshaker_OnGameFeel(On.Screenshaker.orig_OnGameFeel orig, Screenshaker self, Vector2 feelDirection)
		{
            orig(self, feelDirection * (rShakes ? 0 : 1));
		}
		private void ChomaticAberrationFeeler_OnGameFeel(On.ChomaticAberrationFeeler.orig_OnGameFeel orig, ChomaticAberrationFeeler self, Vector2 feelDirection)
		{
			orig(self, feelDirection * (rRain ? 0 : 1));
		}


		private const string ModID = "com.ascyst.rounds.removepostfx";

		private const string ModName = "Remove PostFX";

		public static bool rShakes;

		public static bool rRain;

	}
}
