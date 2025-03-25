using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/DisableDialogueConfirmTrigger")]
	[Tracked]
	public class DisableDialogueConfirmTrigger : Trigger {
		public static bool HooksLoaded = false;

		private bool playerInside = false;

		public DisableDialogueConfirmTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public override void Awake(Scene scene) {
			base.Awake(scene);

			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Celeste.Textbox.ContinuePressed += continuePressed;
			HooksLoaded = true;
		}

		public static void Unload() {
			On.Celeste.Textbox.ContinuePressed -= continuePressed;
			HooksLoaded = false;
		}

		private static bool continuePressed(On.Celeste.Textbox.orig_ContinuePressed orig, Textbox self) {
			DisableDialogueConfirmTrigger trigger = Engine.Scene.Tracker.GetEntities<DisableDialogueConfirmTrigger>().OfType<DisableDialogueConfirmTrigger>().FirstOrDefault(t => (t.playerInside));
			return (trigger != null) ? false : orig(self);
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			playerInside = true;
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			playerInside = false;
		}
	}
}

