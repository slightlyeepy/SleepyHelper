using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/SetStReflectionFallDelayTrigger")]
	public class SetStReflectionFallDelayTrigger : Trigger {
		private static bool customReflectionFallDelayEnabled = false;
		private static int customReflectionFallDelay;

		public SetStReflectionFallDelayTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			customReflectionFallDelay = data.Int("delay");
		}

		public static void Load() {
			On.Celeste.Player.ReflectionFallCoroutine += customReflectionFallCoroutine;
		}

		public static void Unload() {
			On.Celeste.Player.ReflectionFallCoroutine -= customReflectionFallCoroutine;
		}

		private static IEnumerator customReflectionFallCoroutine(On.Celeste.Player.orig_ReflectionFallCoroutine orig, Player player) {
			if (customReflectionFallDelayEnabled) {
				player.Sprite.Play("bigFall");

				// wait before entering
				for (int i = 0; i < customReflectionFallDelay; i++) {
					player.Speed.Y = 0f;
					yield return null;
				}

				// start falling at max speed
				FallEffects.Show(true);
				player.Speed.Y = 320f;

				// wait for water
				while (!player.Scene.CollideCheck<Water>(player.Position)) {
					yield return null;
				}
				Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);

				FallEffects.Show(false);
				player.Sprite.Play("bigFallRecover");

				player.StateMachine.State = 0;
			} else {
				yield return new SwapImmediately(orig(player));
			}
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			customReflectionFallDelayEnabled = true;
		}

		public override void OnStay(Player player) {
			base.OnStay(player);
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			customReflectionFallDelayEnabled = false;
		}
	}
}
