// This file uses code from Maddie's Helping Hand:
//
// The MIT License (MIT)
//
// Copyright (c) 2019 maddie480
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FlagToggleColorGradeFadeTrigger")]
	[Tracked]
	public class FlagToggleColorGradeFadeTrigger : Trigger {
		public static void Load() {
			On.Celeste.Level.Update += onLevelUpdate;
		}

		public static void Unload() {
			On.Celeste.Level.Update -= onLevelUpdate;
		}

		private static void onLevelUpdate(On.Celeste.Level.orig_Update orig, Level self) {
			orig(self);

			// check if the player is in a color grade fade trigger
			Player player = self.Tracker.GetEntity<Player>();
			FlagToggleColorGradeFadeTrigger trigger = self.Tracker.GetEntities<FlagToggleColorGradeFadeTrigger>().OfType<FlagToggleColorGradeFadeTrigger>() .FirstOrDefault(t => (t.evenDuringReflectionFall ? player?.Collider.Collide(t) ?? false : t.playerInside) && self.Session.GetFlag(t.flag) != t.inverted);
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

		private string colorGradeA;
		private string colorGradeB;
		private PositionModes direction;
		private bool evenDuringReflectionFall;
		private string flag;
		private bool inverted;

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
