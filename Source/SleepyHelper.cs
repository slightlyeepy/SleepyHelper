using System;
using System.Collections;

namespace Celeste.Mod.SleepyHelper;

public class SleepyHelper : EverestModule {
	public static SleepyHelper Instance { get; private set; }

	public static bool CustomReflectionFallDelayEnabled = false;
	public static int CustomReflectionFallDelay = 0;

	public SleepyHelper() {
		Instance = this;
#if DEBUG
		// debug builds use verbose logging
		Logger.SetLogLevel(nameof(SleepyHelper), LogLevel.Verbose);
#else
		// release builds use info logging to reduce spam in log files
		Logger.SetLogLevel(nameof(SleepyHelper), LogLevel.Info);
#endif
	}

	public override void Load() {
		On.Celeste.Player.ReflectionFallCoroutine += CustomReflectionFallCoroutine;
	}

	public override void Unload() {
		On.Celeste.Player.ReflectionFallCoroutine -= CustomReflectionFallCoroutine;
	}

	private static IEnumerator CustomReflectionFallCoroutine(On.Celeste.Player.orig_ReflectionFallCoroutine orig, Player self) {
		if (CustomReflectionFallDelayEnabled) {
			self.Sprite.Play("bigFall");

			// wait before entering
			for (int i = 0; i < CustomReflectionFallDelay; i++) {
				self.Speed.Y = 0f;
				yield return null;
			}

			// start falling at max speed
			FallEffects.Show(true);
			self.Speed.Y = 320f;

			// wait for water
			while (!self.Scene.CollideCheck<Water>(self.Position)) {
				yield return null;
			}
			Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);

			FallEffects.Show(false);
			self.Sprite.Play("bigFallRecover");

			self.StateMachine.State = 0;
		} else {
			yield return new SwapImmediately(orig(self));
		}
	}
}
