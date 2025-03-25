using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/SetStReflectionFallDelayTrigger")]
	public class SetStReflectionFallDelayTrigger : Trigger {
		public static bool HooksLoaded = false;

		private readonly int delay;

		private bool playerInside = false;

		public SetStReflectionFallDelayTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			delay = data.Int("delay");
		}

		public override void Awake(Scene scene) {
			base.Awake(scene);

			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Celeste.Player.ReflectionFallCoroutine += reflectionFallCoroutine;
			HooksLoaded = true;
		}

		public static void Unload() {
			On.Celeste.Player.ReflectionFallCoroutine -= reflectionFallCoroutine;
			HooksLoaded = false;
		}

		private static IEnumerator reflectionFallCoroutine(On.Celeste.Player.orig_ReflectionFallCoroutine orig, Player self) {
			SetStReflectionFallDelayTrigger trigger = self.level.Tracker.GetEntities<SetStReflectionFallDelayTrigger>().OfType<SetStReflectionFallDelayTrigger>().FirstOrDefault(t => t.playerInside);
			if (trigger != null) {
				self.Sprite.Play("bigFall");

				// wait before entering
				for (int i = 0; i < trigger.delay; i++) {
					self.Speed.Y = 0f;
					yield return null;
				}

				// start falling at max speed
				FallEffects.Show(true);
				self.Speed.Y = 320f;

				// wait for water
				while (!self.Scene.CollideCheck<Water>(self.Position))
					yield return null;
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);

				FallEffects.Show(false);
				self.Sprite.Play("bigFallRecover");

				self.StateMachine.State = 0;
			} else {
				IEnumerator origEnum = orig(self);
				while (origEnum.MoveNext())
					yield return origEnum.Current;
			}
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
