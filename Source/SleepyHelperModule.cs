﻿using System;
using System.Collections;

namespace Celeste.Mod.SleepyHelper;

public class SleepyHelperModule : EverestModule {
	public static SleepyHelperModule Instance { get; private set; }

	public static bool customReflectionFallDelayEnabled = false;
	public static int customReflectionFallDelay = 0;

	public SleepyHelperModule() {
		Instance = this;
#if DEBUG
		// debug builds use verbose logging
		Logger.SetLogLevel(nameof(SleepyHelperModule), LogLevel.Verbose);
#else
		// release builds use info logging to reduce spam in log files
		Logger.SetLogLevel(nameof(SleepyHelperModule), LogLevel.Info);
#endif
	}

	public override void Load() {
		On.Celeste.Player.ReflectionFallCoroutine += customReflectionFallCoroutine;
	}

	public override void Unload() {
		On.Celeste.Player.ReflectionFallCoroutine -= customReflectionFallCoroutine;
	}

	private static IEnumerator customReflectionFallCoroutine(On.Celeste.Player.orig_ReflectionFallCoroutine orig, Player self) {
		if (customReflectionFallDelayEnabled) {
			self.Sprite.Play("bigFall");

			// wait before entering
			for (int i = 0; i < customReflectionFallDelay; i++) {
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