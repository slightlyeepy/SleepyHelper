using System;
using System.Collections;

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/NoDeathTrigger")]
	public class NoDeathTrigger : Trigger {
		private static bool enabled = false;

		public NoDeathTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public static void Load() {
			On.Celeste.Player.Die += maybeDie;
		}

		public static void Unload() {
			On.Celeste.Player.Die -= maybeDie;
		}

		private static PlayerDeadBody maybeDie(On.Celeste.Player.orig_Die orig, Player player, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true) {
			if (enabled) {
				return null;
			}
			return orig(player, direction, evenIfInvincible, registerDeathInStats);
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			enabled = true;
		}

		public override void OnStay(Player player) {
			base.OnStay(player);
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			enabled = false;
		}
	}
}
