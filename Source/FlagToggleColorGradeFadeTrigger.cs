// This file uses code from Maddie's Helping Hand -- see LICENSE.MaddiesHelpingHand file.

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FlagToggleColorGradeFadeTrigger")]
	[Tracked]
	public class FlagToggleColorGradeFadeTrigger : Trigger {
		public static bool HooksLoaded = false;

		public override void Awake(Scene scene) {
			base.Awake(scene);

			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Celeste.Level.Update += onLevelUpdate;
			HooksLoaded = true;
		}

		public static void Unload() {
			On.Celeste.Level.Update -= onLevelUpdate;
			HooksLoaded = false;
		}

		private static void onLevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
			orig(self);

			// check if the player is in a color grade fade trigger
			Player player = self.Tracker.GetEntity<Player>();
			FlagToggleColorGradeFadeTrigger trigger = self.Tracker.GetEntities<FlagToggleColorGradeFadeTrigger>().OfType<FlagToggleColorGradeFadeTrigger>().FirstOrDefault(t => (t.evenDuringReflectionFall ? player?.Collider.Collide(t) ?? false : t.playerInside) && self.Session.GetFlag(t.flag) != t.inverted);
			if (player != null && trigger != null) {
				DynData<Level> selfData = new DynData<Level>(self);

				// the game fades from lastColorGrade to Session.ColorGrade using colorGradeEase as a lerp value.
				// let's hijack that!
				float positionLerp = trigger.GetPositionLerp(player, trigger.direction);
				if (positionLerp > 0.5f) {
					// we are closer to B. let B be the target color grade when player exits the trigger / dies in it
					selfData["lastColorGrade"] = trigger.colorGradeA;
					self.Session.ColorGrade = trigger.colorGradeB;
					selfData["colorGradeEase"] = positionLerp;
				} else {
					// we are closer to A. let A be the target color grade when player exits the trigger / dies in it
					selfData["lastColorGrade"] = trigger.colorGradeB;
					self.Session.ColorGrade = trigger.colorGradeA;
					selfData["colorGradeEase"] = 1 - positionLerp;
				}
				selfData["colorGradeEaseSpeed"] = 1f;
			}
		}

		private readonly string colorGradeA;
		private readonly string colorGradeB;
		private readonly PositionModes direction;
		private readonly bool evenDuringReflectionFall;
		private readonly string flag;
		private readonly bool inverted;

		private bool playerInside = false;

		public FlagToggleColorGradeFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			colorGradeA = data.Attr("colorGradeA");
			colorGradeB = data.Attr("colorGradeB");
			direction = data.Enum<PositionModes>("direction");
			evenDuringReflectionFall = data.Bool("evenDuringReflectionFall", true); // true by default for backwards compatibility
			flag = data.Attr("flag");
			inverted = data.Bool("inverted");
		}

		public override void OnEnter(Player player) {
			playerInside = true;
		}

		public override void OnLeave(Player player) {
			playerInside = false;
		}
	}
}
