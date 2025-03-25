// This file uses code from Maddie's Helping Hand -- see LICENSE.MaddiesHelpingHand file.

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/ForceGrabController")]
	[Tracked]
	public class ForceGrabController : Entity {
		public static bool HooksLoaded = false;

		private static bool hooksEnabled = false;

		private static Hook buttonCheckHook;
		private static Hook buttonReleasedHook;
		private static Hook grabCheckHook;

		public ForceGrabController(EntityData data, Vector2 offset) : base(data.Position + offset) {
		}

		public override void Awake(Scene scene) {
			base.Awake(scene);

			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Celeste.Player.Update += playerUpdate;

			buttonCheckHook = new Hook(typeof(VirtualButton).GetProperty("Check").GetGetMethod(), typeof(ForceGrabController).GetMethod("onButtonCheck", BindingFlags.NonPublic | BindingFlags.Static));
			buttonReleasedHook = new Hook(typeof(VirtualButton).GetProperty("Released").GetGetMethod(), typeof(ForceGrabController).GetMethod("onButtonReleased", BindingFlags.NonPublic | BindingFlags.Static));
			grabCheckHook = new Hook(typeof(Input).GetProperty("GrabCheck").GetGetMethod(), typeof(ForceGrabController).GetMethod("modGrabResult", BindingFlags.NonPublic | BindingFlags.Static));

			HooksLoaded = true;
		}

		public static void Unload() {
			On.Celeste.Player.Update -= playerUpdate;

			buttonCheckHook?.Dispose();
			buttonReleasedHook?.Dispose();
			grabCheckHook?.Dispose();

			buttonCheckHook = null;
			buttonReleasedHook = null;
			grabCheckHook = null;

			HooksLoaded = false;
		}

		private static void playerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
			ForceGrabController controller = getController();
			if (controller != null)
				hooksEnabled = true;
			orig(self);
			if (controller != null)
				hooksEnabled = false;
		}

		private static bool onButtonCheck(Func<VirtualButton, bool> orig, VirtualButton self) {
			if (hooksEnabled && self == Input.Grab) {
				ForceGrabController controller = getController();
				return (controller != null) ? true : orig(self);
			}
			return orig(self);
		}

		private static bool onButtonReleased(Func<VirtualButton, bool> orig, VirtualButton self) {
			if (hooksEnabled && self == Input.Grab) {
				ForceGrabController controller = getController();
				return (controller != null) ? false : orig(self);
			}
			return orig(self);
		}

		private static bool modGrabResult(Func<bool> orig) {
			if (hooksEnabled) {
				ForceGrabController controller = getController();
				return (controller != null) ? true : orig();
			}
			return orig();
		}

		private static ForceGrabController getController() {
			// return null if the ForceGrabController type isn't tracked yet. this can happen when the mod is being loaded during runtime
			if (Engine.Scene == null || !Engine.Scene.Tracker.Entities.ContainsKey(typeof(ForceGrabController)))
				return null;

			return Engine.Scene.Tracker.GetEntity<ForceGrabController>();
		}
	}
}
