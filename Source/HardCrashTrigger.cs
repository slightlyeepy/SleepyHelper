using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/HardCrashTrigger")]
	public class HardCrashTrigger : Trigger {
		public HardCrashTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			Environment.Exit(0);
		}
	}
}
